using Functional;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;
using Validator.OpenApi.Integration.Tests.TestResources;

namespace Validator.OpenApi.Integration.Tests
{
	public class GuidIdentifierModelBinding : IModelBinder
	{
		public Task BindModelAsync(ModelBindingContext bindingContext)
		{
			var stringValue = bindingContext?.ValueProvider?.GetValue(bindingContext.FieldName).FirstOrDefault();
			if (String.IsNullOrEmpty(stringValue) || !Guid.TryParse(stringValue, out var guid))
				return Task.CompletedTask;

			bindingContext.Result = GuidIdentifier
				.Create(guid)
				.Match(s => ModelBindingResult.Success(s), e => ModelBindingResult.Failed());

			return Task.CompletedTask;
		}
	}

}
