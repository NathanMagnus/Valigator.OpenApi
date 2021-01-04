using Microsoft.Extensions.DependencyInjection;
using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using Valigator.OpenApi.Core.Extensions;

namespace Valigator.OpenApi.Core.Authorization
{
	internal class AuthorizationOperationProcessor : IOperationProcessor
	{
		/// <summary>
		/// Add authorizations to contorllers
		/// </summary>
		/// <param name="authorizations">Authorizations to add</param>
		public AuthorizationOperationProcessor(Authorization[] authorizations)
			=> Authorizations = authorizations;

		public Authorization[] Authorizations { get; }

		public bool Process(OperationProcessorContext context)
		{
			context.OperationDescription.Operation.Security = new List<OpenApiSecurityRequirement>();
			Authorizations.Do(authorization =>
			{
				var requirement = new OpenApiSecurityRequirement();

				AddSecurityRequirements(requirement, authorization);

				var userScopes = authorization.ScopesFunc?.Invoke(context).ToArray() ?? Array.Empty<string>();
				var endpointInformation = new EndpointInformation(context.OperationDescription.Path, context.OperationDescription.Operation.OperationId, context.OperationDescription.Method, userScopes.Any());

				if (OperationPassesFilter(authorization, endpointInformation))
				{
					AddSecurityToOperationDescription(context, requirement);
					AddSecurityDefinitionToContextDocument(context, authorization);
					AddSecurityToContextDocument(context, requirement);
				}
			});

			AddDefaultAuthorizationIfNecessary(context);

			return true;
		}

		private void AddDefaultAuthorizationIfNecessary(OperationProcessorContext context)
		{
			if (!context.OperationDescription.Operation.Security.Any())
				context.OperationDescription.Operation.Security.Add(new OpenApiSecurityRequirement());
		}

		private bool OperationPassesFilter(Authorization authorization, EndpointInformation endpointInformation)
			=> authorization.OperationFilter?.Invoke(endpointInformation) ?? false;

		private void AddSecurityToOperationDescription(OperationProcessorContext context, OpenApiSecurityRequirement requirement)
			=> context.OperationDescription.Operation.Security.Add(requirement);

		private void AddSecurityDefinitionToContextDocument(OperationProcessorContext context, Authorization authorization)
		{
			if (!context.Document.SecurityDefinitions.ContainsKey(authorization.Name))
				context.Document.SecurityDefinitions.Add(authorization.Name, authorization.SecurityScheme);
		}

		private void AddSecurityToContextDocument(OperationProcessorContext context, OpenApiSecurityRequirement requirement)
		{
			if (!DocumentAlreadyHasRequirement(context, requirement))
				context.Document.Security.Add(requirement);
		}

		private void AddSecurityRequirements(OpenApiSecurityRequirement requirement, Authorization authorization)
			=> authorization
				.SecurityRequirement
				.Do(securityRequirement => requirement.Add(securityRequirement.Key, securityRequirement.Value));

		private bool DocumentAlreadyHasRequirement(OperationProcessorContext context, OpenApiSecurityRequirement requirement)
			=> context
				.Document
				.Security
				.SelectMany(existingRequirement => existingRequirement.Select(requirement => (requirement.Key, requirement.Value)))
				.Where(items => requirement.Any(r => r.Key == items.Key && items.Value.SequenceEqual(r.Value)))
				.Any();
	}
}