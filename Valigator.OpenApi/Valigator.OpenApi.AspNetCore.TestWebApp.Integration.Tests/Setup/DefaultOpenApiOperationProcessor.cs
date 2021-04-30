using Functional;
using Namotion.Reflection;
using NJsonSchema;
using NJsonSchema.Generation;
using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Valigator.OpenApi.AspNetCore.Extensions;

namespace Validator.OpenApi.Integration.Tests.Setup
{
	public class DefaultOpenApiOperationProcessor : IOperationProcessor
	{
		public bool Process(OperationProcessorContext context)
		{
			AddConsumes(context.OperationDescription.Operation, "application/json");
			AddProduces(context.OperationDescription.Operation, "application/json");

			var result = Option.Some(context.MethodInfo.ReturnType.UnwrapTask());
			if (result.Match(s => s.IsResult(), () => false))
			{
				result
					.Bind(s => s.GetResultSuccess())
					.Apply(
						successType => AddResponse(HttpStatusCode.OK, context.OperationDescription.Operation.Summary ?? "Successful response.", successType, context.SchemaGenerator, context.SchemaResolver, context.OperationDescription.Operation),
						() => { }
					);

				result
					.Bind(s => s.GetResultFailure())
					.Apply(type =>
					{
						var failureType = type;
						AddResponse(HttpStatusCode.BadRequest, "Error due to bad input.", failureType, context.SchemaGenerator, context.SchemaResolver, context.OperationDescription.Operation);

						if (context.OperationDescription.Operation.ActualSecurity.Any())
							AddResponse(HttpStatusCode.Unauthorized, "Unauthorized bearer token.", failureType, context.SchemaGenerator, context.SchemaResolver, context.OperationDescription.Operation);
					},
					() => { });
			}

			return true;
		}

		private void AddResponse(HttpStatusCode code, string description, Type type, JsonSchemaGenerator generator, JsonSchemaResolver resolver, OpenApiOperation operation)
		{
			var schema = generator.GenerateWithReferenceAndNullability<JsonSchema>(type.ToContextualType(), resolver);

			var response = new OpenApiResponse
			{
				Description = description
			};
			response.Content.Add(type.GetOpenApiTypeName(), new OpenApiMediaType() { Schema = schema });

			operation.Responses[((int)code).ToString()] = response;
		}

		private void AddConsumes(OpenApiOperation operation, string consumes)
		{
			if (operation.Consumes == null)
				operation.Consumes = new List<string>();

			operation.Consumes.Clear();

			if (!operation.Consumes.Contains(consumes))
				operation.TryAddConsumes(consumes);
		}

		private void AddProduces(OpenApiOperation operation, string produces)
		{
			if (operation.Produces == null)
				operation.Produces = new List<string>();

			operation.Produces.Clear();

			if (!operation.Produces.Contains(produces))
				operation.Produces.Add(produces);
		}
	}
}
