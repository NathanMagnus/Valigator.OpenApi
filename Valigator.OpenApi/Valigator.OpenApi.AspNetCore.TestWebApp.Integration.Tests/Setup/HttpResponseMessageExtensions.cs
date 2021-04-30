using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Validator.OpenApi.Integration.Tests.Setup
{
	public static class HttpResponseMessageExtensions
	{
		public static async Task StatusCodeIsAsync(this HttpResponseMessage httpResponseMessage, HttpStatusCode httpStatusCode)
			=> Assert.True(httpResponseMessage.StatusCode == httpStatusCode, $"Expected: {httpStatusCode}, Actual: {httpResponseMessage.StatusCode}\nResponse:\n{await httpResponseMessage.Content.ReadAsStringAsync()}");

		public static async Task ContentValidatesAsync<T>(this HttpResponseMessage httpResponseMessage, Func<T, bool> contentValidation)
			=> Assert.True(contentValidation(
					JsonConvert.DeserializeObject<T>(
						await httpResponseMessage.Content.ReadAsStringAsync())),
				"Response content did not pass validation");

		public static async Task<U> GetContentAsync<T, U>(this HttpResponseMessage httpResponseMessage, Func<T, U> getFromContent)
			=> getFromContent(
					JsonConvert.DeserializeObject<T>(
						await httpResponseMessage.Content.ReadAsStringAsync()));

		public static async Task<T> GetContentAsync<T>(this HttpResponseMessage httpResponseMessage)
			=> JsonConvert.DeserializeObject<T>(
				await httpResponseMessage.Content.ReadAsStringAsync());

		public static async Task<JObject> GetContentAsync(this HttpResponseMessage httpResponseMessage)
			=> JsonConvert.DeserializeObject<JObject>(
				await httpResponseMessage.Content.ReadAsStringAsync());
	}
}
