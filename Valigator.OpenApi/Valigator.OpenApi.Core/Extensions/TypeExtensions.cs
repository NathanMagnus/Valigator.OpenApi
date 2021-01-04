using Functional;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Valigator.OpenApi.Core.Exceptions;

namespace Valigator.OpenApi.Core.Extensions
{
	internal static class TypeExtensions
	{
		private static readonly Func<Type, IEnumerable<Type>> _defaultUnwrappingFunc = item => item
			.UnwrapTask()
			.UnwrapOption()
			.UnwrapEnumerable()
			.UnwrapValigatorData()
			.UnwrapResult();

		public static Result<Option<object>, Exception> CreateInstance(this Type type)
			=> Result.Try(() =>
			{
				if (type.IsAbstract)
					throw new InvalidTypeException(type, $"'{type.Name}' is abstract. Abstract classes are not allowed as outputs.");

				if (type.IsInterface)
					throw new InvalidTypeException(type, $"'{type.Name}' is an interface. Interfaces are not allowed as outputs.");

				return Option.Create(type.GetConstructor(Type.EmptyTypes) != null, () => Activator.CreateInstance(type));
			});

		/// <summary>
		/// Determine if a particular property is a Valigator Data object
		/// </summary>
		public static bool IsValigatorData(this Type type)
			=> type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Data<>);

		public static bool IsTaskGeneric(this Type type)
			=> type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>);

		public static bool IsDictionary(this Type type)
			=> type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(IDictionary<,>) || type.GetGenericTypeDefinition() == typeof(Dictionary<,>));

		public static bool IsEnumerable(this Type type)
			=> (type.IsArray || type.GetInterface(nameof(IEnumerable)) != null) && type != typeof(string);

		public static bool IsResult(this Type type)
			=> type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Result<,>);

		public static bool IsIntEnum(this Type type)
			=> type.IsEnum;

		public static bool IsNullable(this Type type)
			=> type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

		public static string GetOpenApiTypeName(this Type type)
		{
			var backTickIndex = type
				.Name
				.IndexOf('`');

			return type
				.Name
				.Substring(0, backTickIndex >= 0 ? backTickIndex : type.Name.Length)
				.Replace("[]", String.Empty);
		}

		/// <summary>
		/// Determine if the object is "complex" (ie: not primitive). This will result in a $ref in the OpenApi's generated JSON.
		/// </summary>
		public static bool IsComplexObject(this Type type)
		{
			var unwrapped = type.UnwrapNullable();
			return 
				unwrapped != typeof(string) 
				&& unwrapped != typeof(decimal) 
				&& unwrapped != typeof(DateTime) 
				&& unwrapped != typeof(Guid) 
				&& (unwrapped.IsClass || unwrapped.IsInterface || unwrapped.IsEnum || unwrapped.IsValueType && !unwrapped.IsPrimitive);
		}

		public static bool IsNumericType(this Type type)
			=> Type.GetTypeCode(type) switch
			{
				TypeCode.Byte => true,
				TypeCode.SByte => true,
				TypeCode.UInt16 => true,
				TypeCode.UInt32 => true,
				TypeCode.UInt64 => true,
				TypeCode.Int16 => true,
				TypeCode.Int32 => true,
				TypeCode.Int64 => true,
				TypeCode.Decimal => true,
				TypeCode.Double => true,
				TypeCode.Single => true,
				_ => false
			};

		public static bool IsFunctionalOption(this Type type)
			=> type
				.IsGenericType && type.GetGenericTypeDefinition() == typeof(Option<>);

		public static Type UnwrapNullable(this Type type)
			=> type
				.UnwrapType(t => t.IsNullable() ? t = t.GetGenericArguments().First() : t);

		public static IEnumerable<Type> UnwrapDefault(this Type type)
			=> type
				.UnwrapType(_defaultUnwrappingFunc);

		public static IEnumerable<Type> UnwrapType(this Type type, Func<Type, IEnumerable<Type>> unwrappingFunction)
		{
			var stack = new Stack<Type>();
			stack.Push(type);

			while (stack.Count > 0)
			{
				var item = stack.Pop();
				var unwrapped = (unwrappingFunction ?? _defaultUnwrappingFunc).Invoke(item);
				if (unwrapped.Count() == 1 && unwrapped.FirstOrDefault() == item)
				{
					yield return item;
					continue;
				}

				foreach (var unwrappedType in unwrapped)
					stack.Push(unwrappedType);
			}
		}

		public static Type UnwrapType(this Type type, Func<Type, Type> unwrappingFunction)
			=> type.UnwrapType(t => new[] { unwrappingFunction(t) }).First();

		public static Type UnwrapEnumerable(this Type type)
		{
			var currentType = type;
			while (currentType.IsEnumerable())
			{
				if (currentType.HasElementType)
					currentType = currentType.GetElementType();
				else if (currentType.IsDictionary())
				{
					var generics = currentType.GetGenericArguments();
					currentType = typeof(KeyValuePair<,>).MakeGenericType(generics[0], generics[1]);
				}
				else if (currentType.IsGenericType)
					currentType = currentType.GetGenericArguments().First();
				else
					break;
			}
			return currentType;
		}

		public static Type UnwrapValigatorData(this Type type)
			=> type
				.UnwrapType(t => t.IsValigatorData() ? t.GetGenericArguments().First() : t);

		public static Type UnwrapOption(this Type type)
			=> type
				.UnwrapType(t => t.IsFunctionalOption() ? t.GetGenericArguments().First() : t);

		public static IEnumerable<Type> UnwrapResult(this Type type)
			=> type
				.UnwrapType(t =>
				{
					var results = new List<Type>();
					if (t.IsResult())
					{
						results.AddRange(t.GetSuccessType().UnwrapResult());
						results.AddRange(t.GetFailureType().UnwrapResult());
						return results;
					}

					return new[] { t };
				});

		public static Type UnwrapTask(this Type type)
			=> type
				.UnwrapType(t => t.IsTaskGeneric() ? t.GetGenericArguments().First() : t);

		public static Type GetSuccessType(this Type type)
			=> type
				.GetGenericArguments()?.FirstOrDefault();

		public static Type GetFailureType(this Type type)
			=> type
				.GetGenericArguments()?.LastOrDefault();

		//public static Option<Type> GetResultSuccess(this Type type)
		//	=> Option.Create(type.IsResult(), () => type.GetGenericArguments().First());

		//public static Option<Type> GetResultFailure(this Type type)
		//	=> Option.Create(type.IsResult(), () => type.GetGenericArguments().Last());
	}
}
