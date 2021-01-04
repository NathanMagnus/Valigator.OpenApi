using System;

namespace Valigator.OpenApi.Core.Exceptions
{
	/// <summary>
	/// The type provided does not have a default constructor (constructor with no parameters) defined.
	/// </summary>
	public class MissingDefaultConstructorException : Exception
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="instanceType">The type that is missing a default constructor.</param>
		public MissingDefaultConstructorException(Type instanceType) : base($"Missing default constructor for '{instanceType.Name}'.")
		{
			InstanceType = instanceType;
		}

		/// <summary>
		/// The type that is missing a default constructor.
		/// </summary>
		public Type InstanceType { get; }
	}
}
