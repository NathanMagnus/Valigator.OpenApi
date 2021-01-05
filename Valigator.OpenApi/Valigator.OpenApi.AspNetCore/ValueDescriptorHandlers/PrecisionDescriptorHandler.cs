using Functional;
using NJsonSchema;
using System;
using Valigator.Core;
using Valigator.Core.ValueDescriptors;
using Valigator.OpenApi.AspNetCore.Extensions;

namespace Valigator.OpenApi.AspNetCore.ValueDescriptorHandlers
{
	internal class PrecisionDescriptorHandler : IHandleValigatorValueDescriptor
	{
		public const string ExtensionKey = "x-Precision";

		public bool CanHandleValueDescriptor(IValueDescriptor valueDescriptor)
			=> valueDescriptor is PrecisionDescriptor;

		public void HandleDescriptor(IValueDescriptor valueDescriptor, Type valueType, JsonSchema resourceSchemaProperty)
			=> valueDescriptor
				.As<PrecisionDescriptor>()
				.Do(d => resourceSchemaProperty.AddExtensionData(ExtensionKey, $"Precision >= {d.MinimumDecimalPlaces.Match(min => min, () => 0)}; Precision <= {d.MaximumDecimalPlaces.Match(max => max, () => int.MaxValue)}"));

		public void Invert(IValueDescriptor valueDescriptor, Type valueType, JsonSchema resourceSchemaProperty)
			=> valueDescriptor
				.As<PrecisionDescriptor>()
				.Do(d => resourceSchemaProperty.AddExtensionData(ExtensionKey, $"Precision < {d.MinimumDecimalPlaces.Match(min => min, () => 0)}; Precision > {d.MaximumDecimalPlaces.Match(max => max, () => int.MinValue)}"));
	}
}
