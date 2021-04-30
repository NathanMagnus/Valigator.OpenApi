using Valigator;
namespace Validator.OpenApi.Integration.Tests
{
	public partial class TestController
	{
		public class TestValigatorAttribute : ValidateAttribute, IValidateType<int>
		{
			public Data<int> GetData() => Data.Required<int>().InRange(null, 0);
		}
	}
}