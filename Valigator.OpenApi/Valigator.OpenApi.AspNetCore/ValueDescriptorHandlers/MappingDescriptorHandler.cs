using NJsonSchema;
using System;
using Valigator.Core;
using Valigator.Core.ValueDescriptors;

namespace Valigator.OpenApi.AspNetCore.ValueDescriptorHandlers
{
	internal class MappingDescriptorHandler : IHandleValigatorValueDescriptor
	{
		public bool CanHandleValueDescriptor(IValueDescriptor valueDescriptor)
			=> valueDescriptor is MappingDescriptor;

		public void HandleDescriptor(IValueDescriptor valueDescriptor, Type valueType, JsonSchema resourceSchemaProperty) { }

		public void Invert(IValueDescriptor valueDescriptor, Type valueType, JsonSchema resourceSchemaProperty) { }
	}
}
