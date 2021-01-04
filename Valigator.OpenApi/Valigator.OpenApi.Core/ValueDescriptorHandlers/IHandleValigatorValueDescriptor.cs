using NJsonSchema;
using System;
using Valigator.Core;

namespace Valigator.OpenApi.Core.ValueDescriptorHandlers
{
	internal interface IHandleValigatorValueDescriptor
	{
		bool CanHandleValueDescriptor(IValueDescriptor valueDescriptor);
		void HandleDescriptor(IValueDescriptor valueDescriptor, Type valueType, JsonSchema resourceSchemaProperty);
		void Invert(IValueDescriptor valueDescriptor, Type valueType, JsonSchema resourceSchemaProperty);
	}
}
