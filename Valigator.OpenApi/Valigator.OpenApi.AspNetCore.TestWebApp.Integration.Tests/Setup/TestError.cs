using Functional;
using System.Collections.Generic;
using System.Linq;
using Valigator;

namespace Validator.OpenApi.Integration.Tests.Setup
{
	public class TestError
	{
		/// <summary>
		/// Message for this error.
		/// This property is not part of the contract for Error
		/// </summary>
		public Data<string> Message { get; private set; } = Data.Required<string>().Length(1);

		/// <summary>
		/// Error information to display.
		/// This is the only property on Error that is part of the contract for an error.
		/// </summary>
		public Data<TestErrorInformation[]> ErrorInformation { get; private set; } = Data.Collection<TestErrorInformation>().Required();

		/// <summary>
		/// Inner error for nesting
		/// This property is not part of the contract for Error
		/// </summary>
		public Data<Option<object>> InnerError { get; private set; } = Data.Required<object>().Nullable();

		/// <summary>
		/// Stack Trace of this error
		/// This property is not part of the contract for Error
		/// </summary>
		public Data<Option<string>> StackTrace { get; private set; } = Data.Required<string>().Nullable();

		private TestError() { }

		/// <inheritdoc />
		public TestError(string message, string stackTrace, IEnumerable<TestErrorInformation> errorInformation, object innerError)
		{
			ErrorInformation = ErrorInformation.WithValue(errorInformation.ToArray());
			StackTrace = StackTrace.WithValue(stackTrace);
			Message = Message.WithValue(message);
			InnerError = InnerError.WithValue(innerError);
		}
	}
}
