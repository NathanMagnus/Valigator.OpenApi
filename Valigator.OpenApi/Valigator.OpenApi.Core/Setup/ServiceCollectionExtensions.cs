using Microsoft.Extensions.DependencyInjection;
using NJsonSchema.Generation;
using NSwag.Generation.AspNetCore;
using System;
using Valigator.OpenApi.Core.Discriminators;
using Valigator.OpenApi.Core.TypeMapping;
using Valigator.OpenApi.Core.ValigatorUnwrapping;

namespace Valigator.OpenApi.Core.Setup
{
	public static partial class ServiceCollectionExtensions
	{
		public static IServiceCollection AddValigatorOpenApi(this IServiceCollection services, Action<ValigatorOpenApiOptions> confgureOptions)
		{
			var options = new ValigatorOpenApiOptions("OpenAPI Document", "v1", "Open API Description", Array.Empty<Authorization>(), new(), _ => { }, new DefaultSchemaNameGenerator());
			services.AddValigatorOpenApi(options);
			confgureOptions?.Invoke(options);
			return services;
		}

		public static IServiceCollection AddValigatorOpenApi(this IServiceCollection services, ValigatorOpenApiOptions options, Action<AspNetCoreOpenApiDocumentGeneratorSettings> optionAction)
			=> services.AddOpenApiWithValigator(options, optionAction);

		public static IServiceCollection AddValigatorOpenApi(this IServiceCollection services, ValigatorOpenApiOptions options)
			=> services.AddOpenApiWithValigator(options, _ => { });

		private static IServiceCollection AddOpenApiWithValigator(this IServiceCollection services, ValigatorOpenApiOptions options, Action<AspNetCoreOpenApiDocumentGeneratorSettings> optionAction)
		{
			services
				.AddOpenApiDocument(configuration =>
				{
					configuration.PostProcess = document =>
					{
						document.Info.Title = options.Title;
						document.Info.Version = options.Version;
						document.Info.Description = options.Description;
					};

					configuration.ReflectionService = new ValigatorUnwrappingReflectionService(configuration.ReflectionService, configuration);

					configuration.DefaultReferenceTypeNullHandling = ReferenceTypeNullHandling.NotNull;
					configuration.DefaultResponseReferenceTypeNullHandling = ReferenceTypeNullHandling.NotNull;

					configuration.SchemaGenerator = new ValigatorSchemaGenerator(configuration, options.DiscriminatorMappings);
					configuration.SchemaNameGenerator = options.SchemaNameGenerator;

					//configuration.AddSecurities(authorizations);
					configuration.OperationProcessors.Add(new AdditionalTypeMappingOperationProcessor(configuration));
					configuration.DocumentProcessors.Add(new DiscriminatorCleanupDocumentProcessor(options.DiscriminatorMappings));

					optionAction?.Invoke(configuration);
				});
			return services;
		}
	}
}