using Functional;
using System;
using System.Collections.Generic;
using Valigator;
using Valigator.Core.ValueDescriptors;

namespace Validator.OpenApi.Integration.Tests.TestResources
{
	public struct GuidIdentifier : IEquatable<GuidIdentifier>
	{
		private readonly Guid? _value;
		public Guid Value => _value ?? throw new Exception();

		public GuidIdentifier(Guid value)
			=> _value = value;

		public override bool Equals(object obj)
			=> obj is GuidIdentifier CartIdentifier && Equals(CartIdentifier);

		public bool Equals(GuidIdentifier other)
			=> Value == other.Value;

		public override int GetHashCode()
			 => unchecked(1571931217 * -1521134295 + EqualityComparer<Guid>.Default.GetHashCode(Value));


		public static bool operator ==(GuidIdentifier identifier1, GuidIdentifier identifier2)
			=> identifier1.Equals(identifier2);

		public static bool operator !=(GuidIdentifier identifier1, GuidIdentifier identifier2)
			=> !(identifier1 == identifier2);

		public override string ToString()
			=> $"{nameof(GuidIdentifier)}:{Value}";

		public static Result<GuidIdentifier, string> Create(Guid cartIdentifier)
			=> Result
				.Create
				(
					cartIdentifier != Guid.Empty,
					() => new GuidIdentifier(cartIdentifier),
					() => "Value cannot be an empty guid."
				);

		public static Result<GuidIdentifier, ValidationError[]> ValigatorCreate(Guid cartIdentifier)
			=> Result
				.Create
				(
					cartIdentifier != Guid.Empty,
					() => new GuidIdentifier(cartIdentifier),
					() => new[] { new ValidationError($"Value cannot be an empty guid.", new NotNullDescriptor()) }
				);
	}

}
