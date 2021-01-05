using Functional;
using NJsonSchema;
using System;
using System.Linq;
using Valigator.Core;
using Valigator.Core.ValueDescriptors;
using Valigator.OpenApi.AspNetCore.Extensions;

namespace Valigator.OpenApi.AspNetCore.ValueDescriptorHandlers
{
	internal class InSetDescriptorHandler : IHandleValigatorValueDescriptor
	{
		public const string ExtensionKey = "x-InSet";
		public const string InvertedExtensionKey = "x-NotInSet";

		public bool CanHandleValueDescriptor(IValueDescriptor valueDescriptor)
			=> valueDescriptor is InSetDescriptor;

		public void HandleDescriptor(IValueDescriptor valueDescriptor, Type valueType, JsonSchema resourceSchemaProperty)
			=> valueDescriptor
				.As<InSetDescriptor>()
				.Do(d => resourceSchemaProperty.UpdateExtensionData(InvertedExtensionKey, ExtensionKey, $"{{{string.Join(", ", d.Options.ToArray())}}}"));

		public void Invert(IValueDescriptor valueDescriptor, Type valueType, JsonSchema resourceSchemaProperty)
			=> valueDescriptor
				.As<InSetDescriptor>()
				.Do(d => resourceSchemaProperty.UpdateExtensionData(ExtensionKey, InvertedExtensionKey, $"{{{string.Join(", ", d.Options.ToArray())}}}"));
	}
}
