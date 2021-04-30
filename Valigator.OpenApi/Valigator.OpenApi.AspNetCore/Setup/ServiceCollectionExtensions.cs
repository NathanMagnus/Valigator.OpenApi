using Microsoft.Extensions.DependencyInjection;
using NJsonSchema.Generation;
using NSwag.Generation.AspNetCore;
using System;
using Valigator.OpenApi.AspNetCore.Authorization;
using Valigator.OpenApi.AspNetCore.Discriminators;
using Valigator.OpenApi.AspNetCore.TypeMapping;
using Valigator.OpenApi.AspNetCore.ValigatorUnwrapping;

namespace Valigator.OpenApi.AspNetCore.Setup
{
	/// <summary>
	/// Extension methods for setting up OpenAPI with Valigator
	/// </summary>
	public static partial class ServiceCollectionExtensions
	{
		/// <summary>
		/// Add OpenAPI to a solution that uses Valigator.
		/// </summary>
		/// <param name="services">ServiceCollection</param>
		/// <param name="confgureOptions">Additional configuration</param>
		/// <returns>ServiceCollection</returns>
		public static IServiceCollection AddValigatorOpenApi(this IServiceCollection services, Action<ValigatorOpenApiOptions> confgureOptions)
		{
			var options = new ValigatorOpenApiOptions("OpenAPI Document", "v1", "Open API Description", Array.Empty<Authorization.Authorization>(), new(), _ => { }, new DefaultSchemaNameGenerator());
			services.AddValigatorOpenApi(options);
			confgureOptions?.Invoke(options);
			return services;
		}

		/// <summary>
		/// Add OpenAPI to a solution that uses Valigator
		/// </summary>
		/// <param name="services">ServiceCollection</param>
		/// <param name="options">Additional options to use</param>
		/// <param name="optionAction">Additional configuration for OpenAPI Document generation</param>
		/// <returns>ServiceCollection</returns>
		public static IServiceCollection AddValigatorOpenApi(this IServiceCollection services, ValigatorOpenApiOptions options, Action<AspNetCoreOpenApiDocumentGeneratorSettings> optionAction)
			=> services.AddOpenApiWithValigator(options, optionAction);

		/// <summary>
		/// Add OpenAPI to a solution that uses Valigator
		/// </summary>
		/// <param name="services">ServiceCollection</param>
		/// <param name="options">Additional options to use</param>
		/// <returns>ServiceCollection</returns>
		public static IServiceCollection AddValigatorOpenApi(this IServiceCollection services, ValigatorOpenApiOptions options)
			=> services.AddOpenApiWithValigator(options, _ => { });

		private static IServiceCollection AddOpenApiWithValigator(this IServiceCollection services, ValigatorOpenApiOptions options, Action<AspNetCoreOpenApiDocumentGeneratorSettings> optionAction)
			=> services
				.AddOpenApiDocument(configuration =>
				{
					configuration.PostProcess = document =>
					{
						document.Info.Title = options.Title;
						document.Info.Version = options.Version;
						document.Info.Description = options.Description;
					};

					AddSecurities(configuration, options.Authorizations);					

					configuration.ReflectionService = new ValigatorUnwrappingReflectionService(configuration.ReflectionService, configuration);

					configuration.DefaultReferenceTypeNullHandling = ReferenceTypeNullHandling.NotNull;
					configuration.DefaultResponseReferenceTypeNullHandling = ReferenceTypeNullHandling.NotNull;

					configuration.SchemaGenerator = new ValigatorSchemaGenerator(configuration, options.DiscriminatorMappings);
					configuration.SchemaNameGenerator = options.SchemaNameGenerator;
					
					configuration.OperationProcessors.Add(new AdditionalTypeMappingOperationProcessor(configuration));
					configuration.DocumentProcessors.Add(new DiscriminatorCleanupDocumentProcessor(options.DiscriminatorMappings));

					optionAction?.Invoke(configuration);
				});

		private static void AddSecurities(AspNetCoreOpenApiDocumentGeneratorSettings configuration, Authorization.Authorization[] authorizations)
			=> configuration.OperationProcessors.Add(new AuthorizationOperationProcessor(authorizations));
	}
}