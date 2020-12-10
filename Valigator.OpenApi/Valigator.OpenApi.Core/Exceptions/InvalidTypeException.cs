using System;

namespace Valigator.OpenApi.Core.Exceptions
{
	public class InvalidTypeException : Exception
	{
		public InvalidTypeException(Type invalidType, string message) : base(message)
		{
			InvalidType = invalidType;
		}

		public Type InvalidType { get; }
	}
}
