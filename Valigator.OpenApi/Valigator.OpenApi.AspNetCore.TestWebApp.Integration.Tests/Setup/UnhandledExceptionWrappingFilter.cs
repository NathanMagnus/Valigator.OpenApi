using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Validator.OpenApi.Integration.Tests.TestResources;
using System.Net;

namespace Validator.OpenApi.Integration.Tests
{
	internal class UnhandledExceptionWrappingFilter : ExceptionFilterAttribute
	{
		private readonly ILogger _log;

		public UnhandledExceptionWrappingFilter(ILoggerFactory loggerFactory)
		{
			_log = loggerFactory.CreateLogger<UnhandledExceptionWrappingFilter>();
		}

		public override void OnException(ExceptionContext context)
		{
			_log.LogError(context.Exception, context.Exception.Message);
			var error = new UnexpectedError(3);
			context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
			context.Result = new ObjectResult(error);
		}
	}
}
