using NJsonSchema;
using System;
using Valigator.Core;
using Valigator.Core.ValueDescriptors;
using Valigator.OpenApi.AspNetCore.Extensions;

namespace Valigator.OpenApi.AspNetCore.ValueDescriptorHandlers
{
	internal class RequiredDescriptorHandler : IHandleValigatorValueDescriptor
	{
		public bool CanHandleValueDescriptor(IValueDescriptor valueDescriptor)
			=> valueDescriptor is RequiredDescriptor;

		public void HandleDescriptor(IValueDescriptor valueDescriptor, Type valueType, JsonSchema resourceSchemaProperty) { }

		public void Invert(IValueDescriptor valueDescriptor, Type valueType, JsonSchema resourceSchemaProperty) { }
	}
}
