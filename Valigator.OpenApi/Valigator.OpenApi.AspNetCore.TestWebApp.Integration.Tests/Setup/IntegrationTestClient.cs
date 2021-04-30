using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Validator.OpenApi.Integration.Tests.Setup;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System;

namespace Validator.OpenApi.Integration.Tests
{
	public class IntegrationTestClient
	{
		private static TestServer _testServer;
		private static readonly object _testServerCreateLock = new object();

		public HttpClient TestClient { get; }

		protected IntegrationTestClient(string authToken, bool includeLanguageCode = false)
		{
			var httpClient = GetTestClient();
			httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {authToken}");
			if (includeLanguageCode)
				httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-us");

			TestClient = httpClient;
		}

		protected IntegrationTestClient() => TestClient = GetTestClient();

		protected virtual string GetServiceUrl() => "";

		private HttpClient GetTestClient()
		{
			var serviceUrl = GetServiceUrl();

			Console.WriteLine(serviceUrl);
			System.Diagnostics.Debug.WriteLine(serviceUrl);

			return String.IsNullOrWhiteSpace(serviceUrl)
				? GetTestServer().CreateClient()
				: new HttpClient()
				{
					BaseAddress = new Uri(serviceUrl)
				};
		}

		private TestServer GetTestServer()
		{
			lock (_testServerCreateLock)
			{
				if (_testServer == null)
					_testServer = CreateTestServer();
			}
			return _testServer;
		}

		private TestServer CreateTestServer()
		{
			var projectDir = Directory.GetCurrentDirectory();
			return new HostBuilder()
				.ConfigureWebHost(builder => builder
					.UseTestServer()
					.UseContentRoot(projectDir)
					.UseStartup<Startup>()
				)
				.Start()
				.GetTestServer();
		}

		public static IntegrationTestClient Create()
			=> new IntegrationTestClient();
	}
}
