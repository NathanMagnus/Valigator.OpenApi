using Functional;
using Namotion.Reflection;
using NJsonSchema;
using NJsonSchema.Generation;
using NJsonSchema.Generation.TypeMappers;
using NSwag.Generation.AspNetCore;
using System;
using System.Collections.Concurrent;
using Valigator.OpenApi.Core.Extensions;

namespace Valigator.OpenApi.Core.TypeMapping
{
	internal static class CustomTypeMappingHelpers
	{
		private static readonly ConcurrentDictionary<(Type PropertyType, Type SourceType), Unit> _typeMappingCache = new ConcurrentDictionary<(Type PropertyType, Type SourceType), Unit>();
		public static void AddCustomTypeMapping(this AspNetCoreOpenApiDocumentGeneratorSettings settings, Type targetType, Type originalType)
		{
			var key = CreateKey(targetType, originalType);
			if (_typeMappingCache.ContainsKey(key))
				return;

			_typeMappingCache.GetOrAdd(key, Unit.Value);
			settings.AddTypeMapper(targetType, originalType);
		}

		private static void AddTypeMapper(this AspNetCoreOpenApiDocumentGeneratorSettings settings, Type targetType, Type originalType)
			=> settings
				.TypeMappers
				.Add(
					targetType.IsComplexObject()
					? CreateComplexObjectTypeMapper(targetType, originalType)
					: CreatePrimitiveTypeMapper(targetType, originalType)
				);

		private static ITypeMapper CreatePrimitiveTypeMapper(Type targetType, Type originalType)
			=> new CustomPrimitiveTypeMapper(originalType, (generator, resolver, schema) =>
			{
				var primitiveSchema = generator.GenerateWithReferenceAndNullability<JsonSchema>(targetType.ToContextualType(), resolver);

				schema.Type = primitiveSchema.Type;
				schema.Format = primitiveSchema.Format;

				resolver.AddMappingSchema(targetType, primitiveSchema);
			});

		private static ITypeMapper CreateComplexObjectTypeMapper(Type targetType, Type originalType)
			=> new CustomJsonSchemaTypeMapper(originalType, targetType, (generator, resolver, schema) => generator.GenerateWithReferenceAndNullability<JsonSchema>(targetType.ToContextualType(), resolver));

		private static (Type TargetType, Type OriginalType) CreateKey(Type targetType, Type originalType)
			=> (targetType, originalType);

		public static void AddMappingSchema(this JsonSchemaResolver resolver, Type key, JsonSchema schema)
		{
			if (!resolver.HasSchema(key, key.IsIntEnum()))
				resolver.AddSchema(key, key.IsIntEnum(), schema);
		}
	}
}
