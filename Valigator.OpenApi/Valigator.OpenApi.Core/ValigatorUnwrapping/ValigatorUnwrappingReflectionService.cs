using Namotion.Reflection;
using Newtonsoft.Json;
using NJsonSchema.Generation;
using NSwag.Generation.AspNetCore;
using System.Linq;
using Functional;
using System;
using Valigator.OpenApi.Core.Extensions;
using Valigator.OpenApi.Core.Exceptions;
using Valigator.OpenApi.Core.Setup;

namespace Valigator.OpenApi.Core.ValigatorUnwrapping
{
	/// <summary>
	/// Custom reflection service that turns types into their most basic forms.
	/// </summary>
	internal class ValigatorUnwrappingReflectionService : IReflectionService
	{
		private readonly IReflectionService _reflectionService;

		private AspNetCoreOpenApiDocumentGeneratorSettings _aspNetCoreSettings { get; }

		public ValigatorUnwrappingReflectionService(IReflectionService reflectionService, AspNetCoreOpenApiDocumentGeneratorSettings aspNetCoreSettings)
		{
			_reflectionService = reflectionService;
			_aspNetCoreSettings = aspNetCoreSettings;
		}

		private Result<ContextualType, Exception> GetContextualType(ContextualType contextualType)
			=> GetValigatorContextualType(contextualType);

		private Result<ContextualType, Exception> GetValigatorContextualType(ContextualType contextualType)
			=> GetPropertyInfo(contextualType)
				.Match(
					info => info
						.InstanceType
						.CreateInstance()
						.Map(opt => opt
							.Match(obj => Model
								.GetPropertyDescriptors(obj)
								.First(d => d.PropertyName == info.PropertyName),
								() => throw new MissingDefaultConstructorException(info.InstanceType)
							)
						)
						.Do(propertyDescriptor => _aspNetCoreSettings.AddAdditionalTypeMappings(propertyDescriptor.DataDescriptor))
						.Map(propertyDescriptor => contextualType
							.Type
							.UnwrapType(t => t.UnwrapValigatorData().UnwrapOption())
							.ToContextualType(contextualType.ContextAttributes.Concat(new[] { new ValigatorPropertyDataDescriptorAttribute(propertyDescriptor.DataDescriptor) }))
					),
					() => Result.Success<ContextualType, Exception>(contextualType)
				);

		private Option<(Type InstanceType, string PropertyName)> GetPropertyInfo(ContextualType contextualType)
			=> Option
				.Create(contextualType.Type.IsValigatorData(), 1)
				.Match(
					_ => Option.FromNullable(contextualType as ContextualPropertyInfo),
					() => Option.None<ContextualPropertyInfo>()
				)
				.Match(
					contextualPropertyInfo => Option.Some((contextualPropertyInfo.PropertyInfo.DeclaringType, contextualPropertyInfo.Name)),
					() =>
					{
						var additionalDataAttributeOption = Option
							.FromNullable(contextualType
								.Attributes
								.OfType<ValigatorInformationAttribute>()
								.FirstOrDefault()
							);
						return additionalDataAttributeOption.Map(additionalDataAttribute => (additionalDataAttribute.ParentType, additionalDataAttribute.PropertyName));
					}
				);

		public JsonTypeDescription GetDescription(ContextualType contextualType, ReferenceTypeNullHandling defaultReferenceTypeNullHandling, JsonSchemaGeneratorSettings settings)
			=> GetContextualType(contextualType)
				.Match(
					type => _reflectionService.GetDescription(type, defaultReferenceTypeNullHandling, settings),
					ex => throw ex
				);

		public JsonTypeDescription GetDescription(ContextualType contextualType, JsonSchemaGeneratorSettings settings)
			=> GetContextualType(contextualType)
				.Match(
					type => _reflectionService.GetDescription(type, settings),
					ex => throw ex
				);

		public bool IsNullable(ContextualType contextualType, ReferenceTypeNullHandling defaultReferenceTypeNullHandling)
			=> GetContextualType(contextualType)
				.Match(
					type => _reflectionService.IsNullable(type, defaultReferenceTypeNullHandling),
					ex => throw ex
				);

		public bool IsStringEnum(ContextualType contextualType, JsonSerializerSettings serializerSettings)
			=> GetContextualType(contextualType)
				.Match(
					type => _reflectionService.IsNullable(type, ReferenceTypeNullHandling.NotNull),
					ex => throw ex
				);
	}
}
