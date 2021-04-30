

using Functional;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Validator.OpenApi.Integration.Tests.Setup;
using Validator.OpenApi.Integration.Tests.TestResources;
using System.Threading.Tasks;
using Valigator;

namespace Validator.OpenApi.Integration.Tests
{
	internal class ValidateModelStateFilter : IAsyncResultFilter
	{
		public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
		{
			if (context.Result is ObjectResult objectResult && objectResult.Value != null)
			{
				var value = objectResult.Value;
				// Perform validation
				await Model
					.Verify(value)
					.DoAsync(async _ => await next.Invoke(), errors =>
					{
						context.Result = new BadRequestObjectResult(Result.Failure<Unit, TestError>(new UnexpectedError(2)));
						return Task.CompletedTask;
					});

			}
			else
				await next.Invoke();
		}
	}
}
