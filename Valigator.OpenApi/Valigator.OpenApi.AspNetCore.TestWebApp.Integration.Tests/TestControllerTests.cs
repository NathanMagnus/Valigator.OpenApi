using FluentAssertions;
using Newtonsoft.Json.Linq;
using Validator.OpenApi.Integration.Tests.Setup;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using System;

namespace Validator.OpenApi.Integration.Tests
{
	public class TestControllerTests
	{
		[Fact]
		public async Task AnonymousObjectReturnsProperly()
		{
			var client = IntegrationTestClient.Create().TestClient;
			var uri = $"test/{Guid.NewGuid()}/anonymous";

			var result = await client.GetAsync(uri);
			await result.StatusCodeIsAsync(HttpStatusCode.OK);

			var json = JObject.Parse(await result.Content.ReadAsStringAsync());
			json.ToString().Should().Be(@"{
  ""InnerObject"": 2,
  ""OtherValue"": 2
}");
		}
	}
}
