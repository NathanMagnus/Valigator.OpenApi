using System;

namespace Valigator.OpenApi.Core.Exceptions
{
	public class MissingDefaultConstructorException : Exception
	{
		public MissingDefaultConstructorException(Type instanceType) : base($"Missing default constructor for '{instanceType.Name}'.")
		{
			InstanceType = instanceType;
		}

		public Type InstanceType { get; }
	}
}
