using Functional;
using NJsonSchema;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valigator.Core;
using Valigator.Core.ValueDescriptors;
using Valigator.OpenApi.AspNetCore.Extensions;

namespace Valigator.OpenApi.AspNetCore.ValueDescriptorHandlers
{

	internal class CustomDescriptorHandler : IHandleValigatorValueDescriptor
	{
		public const string ExtensionKey = "x-CustomValue";
		public const string InvertedExtensionKey = "x-NotCustomValue";

		public bool CanHandleValueDescriptor(IValueDescriptor valueDescriptor)
			=> valueDescriptor is CustomDescriptor;

		public void HandleDescriptor(IValueDescriptor valueDescriptor, Type valueType, JsonSchema resourceSchemaProperty)
			=> valueDescriptor
				.As<CustomDescriptor>()
				.Do(d => resourceSchemaProperty.UpdateExtensionData(InvertedExtensionKey, ExtensionKey, d.Description));

		public void Invert(IValueDescriptor valueDescriptor, Type valueType, JsonSchema resourceSchemaProperty)
			=> valueDescriptor
				.As<CustomDescriptor>()
				.Do(d => resourceSchemaProperty.UpdateExtensionData(ExtensionKey, InvertedExtensionKey, d.Description));
	}
}
