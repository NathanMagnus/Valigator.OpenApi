using Validator.OpenApi.Integration.Tests.TestResources;
using System;
using Valigator;

namespace Validator.OpenApi.Integration.Tests
{
	public class GuidIdentifierValidationAttribute : ValidateAttribute, IValidateType<GuidIdentifier>
	{
		public Data<GuidIdentifier> GetData() => Data.Required<GuidIdentifier>().MappedFrom<Guid>(g => GuidIdentifier.ValigatorCreate(g));
	}
}
