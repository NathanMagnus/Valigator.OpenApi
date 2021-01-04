using NJsonSchema.Generation;
using NSwag.Generation.AspNetCore;
using System;
using Valigator.OpenApi.Core.Discriminators;

namespace Valigator.OpenApi.Core.Setup
{
	public record ValigatorOpenApiOptions(string Title, string Version, string Description, Authorization.Authorization[] Authorizations, DiscriminatorMappings DiscriminatorMappings, Action<AspNetCoreOpenApiDocumentGeneratorSettings> ConfigureNswagSettings, ISchemaNameGenerator SchemaNameGenerator);
}