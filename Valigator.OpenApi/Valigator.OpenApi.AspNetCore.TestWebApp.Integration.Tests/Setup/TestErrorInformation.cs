using Valigator;

namespace Validator.OpenApi.Integration.Tests.Setup
{
	public class TestErrorInformation
	{
		private TestErrorInformation() { }

		/// <summary>
		/// Constructor
		/// </summary>
		public TestErrorInformation(string type, object information)
		{
			Type = Type.WithValue(type);
			Information = Information.WithValue(information);
		}

		/// <summary>
		/// The name of the error code
		/// </summary>
		public Data<string> Type { get; private set; } = Data.Required<string>().Length(minimumLength: 1);

		/// <summary>
		/// The error value
		/// </summary>
		public Data<object> Information { get; private set; } = Data.Required<object>();
	}
}
