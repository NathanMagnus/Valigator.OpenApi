using System;

namespace Valigator.OpenApi.Core.Discriminators
{
	public record DiscriminatorMapping(string PropertyName, params (Enum Enum, Type Type)[] Mappings);
}