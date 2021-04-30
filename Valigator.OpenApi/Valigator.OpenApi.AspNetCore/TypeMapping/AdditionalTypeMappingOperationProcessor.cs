using Functional;
using Namotion.Reflection;
using NJsonSchema;
using NSwag;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Valigator.OpenApi.AspNetCore.Exceptions;
using Valigator.OpenApi.AspNetCore.Extensions;
using Valigator.OpenApi.AspNetCore.ModelBinding;
using Valigator.OpenApi.AspNetCore.ValigatorUnwrapping;

namespace Valigator.OpenApi.AspNetCore.TypeMapping
{
	internal class AdditionalTypeMappingOperationProcessor : IOperationProcessor
	{
		private readonly AspNetCoreOpenApiDocumentGeneratorSettings _aspNetCoreSettings;

		public AdditionalTypeMappingOperationProcessor(AspNetCoreOpenApiDocumentGeneratorSettings aspNetCoreSettings)
		{
			_aspNetCoreSettings = aspNetCoreSettings;
		}

		public bool Process(OperationProcessorContext context)
			=> HandleAdditionalTypeMappings(context)
				.Match(_ => true, ex => throw ex);

		private Data<TValue> GetData<TValue>(IValidateType<TValue> validate)
			=> validate.GetData();

		private void HandleAdditionalTypeMappings<TValue>(OperationProcessorContext context, KeyValuePair<ParameterInfo, OpenApiParameter> parameter, IValidateType<TValue> validate)
			=> HandleAdditionalTypeMappings(context, parameter, (dynamic)validate.GetData());

		private void HandleAdditionalTypeMappings<TValue>(OperationProcessorContext context, KeyValuePair<ParameterInfo, OpenApiParameter> parameter, Data<TValue> data)
		{
			var dataDescriptor = data.DataDescriptor;
			var mapTo = dataDescriptor.PropertyType;

			_aspNetCoreSettings.AddAdditionalTypeMappings(dataDescriptor);

			var mapToSchema = context.SchemaGenerator.GenerateWithReferenceAndNullability<JsonSchema>(mapTo.ToContextualType(new[] { new ValigatorPropertyDataDescriptorAttribute(dataDescriptor) }), context.SchemaResolver);
			mapToSchema.ModifyPropertySchema(dataDescriptor, _aspNetCoreSettings);

			parameter.Value.Schema = mapToSchema;

			var originParameters = new HashSet<string>(parameter.Key.ParameterType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).Select(k => k.Name));

			// Get rid of the word "Value" that gets added
			context
				.OperationDescription
				.Operation
				.Parameters
				.Where(p => originParameters.Contains(p.Name))
				.ToArray()
				.Do(item => context.OperationDescription.Operation.Parameters.Remove(item));
		}

		private Result<Unit, Exception> HandleAdditionalTypeMappings(OperationProcessorContext context)
			=> context
				.Parameters
				.SelectMany(parameter => parameter.Key.CustomAttributes.Where(a => typeof(ValidateAttribute).IsAssignableFrom(a.AttributeType) || typeof(ValidateModelBinderAttribute).IsAssignableFrom(a.AttributeType)).Select(annotation => (Parameter: parameter, Annotation: annotation)))
				.Select(tuple =>
				{
					var annotation = tuple.Annotation;
					var parameter = tuple.Parameter;
					var annotationInterfaceType = annotation
						.AttributeType
						.GetInterfaces()
						.FirstOrDefault(i => i == typeof(IValidateType<>).MakeGenericType(parameter.Key.ParameterType));

					if (annotation.Constructor == null)
						return Result.Failure<Unit, Exception>(new MissingDefaultConstructorException(annotationInterfaceType));

					var unmappedInstance = annotation.Constructor.Invoke(null);
					if (unmappedInstance is OpenApiValidateModelBinderAttribute openApiInstance)
						HandleAdditionalTypeMappings(context, parameter, (dynamic)openApiInstance.GetData());
					else if (annotationInterfaceType?.IsAssignableFrom(unmappedInstance.GetType()) ?? false)
						HandleAdditionalTypeMappings(context, parameter, (dynamic)unmappedInstance);
					else
						return Result.Failure<Unit, Exception>(new InvalidTypeException(annotation.AttributeType, $"Object of type '{annotation.AttributeType}' must implement '{typeof(IValidateType<>)}' or inherit '{typeof(OpenApiValidateModelBinderAttribute)}'"));
					return Result.Success<Unit, Exception>(Unit.Value);
				})
				.TakeUntilFailure()
				.Map(_ => Unit.Value);
	}
}