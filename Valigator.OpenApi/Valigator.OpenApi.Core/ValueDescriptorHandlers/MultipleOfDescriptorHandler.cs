using Functional;
using NJsonSchema;
using System;
using Valigator.Core;
using Valigator.Core.ValueDescriptors;
using Valigator.OpenApi.Core.Extensions;

namespace Valigator.OpenApi.Core.ValueDescriptorHandlers
{
	internal class MultipleOfDescriptorHandler : IHandleValigatorValueDescriptor
	{
		public const string InvertedExtensionKey = "x-NotMulitpleOf";

		public bool CanHandleValueDescriptor(IValueDescriptor valueDescriptor)
			=> valueDescriptor is MultipleOfDescriptor;

		public void HandleDescriptor(IValueDescriptor valueDescriptor, Type valueType, JsonSchema resourceSchemaProperty)
			=> valueDescriptor
				.As<MultipleOfDescriptor>()
				.Do(d =>
				{
					resourceSchemaProperty.RemoveExtensionData(InvertedExtensionKey);
					if (decimal.TryParse(d.Divisor?.ToString(), out var decimalValue))
						resourceSchemaProperty.MultipleOf = decimalValue;
				});

		public void Invert(IValueDescriptor valueDescriptor, Type valueType, JsonSchema resourceSchemaProperty)
			=> valueDescriptor
				.As<MultipleOfDescriptor>()
				.Do(d =>
				{
					resourceSchemaProperty.MultipleOf = null;
					if (decimal.TryParse(d.Divisor?.ToString(), out var decimalValue))
					{
						resourceSchemaProperty.AddExtensionData(InvertedExtensionKey, decimalValue);
					}
				});
	}
}
