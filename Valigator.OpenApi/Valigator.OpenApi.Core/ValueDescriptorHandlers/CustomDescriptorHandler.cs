using Functional;
using NJsonSchema;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valigator.Core;
using Valigator.Core.ValueDescriptors;
using Valigator.OpenApi.Core.Extensions;

namespace Valigator.OpenApi.Core.ValueDescriptorHandlers
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
				.Do(d => resourceSchemaProperty.RemoveAndAddExtensionData(InvertedExtensionKey, ExtensionKey, d.Description));

		public void Invert(IValueDescriptor valueDescriptor, Type valueType, JsonSchema resourceSchemaProperty)
			=> valueDescriptor
				.As<CustomDescriptor>()
				.Do(d => resourceSchemaProperty.RemoveAndAddExtensionData(ExtensionKey, InvertedExtensionKey, d.Description));
	}
}
