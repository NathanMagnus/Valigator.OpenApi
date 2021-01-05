using NJsonSchema.Generation;
using NSwag.Generation.AspNetCore;
using System;
using Valigator.OpenApi.AspNetCore.Discriminators;

namespace Valigator.OpenApi.AspNetCore.Setup
{
	public record ValigatorOpenApiOptions(string Title, string Version, string Description, Authorization.Authorization[] Authorizations, DiscriminatorMappings DiscriminatorMappings, Action<AspNetCoreOpenApiDocumentGeneratorSettings> ConfigureNswagSettings, ISchemaNameGenerator SchemaNameGenerator);
}