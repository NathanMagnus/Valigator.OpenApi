using Functional;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Validator.OpenApi.Integration.Tests.TestResources;
using System.Threading.Tasks;

namespace Validator.OpenApi.Integration.Tests.Setup
{
	internal class ResultUnwrappingFilter : IAsyncResultFilter
	{
		private readonly IWebHostEnvironment _hostingEnvironment;
		private readonly ILogger _log;

		public ResultUnwrappingFilter(ILoggerFactory loggerFactory, IWebHostEnvironment hostingEnvironment)
		{
			_hostingEnvironment = hostingEnvironment;
			_log = loggerFactory.CreateLogger<ResultUnwrappingFilter>();
		}

		public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
		{
			var result = context.Result as ObjectResult;
			var type = result?.Value?.GetType();
			if (type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Result<,>) && type.GenericTypeArguments[1] == typeof(TestError))
				context.Result = GetNewResultValue((dynamic)result.Value);

			await next.Invoke();
		}

		private object GetNewResultValue<TSuccess>(Result<TSuccess, TestError> result)
			=> result
				.Match
				(
					success => new ObjectResult(success) { StatusCode = 200 },
					failure =>
					{
						return new ObjectResult(new UnexpectedError(6)) { StatusCode = 505 };
					});
	}
}
