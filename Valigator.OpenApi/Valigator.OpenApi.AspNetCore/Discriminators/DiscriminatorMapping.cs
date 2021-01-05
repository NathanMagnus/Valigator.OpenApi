using System;

namespace Valigator.OpenApi.AspNetCore.Discriminators
{
	/// <summary>
	/// Discriminator mapping definition.
	/// </summary>
	/// <param name="PropertyName">The name of the property that is used to distinguish between the discriminated objects.</param>
	/// <param name="Mappings">The enum value to type that defines the descriminator.</param>
	public record DiscriminatorMapping(string PropertyName, params (Enum Enum, Type Type)[] Mappings);
}