using Valigator;

namespace Validator.OpenApi.Integration.Tests.TestResources
{
	public class ExampleExternalClass
	{
		public Data<ExampleExternalClass> Self_Reference { get; set; } = Data.Required<ExampleExternalClass>();
	}
}
