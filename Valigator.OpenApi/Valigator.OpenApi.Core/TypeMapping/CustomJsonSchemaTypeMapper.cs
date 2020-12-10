using NJsonSchema;
using NJsonSchema.Generation;
using NJsonSchema.Generation.TypeMappers;
using System;
using Valigator.OpenApi.Core.Extensions;

namespace Valigator.OpenApi.Core.TypeMapping
{
	public class CustomJsonSchemaTypeMapper : ITypeMapper
	{
		private readonly Func<JsonSchemaGenerator, JsonSchemaResolver, JsonSchema, JsonSchema> _schemaFactory;

		public CustomJsonSchemaTypeMapper(Type originalType, Type targetType, Func<JsonSchemaGenerator, JsonSchemaResolver, JsonSchema, JsonSchema> schemaFactory)
		{
			_schemaFactory = schemaFactory;
			MappedType = originalType;
			TargetType = targetType;
		}

		public Type MappedType { get; }
		public Type TargetType { get; }

		public bool UseReference { get; } = true;

		public void GenerateSchema(JsonSchema schema, TypeMapperContext context)
		{
			if (!context.JsonSchemaResolver.HasSchema(context.Type, context.Type.IsIntEnum()))
				CreateAndAddSchema(schema, context);
			schema.Reference = context.JsonSchemaResolver.GetSchema(context.Type, context.Type.IsIntEnum());
		}

		private void CreateAndAddSchema(JsonSchema schema, TypeMapperContext context)
		{
			var newSchema = _schemaFactory(context.JsonSchemaGenerator, context.JsonSchemaResolver, schema);
			newSchema.Title = TargetType
				.UnwrapOption()
				.GetOpenApiTypeName();

			context
				.JsonSchemaResolver
				.AddMappingSchema(context.Type, newSchema);
		}
	}
}
