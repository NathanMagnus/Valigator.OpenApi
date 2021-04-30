using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Validator.OpenApi;
using Validator.OpenApi.Integration.Tests.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Validator.OpenApi.Integration.Tests
{
	public class SwaggerGenerationTests
	{
		[Fact]
		public async Task SwaggerGenerateProperly()
		{
			var client = IntegrationTestClient.Create();
			var uri = "openapi/api.json";

			var result = await client.TestClient.GetAsync(uri);
			await result.StatusCodeIsAsync(HttpStatusCode.OK);

			var json = JObject.Parse(await result.Content.ReadAsStringAsync());


			CheckRoot(json);
			CheckInfo(json);
			CheckServers(json);
			CheckPaths(json);
			CheckComponents(json);
			CheckSecurity(json);
		}

		private void CheckServers(JObject json)
		{
			json.GetPath("servers").ShouldBeEquivalentTo(
			new[]
			{
				new { url = "http://localhost"}
			});
		}

		private void CheckSecurity(JObject json)
		{
			json.CheckExists("security", 0, "ServiceToken");
			json.CheckExists("security", 1, "UserToken");
		}

		private void CheckComponents(JObject json)
		{
			CheckSchemas(json);
			CheckSecuritySchemes(json);
		}

		private void CheckSecuritySchemes(JObject json)
		{
			CheckServiceTokenSecurityScheme(json);
			CheckUserTokenSecurityScheme(json);
		}

		private void CheckUserTokenSecurityScheme(JObject json)
		{
			var userToken = json.GetPath("components", "securitySchemes", "UserToken");

			userToken.ShouldBeEquivalentTo(new
			{
				type = "apiKey",
				description = "User Token from Auth service.",
				name = "Bearer",
				@in = "header",
				bearerFormat = "Bearer [BearerToken]"
			});

			userToken.CheckExists("flows", "implicit");
		}

		private void CheckServiceTokenSecurityScheme(JObject json)
		{
			var serviceToken = json.GetPath("components", "securitySchemes", "ServiceToken");

			serviceToken.ShouldBeEquivalentTo(new
			{
				type = "apiKey",
				description = "Service Token from Auth service.",
				name = "Bearer",
				@in = "header",
				bearerFormat = "Bearer [BearerToken]"
			});

			serviceToken.CheckExists("flows", "implicit");
		}

		private Dictionary<string, object> CreateDictionary(string key, object value)
			=> CreateDictionary((key, value));

		private Dictionary<string, object> CreateDictionary(params (string Key, object Value)[] values)
		{
			var dictionary = new Dictionary<string, object>();
			foreach (var value in values)
				dictionary.Add(value.Key, value.Value);
			return dictionary;
		}

		private void CheckSchemas(JObject json)
		{
			var schemas = json.GetPath("components", "schemas");

			schemas.ShouldBeEquivalentTo(
				CreateDictionary(
					("TestGetEndpointResource", new
					{
						required = new[] { "Name", "ImageVersion", "AssemblyVersion", "Revision" },
						type = "object",
						additionalProperties = false,
						properties = new
						{
							Name = CreateDictionary(
								("type", "string"),
								("minLength", 1),
								("nullable", false)
							),
							ImageVersion = CreateDictionary(
								("type", "string"),
								("minLength", 1),
								("nullable", false)
							),
							AssemblyVersion = CreateDictionary(
								("type", "string"),
								("minLength", 1),
								("nullable", false)
							),
							Revision = CreateDictionary(
								("type", "string"),
								("minLength", 1),
								("nullable", false)
							),
							BuildDate = CreateDictionary(
								("nullable", true),
								("type", "string"),
								("format", "date-time")
							),
							AppEnvName = CreateDictionary(
								("type", "string"),
								("minLength", 100),
								("nullable", true)
							)
						}
					}
					),
					("TestError",
						new
						{
							required = new[] { "Message", "ErrorInformation", "InnerError", "StackTrace" },
							type = "object",
							additionalProperties = false,
							properties = new
							{
								Message = CreateDictionary(
									("type", "string"),
									("minLength", 1),
									("nullable", false)
								),
								ErrorInformation = CreateDictionary(
								("type", "array"),
										("nullable", false),
										("items", CreateDictionary("$ref", "#/components/schemas/TestErrorInformation"))
									),
								InnerError = CreateDictionary(
										("nullable", true)
									),
								StackTrace = CreateDictionary(
										("type", "string"),
										("nullable", true)
									)
							}
						}
					),
							("ExamplePostResource", new
							{
								required = new[]
								{
									"Str",
									"LessThanInclusive",
									"ExampleNested",
									"GreaterThanExclusive",
									"RangeMixed",
									"InSet",
									"Not0",
									"Date",
									"Money",
									"Int",
									"Int2",
									"Guid",
									"ExampleNestedArray",
									"CycleA",
									"CartIdentifier"
								},
								type = "object",
								additionalProperties = false,
								properties = new
								{
									Str = CreateDictionary(("type", "string"), ("nullable", false)),
									LessThanInclusive = CreateDictionary(
								("type", "array"),
								("format", "int32"),
								("minItems", 2),
								("nullable", false),
								("items", CreateDictionary(
									("type", "integer"),
									("format", "int32"),
									("maximum", 100.0),
									("nullable", false)
								))
							),
									LessThanExclusive = CreateDictionary(
									("type", "array"),
									("format", "int32"),
									("nullable", false),
									("items", CreateDictionary(
										("type", "integer"),
										("format", "int32"),
										("maximum", 200.0),
										("nullable", false),
										("exclusiveMaximum", true)
									))
							),
									GreaterThanInclusive = CreateDictionary(
									("type", "array"),
									("format", "int32"),
									("default", new[] { 1, 2 }),
									("minItems", 300),
									("nullable", false),
									("items", CreateDictionary(
										("type", "integer"),
										("format", "int32"),
										("minimum", 500.0),
										("nullable", false)
									))
							),
									ExampleNested = CreateDictionary(
								("nullable", false),
								("oneOf", new[] { CreateDictionary("$ref", "#/components/schemas/ExampleNestedClass") })
							),
									GreaterThanExclusive = CreateDictionary(
								("type", "array"),
								("format", "int32"),
								("nullable", false),
								("items", CreateDictionary(
									("type", "integer"),
									("format", "int32"),
									("minimum", 400.0),
									("nullable", false),
									("exclusiveMinimum", true)
								))
							),
									RangeMixed = CreateDictionary(
								("type", "array"),
								("format", "int32"),
								("nullable", false),
								("items", CreateDictionary(
									("type", "integer"),
									("format", "int32"),
									("maximum", 100.0),
									("minimum", 5.0),
									("nullable", false),
									("exclusiveMaximum", true)
									)
								)
							),
									InSet = CreateDictionary(
								("type", "array"),
								("format", "int32"),
								("nullable", false),
								("items", CreateDictionary(
									("type", "integer"),
									("format", "int32"),
									("nullable", false),
									("x-InSet", "{1, 3, 5, 7, 9}")
								))
							),
									Not0 = CreateDictionary(
								("type", "array"),
								("format", "int32"),
								("nullable", false),
								("items", CreateDictionary(
									("type", "integer"),
									("format", "int32"),
									("x-NotEquals", 0),
									("nullable", false)
								))
							),
									Date = CreateDictionary(
								("nullable", false),
								("type", "string"),
								("format", "date-time"),
								("x-Minimum", DateTime.Parse("1990-01-22"))
							),
									Money = CreateDictionary(
								("type", "number"),
								("format", "decimal"),
								("minimum", 0.0),
								("nullable", false),
								("exclusiveMinimum", true)
							),
									Int = CreateDictionary(
								("type", "integer"),
								("format", "int32"),
								("minimum", 1.0),
								("nullable", false)
							),
									Int2 = CreateDictionary(
								("type", "integer"),
								("format", "int32"),
								("maximum", 100.0),
								("nullable", false)
							),
									Guid = CreateDictionary(
										("type", "string"),
										("format", "guid"),
								("nullable", false)
							),
									ExampleNestedArray = CreateDictionary(
								("type", "array"),
								("nullable", false),
								("items", CreateDictionary("$ref", "#/components/schemas/ExampleNestedClass"))
							),
									CycleA = CreateDictionary(
								("nullable", false),
								("oneOf", new[] { CreateDictionary("$ref", "#/components/schemas/CycleA") })
							),
									ExampleEnumValue = CreateDictionary(
								("default", "Value2"),
								("nullable", false),
								("oneOf", new[] { CreateDictionary("$ref", "#/components/schemas/ExampleEnum") })
							)
								}
							}),
							("ExampleNestedClass", new
							{
								required = new[] { "Int", "Self" },
								type = "object",
								additionalProperties = false,
								properties = new
								{
									Int = CreateDictionary(
								("type", "integer"),
								("format", "int32"),
								("nullable", false)
							),
									Self = CreateDictionary(
								("nullable", false),
								("oneOf", new[] { CreateDictionary("$ref", "#/components/schemas/ExampleNestedClass") })
							)
								}
							}),
							("Guid", new { type = "string", format = "guid" }),
							("CycleA", new
							{
								required = new[] { "B" },
								type = "object",
								additionalProperties = false,
								properties = new
								{
									B = CreateDictionary(
								("type", "array"),
								("nullable", false),
								("items", CreateDictionary("$ref", "#/components/schemas/CycleB"))
							)
								}
							}),
							("CycleB", new
							{
								required = new[] { "C" },
								type = "object",
								additionalProperties = false,
								properties = new
								{
									C = CreateDictionary(
								("nullable", false),
								("oneOf", new[] { CreateDictionary("$ref", "#/components/schemas/CycleC") })
							)
								}
							}),
							("CycleC", new
							{
								required = new[] { "A" },
								type = "object",
								additionalProperties = false,
								properties = new
								{
									A = CreateDictionary(
								("nullable", false),
								("oneOf", new[] { CreateDictionary("$ref", "#/components/schemas/CycleA") })
							)
								}
							}),
							("ObjectWithOneProperty", new
							{
								type = "object",
								additionalProperties = false,
								properties = new
								{
									Property1 = new
									{
										type = "integer",
										format = "int32"
									}
								}
							}),
							("ExampleEnum", CreateDictionary(("type", "string"), ("description", ""), ("x-enumNames", new[] { "Value1", "Value2" }), ("enum", new[] { "Value1", "Value2" }))),
							("ResultOfExamplePostResourceAndTestError", new
							{
								type = "object",
								additionalProperties = false
							}),
							("TestErrorInformation", CreateDictionary(
								("required", new[] { "Type", "Information" }),
								("type", "object"),
								("additionalProperties", false),
								("properties", CreateDictionary(
									("Type", CreateDictionary(
										("type", "string"),
										("minLength", 1),
										("nullable", false)
									)),
								("Information", CreateDictionary(
										("nullable", false)
									))
								))
							)),
							 ("ResultOfTestGetEndpointResourceAndTestError", new
							 {
								 type = "object",
								 additionalProperties = false
							 })

			));
		}

		private void CheckRoot(JObject json)
		{
			json.GetPath("x-generator").ValidateValue("NSwag v13.9.4.0 (NJsonSchema v10.3.1.0 (Newtonsoft.Json v12.0.0.0))");
			json.GetPath("openapi").ValidateValue("3.0.0");
		}

		private void CheckInfo(JObject json)
		{
			json.GetPath<JObject>("info").ShouldBeEquivalentTo(new
			{
				title = "Open Api Test Project Title",
				description = "Open Api Test Project Description",
				version = "v1"
			});
		}

		private void CheckPaths(JObject json)
		{
			CheckTestGetEndpoint(json);
			CheckTestPostEndpoint(json);
		}

		private void CheckTestPostEndpoint(JObject json)
		{
			var route = $"/{(typeof(TestController).GetCustomAttributes(true).First(a => a.GetType() == typeof(RouteAttribute)) as RouteAttribute).Template}";
			var get = json.GetPath("paths", route, "post");
			var responses = get.GetPath("responses");

			get.ShouldBeEquivalentTo(
				new
				{
					tags = new[] { "Test" },
					operationId = "Test_TestPostEndpoint",
					parameters = new object[]
					{
						new Dictionary<string, object>()
						{
							{ "name", "range1To100" },
							{ "in", "query" },
							{ "schema", new
							{
								type="integer",
								format="int32",
								maximum=100.0,
								minimum=1.0
							} },
							{"x-position",1 }
						},
						new Dictionary<string, object>()
						{
							{ "name", "range200To300" },
							{ "in", "query" },
							{ "schema", new
							{
								type="integer",
								format="int32",
								maximum=300.0,
								minimum=200.0
							} },
							{"x-position",2 }
						},
						new Dictionary<string, object>()
						{
							{ "name", "stringWithLength" },
							{ "in", "query" },
							{ "schema", new
							{
								type="string",
								maxLength = 50,
								minLength = 5
							} },
							{"x-position",3 }
						},
						new Dictionary<string, object>()
						{
							{ "name", "objArray" },
							{ "in", "query" },
							{"explode", true},
							{ "schema", new
							{
								type="array",
								maxItems=5,
								minItems=1,
								items=new object()
							} },
							{"x-position",4 }
						},
						new Dictionary<string, object>()
						{
							{ "name", "Accept-Language" },
							{ "in", "header" },
							{ "schema", new
							{
								type="string",
							} },
							{"x-position",6 }
						},
							new Dictionary<string, object>()
						{
							{ "name", "testValigatorAttribute" },
							{ "in", "query" },
							{ "schema", CreateDictionary(
								("type","integer"),
								("format","int32"),
								("minimum",0.0m)
							) },
							{"x-position",7 }
						},
							CreateDictionary(
							("name", "fromHeaderNoRemoval")
						),
						CreateDictionary(
							("name", "testComplexObjectInRoute")
						)
					},
					requestBody = new Dictionary<string, object>()
					{
						{"x-name", "examplePostResource" },
						{"content", new Dictionary<string, object>()
							{
								{"application/json", new
									{
										schema = new Dictionary<string, string>()
										{
											{"$ref",  "#/components/schemas/ExamplePostResource"}
										}
									}
								}
							}
						},
						{"x-position", 5 }
					},
					responses = new Dictionary<string, object>()
					{
						{
							"200", new
							{
								description = "Successful response.",
								content = new
								{
									ExamplePostResource = new
									{
										schema = new Dictionary<string, string>()
										{
											{ "$ref",  "#/components/schemas/ExamplePostResource" }
										}
									}
								}
							}
						},
						{
							"400",new
							{
								description = "Error due to bad input.",
								content = new
								{
									TestError = new
									{
										schema = new Dictionary<string, string>()
										{
											{ "$ref", "#/components/schemas/TestError" }
										}
									}
								}
							}
						},
						{
							"401", new
							{
								description = "Unauthorized bearer token.",
								content = new
								{
									TestError = new
									{
										schema = new Dictionary<string, string>()
										{
											{ "$ref",  "#/components/schemas/TestError" }
										}
									}
								}
							}
						}
					},
					security = new object[] { new { ServiceToken = Array.Empty<object>() }, new { UserToken = Array.Empty<object>() } }
				});
		}

		private void CheckTestGetEndpoint(JObject json)
		{
			var route = $"/{(typeof(TestController).GetCustomAttributes(true).First(a => a.GetType() == typeof(RouteAttribute)) as RouteAttribute).Template}";
			var get = json.GetPath("paths", route, "get");
			var responses = get.GetPath("responses");

			get.ShouldBeEquivalentTo(
				new
				{
					tags = new[] { "Test" },
					operationId = "Test_TestGetEndpoint",
					responses = new Dictionary<string, object>()
					{
						{
							"200", new
							{
								description = "Successful response.",
								content = new
								{
									TestGetEndpointResource = new
									{
										schema = new Dictionary<string, string>()
										{
											{ "$ref",  "#/components/schemas/TestGetEndpointResource" }
										}
									}
								}
							}
						},
						{
							"400",new
							{
								description = "Error due to bad input.",
								content = new
								{
									TestError = new
									{
										schema = new Dictionary<string, string>()
										{
											{ "$ref", "#/components/schemas/TestError" }
										}
									}
								}
							}
						},
						{
							"401", new
							{
								description = "Unauthorized bearer token.",
								content = new
								{
									TestError = new
									{
										schema = new Dictionary<string, string>()
										{
											{ "$ref",  "#/components/schemas/TestError" }
										}
									}
								}
							}
						}
					},
					security = new[] { new object() }
				});
		}
	}
}
