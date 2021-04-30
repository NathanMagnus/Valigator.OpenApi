using NSwag.Generation;
using NSwag.Generation.AspNetCore;
using System;
using System.Collections.Generic;
using Valigator.Core;

namespace Validator.OpenApi.Integration.Tests.Setup
{
	public static class AspNetCoreOpenApiDocumentGeneratorSettingsExtensions
	{
		public static void AddExtraResponses(this AspNetCoreOpenApiDocumentGeneratorSettings openApiSettings)
			=> openApiSettings.OperationProcessors.Add(new DefaultOpenApiOperationProcessor());
	}
}
