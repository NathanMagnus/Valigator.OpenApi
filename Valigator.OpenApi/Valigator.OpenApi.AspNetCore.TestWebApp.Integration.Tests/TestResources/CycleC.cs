using Valigator;

namespace Validator.OpenApi.Integration.Tests.TestResources
{
	[ValigatorModel]
	public class CycleC
	{
		public Data<CycleA> A { get; set; } = Data.Required<CycleA>();
	}
}
