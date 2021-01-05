using System;

namespace Valigator.OpenApi.AspNetCore.Exceptions
{
	/// <summary>
	/// An invalid type was used. Message contains the reason why the type was invalid.
	/// </summary>
	public class InvalidTypeException : Exception
	{
		public InvalidTypeException(Type invalidType, string message) : base(message)
			=> InvalidType = invalidType;

		/// <summary>
		/// The type that was invalid.
		/// </summary>
		public Type InvalidType { get; }
	}
}
