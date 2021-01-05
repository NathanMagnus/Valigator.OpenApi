using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Valigator.OpenApi.AspNetCore.Setup;

namespace Valigator.OpenApi.AspNetCore.Newtonsoft.Json
{
	public static class ConfigurationExtensions
	{
		public static void AddOpenApiWithValigator(this IServiceCollection services, ValigatorOpenApiOptions options, IEnumerable<JsonConverter> converters)
			=> services.AddOpenApiWithValigator(options, converters.ToArray());

		public static void AddOpenApiWithValigator(this IServiceCollection services, ValigatorOpenApiOptions options, params JsonConverter[] converters)
			=> services
				.AddValigatorOpenApi(
					options, 
					configuration =>
					{
						foreach (var converter in converters)
							configuration.ActualSerializerSettings.Converters.Add(converter);
					}
				);
	}
}