using NSwag;
using NSwag.Generation.Processors.Contexts;
using System;
using System.Collections.Generic;

namespace Valigator.OpenApi.AspNetCore.Authorization
{
	/// <summary>
	/// Authorization information.
	/// </summary>
	public record Authorization(string Name, OpenApiSecurityScheme SecurityScheme, OpenApiSecurityRequirement SecurityRequirement, Func<OperationProcessorContext, IEnumerable<string>> ScopesFunc, Func<EndpointInformation, bool> OperationFilter, params string[] ScopeNames);

}