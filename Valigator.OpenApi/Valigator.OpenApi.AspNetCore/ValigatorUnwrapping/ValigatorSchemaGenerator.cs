using Functional;
using Namotion.Reflection;
using NJsonSchema;
using NJsonSchema.Generation;
using NSwag;
using NSwag.Generation;
using NSwag.Generation.AspNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Valigator.AspNetCore;
using Valigator.Core;
using Valigator.Core.ValueDescriptors;
using Valigator.OpenApi.AspNetCore.Discriminators;
using Valigator.OpenApi.AspNetCore.Extensions;

namespace Valigator.OpenApi.AspNetCore.ValigatorUnwrapping
{
	internal class ValigatorSchemaGenerator : OpenApiSchemaGenerator
	{
		public JsonSchemaResolver Resolver { get; private set; }
		private AspNetCoreOpenApiDocumentGeneratorSettings _aspNetCoreSettings { get; }

		private readonly DiscriminatorMappings _discriminatorMappings;

		public ValigatorSchemaGenerator(AspNetCoreOpenApiDocumentGeneratorSettings aspNetCoreSettings, DiscriminatorMappings discriminatorMappings) : base(aspNetCoreSettings)
		{
			_discriminatorMappings = discriminatorMappings;
			_aspNetCoreSettings = aspNetCoreSettings;
		}

		public override void Generate<TSchemaType>(TSchemaType schema, ContextualType contextualType, JsonSchemaResolver schemaResolver)
		{
			if (Resolver == null)
				Resolver = schemaResolver;

			base.Generate(schema, contextualType, Resolver);
		}

		private void ApplyValigatorAttributes(JsonSchema schema, ContextualType contextualType, JsonTypeDescription typeDescription)
		{
			var newSchema = schema is OpenApiParameter openApiParameter
				? openApiParameter.Schema
				: schema;

			base.ApplyDataAnnotations(newSchema, typeDescription);

			ModifyRequiredPropertiesInformation(schema, typeDescription.ContextualType.Type);

			contextualType
				.ContextAttributes
				.OfType<IDescriptorProvider>()
				.Where(attribute => attribute.GetType().GetInterfaces().Any(i => i.GetGenericArguments().Any(genericType => genericType == contextualType.Type)))
				.Do(attribute => newSchema.ModifyPropertySchema(attribute.GetDescriptor(contextualType.Type), _aspNetCoreSettings))
				.ToArray();
		}

		public override void ApplyDataAnnotations(JsonSchema schema, JsonTypeDescription typeDescription)
		{
			ApplyValigatorAttributes(schema, typeDescription.ContextualType, typeDescription);

			typeDescription
				.ContextualType
				.ContextAttributes
				.OfType<ValigatorPropertyDataDescriptorAttribute>()
				.TryFirst()
				.Do(propertyDescriptorAttribute =>
				{
					propertyDescriptorAttribute
						.DataDescriptor
						.GetItemDataDescriptor()
						.Do(descriptor =>
						{
							var elementType = GetElementType(descriptor, typeDescription);
							var elementSchema = GenerateElementSchema(elementType, descriptor);

							if (!elementType.IsComplexObject())
								schema.Item = elementSchema;

							schema.ModifyPropertySchema(descriptor, _aspNetCoreSettings);

							schema.Type = JsonObjectType.Array;
							schema.Format = elementSchema.Format;
						});

					schema.ModifyPropertySchema(propertyDescriptorAttribute.DataDescriptor, _aspNetCoreSettings);
				});
		}

		private JsonSchema GenerateElementSchema(Type type, DataDescriptor descriptor)
		{
			var schema = GenerateWithReferenceAndNullability<JsonSchema>(type.ToContextualType(), Resolver);
			schema.ModifyPropertySchema(descriptor, _aspNetCoreSettings);
			return schema;
		}

		private Type GetElementType(DataDescriptor descriptor, JsonTypeDescription typeDescription)
			=> descriptor
				.MappingDescriptor
				.Match(
					mappingDescriptor => mappingDescriptor.SourceType,
					() => typeDescription.ContextualType.Type.GetElementType() ?? typeDescription.ContextualType.Type.GetGenericArguments().FirstOrDefault()
				)
				.UnwrapOption();

		/// <summary>Generates the properties for the given type and schema.</summary>
		/// <param name="schema">The properties</param>
		/// <param name="typeDescription">The type description.</param>
		/// <param name="schemaResolver">The schema resolver.</param>
		/// <returns>The task.</returns>
		protected override void GenerateObject(JsonSchema schema, JsonTypeDescription typeDescription, JsonSchemaResolver schemaResolver)
		{
			base.GenerateObject(schema, typeDescription, schemaResolver);
			ModifyRequiredPropertiesInformation(schema, typeDescription.ContextualType.Type)
				.Do(_ => { }, ex => throw ex);

			if (_discriminatorMappings.TryGetValue(typeDescription.ContextualType.Type, out var mapping) && Settings.GenerateKnownTypes)
				mapping.CreateDiscriminator(schema, typeDescription.ContextualType.Type, this);
		}

		protected override string[] GetTypeProperties(Type type)
			=> type.IsValigatorData()
				? Array.Empty<string>()
				: base.GetTypeProperties(type);

		private static Result<Option<object>, Exception> ModifyRequiredPropertiesInformation(JsonSchema schema, Type type)
			=> type
				.CreateInstance()
				.Do(opt => opt
					.Do(instance =>
						Model
						.GetPropertyDescriptors(instance)
						.Where(descriptor => descriptor.DataDescriptor.ValueDescriptors.Any(valueDescriptor => valueDescriptor is RequiredDescriptor))
						.Do(propertyDescriptor => AddPropertyDescriptor(schema.RequiredProperties, propertyDescriptor.PropertyName))
					)
				);

		private static void AddPropertyDescriptor(ICollection<string> requiredProperties, string propertyName)
		{
			if (!requiredProperties.Contains(propertyName))
				requiredProperties.Add(propertyName);
		}
	}
}
