using Valigator;

namespace Validator.OpenApi.Integration.Tests.TestResources
{
	[ValigatorModel]
	public class CycleB
	{
		public Data<CycleC> C { get; set; } = Data.Required<CycleC>();
	}
}
