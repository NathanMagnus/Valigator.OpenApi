using NJsonSchema;
using NJsonSchema.Generation;
using NJsonSchema.Generation.TypeMappers;
using System;

namespace Valigator.OpenApi.Core.TypeMapping
{
	internal class CustomPrimitiveTypeMapper : ITypeMapper
	{
		public CustomPrimitiveTypeMapper(Type mappedType, Action<JsonSchemaGenerator, JsonSchemaResolver, JsonSchema> schemaFactory)
		{
			MappedType = mappedType;
			_schemaModifyer = schemaFactory;
		}

		/// <summary>
		/// Gets the mapped type.
		/// </summary>
		public Type MappedType { get; }

		/// <summary>
		/// The action that will modify the schema
		/// </summary>
		private readonly Action<JsonSchemaGenerator, JsonSchemaResolver, JsonSchema> _schemaModifyer;

		/// <summary>Gets a value indicating whether to use a JSON Schema reference for the type.</summary>
		public bool UseReference { get; } = false;

		/// <summary>
		/// Gets the schema for the mapped type.
		/// </summary>
		/// <param name="schema">The schema.</param>
		/// <param name="context">The context.</param>
		public void GenerateSchema(JsonSchema schema, TypeMapperContext context)
		{
			_schemaModifyer(context.JsonSchemaGenerator, context.JsonSchemaResolver, schema);
		}
	}
}
