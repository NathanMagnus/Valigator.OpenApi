using Valigator;

namespace Validator.OpenApi.Integration.Tests.TestResources
{
	[ValigatorModel]
	public class CycleA
	{
		public Data<CycleB[]> B { get; set; } = Data.Required<CycleB[]>();
	}
}
