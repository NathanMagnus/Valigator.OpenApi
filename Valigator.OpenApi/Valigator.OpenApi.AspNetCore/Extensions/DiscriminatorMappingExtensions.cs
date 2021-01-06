using Newtonsoft.Json;
using NJsonSchema;
using NJsonSchema.Converters;
using System;
using Valigator.OpenApi.AspNetCore.Discriminators;
using Valigator.OpenApi.AspNetCore.ValigatorUnwrapping;

namespace Valigator.OpenApi.AspNetCore.Extensions
{
	internal static class DiscriminatorMappingExtensions
	{
		public static void CreateDiscriminator(this DiscriminatorMapping mapping, JsonSchema schema, Type parent, ValigatorSchemaGenerator generator)
		{
			var jsonConverterAttribute = new JsonConverterAttribute(typeof(JsonInheritanceConverter), mapping.PropertyName);
			GenerateInheritanceDiscriminator(parent, mapping, jsonConverterAttribute, schema, generator);
			GenerateMappedType(mapping, generator);
		}

		private static void GenerateInheritanceDiscriminator(Type parent, DiscriminatorMapping discriminator, JsonConverterAttribute jsonConverterAttribute, JsonSchema typeSchema, ValigatorSchemaGenerator generator)
		{
			typeSchema.DiscriminatorObject = new OpenApiDiscriminator
			{
				JsonInheritanceConverter = jsonConverterAttribute,
				PropertyName = discriminator.PropertyName
			};

			foreach (var (enumValue, type) in discriminator.Mappings)
			{
				var schema = generator.Generate(type, generator.Resolver);
				if (!parent.IsAssignableFrom(type))
					schema.AllOf.Add(generator.Generate(parent, generator.Resolver));

				typeSchema.DiscriminatorObject.Mapping.Add(enumValue.ToString(), new CustomJsonSchema { Reference = schema });
			}
		}

		private static void GenerateMappedType(DiscriminatorMapping mapping, ValigatorSchemaGenerator generator)
		{
			foreach (var (_, mappingType) in mapping.Mappings)
				GenerateKnownType(mappingType, generator);
		}

		private static void GenerateKnownType(Type type, ValigatorSchemaGenerator generator)
		{
			if (!generator.Resolver.HasSchema(type, type.IsEnum))
				generator.Generate(type, generator.Resolver);
		}

		public class CustomJsonSchema : JsonSchema { }
	}
}
