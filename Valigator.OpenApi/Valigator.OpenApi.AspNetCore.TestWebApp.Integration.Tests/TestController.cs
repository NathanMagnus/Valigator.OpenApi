using Functional;
using Microsoft.AspNetCore.Mvc;
using Validator.OpenApi.Integration.Tests.Setup;
using Validator.OpenApi.Integration.Tests.TestResources;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Valigator;

namespace Validator.OpenApi.Integration.Tests
{
	[Route("test/{testComplexObjectInRoute}")]
	public partial class TestController : Controller
	{
		[HttpGet]
		public Result<TestGetEndpointResource, TestError> TestGetEndpoint()
			=> Result.Success<TestGetEndpointResource, TestError>((TestGetEndpointResource)null);

		[HttpPost]
		public Task<Result<ExamplePostResource, TestError>> TestPostEndpoint(
			[Range(1, 100)] int range1To100,
			[Range(200, 300)] int range200To300,
			[StringLength(50, MinimumLength = 5)] string stringWithLength,
			[MaxLength(5), MinLength(1)] object[] objArray,
			[FromBody] ExamplePostResource examplePostResource,
			[FromHeader(Name = "Accept-Language")] string acceptLanguage,
			[TestValigator] int testValigatorAttribute,
			[FromRoute, GuidIdentifierValidation, ModelBinder(BinderType = typeof(GuidIdentifierModelBinding))] GuidIdentifier testComplexObjectInRoute,
			[FromHeader] ObjectWithOneProperty fromHeaderNoRemoval,
			CancellationToken cancellationToken)
			=> throw new NotImplementedException();

		[HttpGet("anonymous")]
		public object AnonymousOutput()
			=> new { InnerObject = ((Data<int>)Data.Required<int>().GreaterThan(1)).WithValue(2), OtherValue = 2 }.ToValigatorModel();

	}
}
