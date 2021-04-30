using Microsoft.AspNetCore.Builder;
using NSwag.AspNetCore;
using System;

namespace Valigator.OpenApi.AspNetCore.TestWebApp.Integration.Tests.Setup
{
	internal static class ApplicationBuilderExtensions
	{
		private const string _openApiJsonPath = "/openapi/api.json";

		public static void UseNSwag(this IApplicationBuilder app, Action<ReDocSettings> additionalSettingsAction = null)
			=> app
				.UseOpenApi(s => s.Path = _openApiJsonPath)
				.UseReDoc(settings =>
				{
					settings.Path = "/openapi";
					settings.DocumentPath = _openApiJsonPath;

					settings.AdditionalSettings.Add("showExtensions", true);
					settings.AdditionalSettings.Add("sortPropsAlphabetically", true);

					additionalSettingsAction?.Invoke(settings);
				});
	}
}
