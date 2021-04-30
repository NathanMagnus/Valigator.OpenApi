using Validator.OpenApi.Integration.Tests.Setup;
using System;
using Valigator;

namespace Validator.OpenApi.Integration.Tests.TestResources
{
	[ValigatorModel]
	public class UnexpectedError : TestError
	{
		public UnexpectedError(int i) : base(nameof(UnexpectedError), String.Empty, Array.Empty<TestErrorInformation>(), null)
		{
			Int = Int.WithValue(i);
		}

		public Data<int> Int { get; set; }
	}
}
