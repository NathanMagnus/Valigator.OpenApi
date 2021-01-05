using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NJsonSchema.Generation;
using System;
using Valigator.OpenApi.AspNetCore.Discriminators;
using Valigator.OpenApi.AspNetCore.Setup;
using Valigator.OpenApi.AspNetCore.Newtonsoft.Json;
using Valigator.OpenApi.AspNetCore.TestWebApp.Controllers;

namespace Valigator.OpenApi.AspNetCore.TestWebApp
{
	public class Startup
	{
		private static readonly DefaultContractResolver _defaultContractResolver = new DefaultContractResolver();
		private readonly IConfiguration _configuration;

		public Startup(IConfiguration configuration)
			=> _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

		public void ConfigureServices(IServiceCollection services)
		{
			var serializerSettings = CreateJsonSerializerSettings();

			services
				.AddOptions();

			services.AddOpenApiWithValigator(
				new ValigatorOpenApiOptions(
					"Title",
					"Unversioned",
					"Description",
					Array.Empty<Authorization.Authorization>(),
					CreateDiscriminatorMappings(),
					s => { },
					new DefaultSchemaNameGenerator()
				),
				serializerSettings.Converters
			);
			//TODO: Register auth

			services
				.AddControllers(opts =>
				{
					// Add default produces and consumes of application/json
					opts.Filters.Add(new ProducesAttribute("application/json"));
					opts.Filters.Add(new ConsumesAttribute("application/json"));
				})
				.AddNewtonsoftJson(options =>
				{
					options.SerializerSettings.ContractResolver = _defaultContractResolver;
					foreach (var converter in serializerSettings.Converters)
						options.SerializerSettings.Converters.Add(converter);
				})
				.SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
				.AddValigator(errors => new BadRequestObjectResult(new { Error = "Input Error" }), errors => new BadRequestObjectResult(new { Error = "Result Error" }));
		}

		private DiscriminatorMappings CreateDiscriminatorMappings()
			=> new DiscriminatorMappings(
					(
						typeof(DiscriminatedObjectBase), 
						new DiscriminatorMapping(
							nameof(DiscriminatedObjectBase.Discriminator), 
							(DiscriminatorEnum.Object1, typeof(DiscriminatedObject1)),
							(DiscriminatorEnum.ObjectTwo, typeof(DiscriminatedObjectTwo))
						)
					)
				);

		private JsonSerializerSettings CreateJsonSerializerSettings()
		{
			var settings = new JsonSerializerSettings() { ContractResolver = _defaultContractResolver };
			settings.Converters.Add(new StringEnumConverter());
			return settings;
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostApplicationLifetime applicationLifetime)
		{
			//NOTE: Changing the order of middleware shouldn't be done without good reason. The order they are registered in is the order they are run in and this could be important.
			app
				.UseAuthentication();

			app
				.UseNSwag();

			app
				.UseStaticFiles()
				.UseRouting()
				.UseEndpoints(endpoints => endpoints.MapControllers())
				.UseEndpoints(endpoints => endpoints.MapGet("/", async context =>
				{
					context.Response.Redirect("/openapi/index.html?url=/openapi/api.json");
					//context.Response.Redirect("/openapi/api.json");
					await context.Response.CompleteAsync();
				}));
		}
	}
}
