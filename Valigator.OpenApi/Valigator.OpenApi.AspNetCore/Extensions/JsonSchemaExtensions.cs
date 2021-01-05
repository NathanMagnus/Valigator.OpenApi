using Functional;
using Newtonsoft.Json;
using NJsonSchema;
using NSwag.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using Valigator.Core;
using Valigator.Core.StateDescriptors;
using Valigator.Core.ValueDescriptors;
using Valigator.OpenApi.AspNetCore.ValueDescriptorHandlers;

namespace Valigator.OpenApi.AspNetCore.Extensions
{
	internal static class JsonSchemaExtensions
	{
		public static void ModifyPropertySchema(this JsonSchema schema, DataDescriptor dataDescriptor, OpenApiDocumentGeneratorSettings settings)
		{
			schema.IsNullableRaw = true; // Everything will be nullable unless Valigator indicates NotNullable via  DataDescriptor

			var tuples = dataDescriptor
				.ValueDescriptors
				.Concat(dataDescriptor.MappingDescriptor.Match(s => s.SourceValueDescriptors, () => Array.Empty<IValueDescriptor>()))
				.SelectMany(d => DescriptorHandlerFactory.CreateDescriptorHandlers().Where(h => h.CanHandleValueDescriptor(d)).Select(h => (ValueDescriptor: d, Handler: h)))
				.Do(tuple => tuple.Handler.HandleDescriptor(tuple.ValueDescriptor, dataDescriptor.PropertyType.UnwrapOption(), schema));

			schema.SetDefaultValue(dataDescriptor, settings);
			schema.SetState(dataDescriptor);
		}

		public static IDictionary<string, object> GetOrCreateExtensionData(this JsonSchema schema)
			=> schema.ExtensionData ?? (schema.ExtensionData = new Dictionary<string, object>());

		public static Option<object> GetExtensionData(this JsonSchema schema, string key)
			=> Option.FromNullable(schema.GetOrCreateExtensionData().ContainsKey(key) ? schema.ExtensionData[key] : null);

		public static void AddExtensionData(this JsonSchema schema, string key, object value)
		{
			if (schema.ExtensionData == null)
				schema.ExtensionData = new Dictionary<string, object>();

			schema.ExtensionData.Remove(key);

			// If you want to try to get this into the same format as '>=X' in redoc, check out the following:
			// https://github.com/Mermade/oas-kit/blob/62bbf0189f8f3f4ba17aa8482e445ce1244cd275/packages/oas-validator/index.js
			// https://github.com/Rebilly/ReDoc/blob/301ca225ff82677e4a093f196cf8bbe46ccec076/src/components/Fields/FieldDetails.tsx
			schema.ExtensionData.Add(key, value);
		}

		public static void UpdateExtensionData(this JsonSchema schema, string oldKey, string newKey, object newValue)
		{
			schema.RemoveExtensionData(oldKey);
			schema.AddExtensionData(newKey, newValue);
		}

		public static void RemoveExtensionData(this JsonSchema schema, string key)
		{
			if (schema.ExtensionData == null)
				schema.ExtensionData = new Dictionary<string, object>();

			schema.ExtensionData.Remove(key);
		}

		private static void SetState(this JsonSchema resourceSchemaProperty, DataDescriptor dataDescriptor)
			=> ((dataDescriptor.StateDescriptor as StateDescriptor)?.DefaultValue ?? (dataDescriptor.StateDescriptor as CollectionStateDescriptor)?.DefaultValue.Map(x => (object)x) ?? Option.None<object>())
				.Do(_ => { }, () =>
				{
					if (!dataDescriptor.ValueDescriptors.OfType<RequiredDescriptor>().Any())
						resourceSchemaProperty.AddExtensionData("x-Optional", true);
				});

		private static void SetDefaultValue(this JsonSchema resourceSchemaProperty, DataDescriptor dataDescriptor, OpenApiDocumentGeneratorSettings settings)
			=> GetStateDescriptor(dataDescriptor)
				.Do(s => resourceSchemaProperty.Default = GetDefaultValue(s, settings));

		private static Option<object> GetStateDescriptor(DataDescriptor dataDescriptor)
		{
			if (dataDescriptor.StateDescriptor is StateDescriptor stateDescriptor)
				return stateDescriptor.DefaultValue;
			if (dataDescriptor.StateDescriptor is CollectionStateDescriptor collectionStateDescriptor)
				return collectionStateDescriptor.DefaultValue.Map(x => (object)x);
			return Option.None<object>();
		}

		private static object GetDefaultValue(object obj, OpenApiDocumentGeneratorSettings settings)
		{
			if (obj == null)
				return obj;

			var newObj = obj.GetType().IsFunctionalOption() ? GetOptionValue(obj) : obj;

			var newObjType = newObj.GetType();
			if (newObjType.IsEnum)
				return newObj.ToString();

			if (newObjType.IsComplexObject())
				return JsonConvert.DeserializeObject(JsonConvert.SerializeObject(newObj, settings.SerializerSettings));

			return newObj;
		}

		private static object GetOptionValue(dynamic obj)
			=> GetOptionalValue(obj);

		private static object GetOptionalValue<T>(Option<T> obj)
			=> obj.Match(some => some, () => default);

	}
}
