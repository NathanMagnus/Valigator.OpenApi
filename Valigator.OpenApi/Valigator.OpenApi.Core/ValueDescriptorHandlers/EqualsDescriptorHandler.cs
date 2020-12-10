using Functional;
using NJsonSchema;
using System;
using Valigator.Core;
using Valigator.Core.ValueDescriptors;
using Valigator.OpenApi.Core.Extensions;

namespace Valigator.OpenApi.Core.ValueDescriptorHandlers
{
	internal class EqualsDescriptorHandler : IHandleValigatorValueDescriptor
	{
		public const string ExtensionKey = "x-Equals";
		public const string InvertedExtensionKey = "x-NotEquals";

		public bool CanHandleValueDescriptor(IValueDescriptor valueDescriptor)
			=> valueDescriptor is EqualsDescriptor;

		public void HandleDescriptor(IValueDescriptor valueDescriptor, Type valueType, JsonSchema resourceSchemaProperty)
			=> valueDescriptor
				.As<EqualsDescriptor>()
				.Do(d =>
				{
					resourceSchemaProperty.RemoveExtensionData(InvertedExtensionKey);
					if (valueType.IsNumericType() && decimal.TryParse(d.Value?.ToString(), out var value))
					{
						resourceSchemaProperty.Minimum = value;
						resourceSchemaProperty.Maximum = value;
					}
					else
						resourceSchemaProperty.AddExtensionData(ExtensionKey, d.Value);
				});

		public void Invert(IValueDescriptor valueDescriptor, Type valueType, JsonSchema resourceSchemaProperty)
			=> valueDescriptor
				.As<EqualsDescriptor>()
				.Do(d =>
				{
					resourceSchemaProperty.Minimum = null;
					resourceSchemaProperty.Maximum = null;
					resourceSchemaProperty.RemoveAndAddExtensionData(ExtensionKey, InvertedExtensionKey, d.Value);
				});
	}
}
