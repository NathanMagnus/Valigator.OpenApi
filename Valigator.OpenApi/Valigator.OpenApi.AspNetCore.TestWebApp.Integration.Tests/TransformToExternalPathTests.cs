using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Validator.OpenApi.Integration.Tests.Setup;
using Xunit;

namespace Validator.OpenApi.Integration.Tests
{
	public class TransformToExternalPathTests
	{
		[Theory]
		[InlineData("", "/openapi/index.html?url=/openapi/api.json")]
		[InlineData("openapi", "/openapi/index.html?url=/openapi/api.json")]
		[InlineData("/openapi", "/openapi/index.html?url=/openapi/api.json")]
		[InlineData("/openapi/", "/openapi/index.html?url=/openapi/api.json")]
		[InlineData("rqcart/openapi", "/rqcart/openapi/index.html?url=/rqcart/openapi/api.json")]
		[InlineData("/rqcart/openapi", "/rqcart/openapi/index.html?url=/rqcart/openapi/api.json")]
		[InlineData("/rqcart/openapi/", "/rqcart/openapi/index.html?url=/rqcart/openapi/api.json")]
		[InlineData("/a/b/c/d/openapi/", "/a/b/c/d/openapi/index.html?url=/a/b/c/d/openapi/api.json")]
		public async Task TransformValueIsAsExpected(string originalUrl, string expected)
		{
			var client = IntegrationTestClient.Create().TestClient;
			client.DefaultRequestHeaders.Add("Original-Path", originalUrl);

			var result = await client.GetAsync($"openapi");

			await result.StatusCodeIsAsync(HttpStatusCode.Redirect);

			result.Headers.Location.Should().Be(expected);
		}

		[Fact]
		public async Task TransformValueWithNoHeader()
		{
			var result = await IntegrationTestClient.Create().TestClient.GetAsync($"openapi");

			await result.StatusCodeIsAsync(HttpStatusCode.Redirect);

			result.Headers.Location.Should().Be("/openapi/index.html?url=/openapi/api.json");
		}

	}
}