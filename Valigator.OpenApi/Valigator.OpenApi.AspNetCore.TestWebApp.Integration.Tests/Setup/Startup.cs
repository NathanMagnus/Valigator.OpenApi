using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using NJsonSchema.Generation;
using NSwag;
using NSwag.Generation.Processors.Contexts;
using System;
using System.Collections.Generic;
using Validator.OpenApi.Integration.Tests.TestResources;
using Valigator;
using Valigator.OpenApi.AspNetCore;
using Valigator.OpenApi.AspNetCore.Authorization;
using Valigator.OpenApi.AspNetCore.Newtonsoft.Json;
using Valigator.OpenApi.AspNetCore.Setup;
using Valigator.OpenApi.AspNetCore.TestWebApp.Integration.Tests.Setup;

namespace Validator.OpenApi.Integration.Tests.Setup
{

	public class Startup
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddOptions();

			services
				.AddControllers(opts =>
				{
					//NOTE: Changing the order of Fo;ters shouldn't be done without good reason. The order they are registered in is the order they are run in and this could be important.
					opts.Filters.Add(typeof(ResultUnwrappingFilter));
					opts.Filters.Add(typeof(ValidateModelStateFilter));
					opts.Filters.Add(typeof(UnhandledExceptionWrappingFilter));

					// Add default produces and consumes of application/json
					opts.Filters.Add(new ProducesAttribute("application/json"));
					opts.Filters.Add(new ConsumesAttribute("application/json"));
				})
				.AddNewtonsoftJson(options =>
				{
					options.SerializerSettings.ContractResolver = new DefaultContractResolver();
					options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
					options.SerializerSettings.Converters.Add(CartIdentifierConverter.Instance);
				})
				.SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
				.AddValigator(errors => new BadRequestResult(), errors => new BadRequestResult()); ;

			var additionalMappings = new Dictionary<Type, Type>();
			additionalMappings.Add(typeof(GuidIdentifier), typeof(Guid));

			services.AddOpenApiWithValigator(
				new ValigatorOpenApiOptions(
					"Open Api Test Project Title",
					"Version",
					"Open Api Test Project Description",
					new[]
					{
						CreateServiceTokenAuthorization(info => info.OperationId != CreateOperationId(nameof(TestController), nameof(TestController.TestGetEndpoint))),
						CreateUserTokenAuthorization(info => info.OperationId != CreateOperationId(nameof(TestController), nameof(TestController.TestGetEndpoint)))
					},
					new Valigator.OpenApi.AspNetCore.Discriminators.DiscriminatorMappings(),
					c => c.AddExtraResponses(),
					new DefaultSchemaNameGenerator())
			);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app)
		{
			//NOTE: Changing the order of middlewares shouldn't be done without good reason. The order they are registered in is the order they are run in and this could be important.
			app.UseNSwag();
			app.UseRouting();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}

		public const string UserTokenSecurityRequirementKey = "UserToken";
		public static readonly OpenApiSecurityRequirement UserTokenRequirement = new OpenApiSecurityRequirement()
		{
			{UserTokenSecurityRequirementKey, new string[0] }
		};

		public static readonly OpenApiSecurityRequirement ServiceTokenRequirement = new OpenApiSecurityRequirement()
		{
			{ "ServiceToken", new string[0] }
		};

		/// <summary>
		/// User Token Authorization
		/// </summary>
		public static Authorization CreateUserTokenAuthorization(Func<EndpointInformation, bool> operationFilter = null, Func<OperationProcessorContext, IEnumerable<string>> operationProcessorAction = null)
			=> new Authorization(UserTokenSecurityRequirementKey, new OpenApiSecurityScheme()
			{
				In = OpenApiSecurityApiKeyLocation.Header,
				Description = "User Token from Auth service.",
				Name = "Bearer",
				Type = OpenApiSecuritySchemeType.ApiKey,
				BearerFormat = "Bearer [BearerToken]",
				Flow = OpenApiOAuth2Flow.Undefined
			},
			UserTokenRequirement,
			operationProcessorAction,
			operationFilter,
			Array.Empty<string>());


		/// <summary>
		/// Service Token Authorization
		/// </summary>
		public static Authorization CreateServiceTokenAuthorization(Func<EndpointInformation, bool> operationFilter = null, Func<OperationProcessorContext, IEnumerable<string>> operationProcessorAction = null)
			=> new Authorization("ServiceToken", new OpenApiSecurityScheme()
			{
				In = OpenApiSecurityApiKeyLocation.Header,
				Description = "Service Token from Auth service.",
				Name = "Bearer",
				Type = OpenApiSecuritySchemeType.ApiKey,
				BearerFormat = "Bearer [BearerToken]",
				Flow = OpenApiOAuth2Flow.Undefined
			},
			ServiceTokenRequirement,
			operationProcessorAction,
			operationFilter,
			Array.Empty<string>());

		public static string CreateOperationId(string controllerName, string methodName)
		{
			var index = controllerName.LastIndexOf("Controller");
			return $"{controllerName.Substring(0, index > -1 ? index : controllerName.Length)}_{methodName}";
		}
	}
}
