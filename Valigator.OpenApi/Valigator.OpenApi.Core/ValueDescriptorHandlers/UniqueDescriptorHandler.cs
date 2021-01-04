using Functional;
using NJsonSchema;
using System;
using Valigator.Core;
using Valigator.Core.ValueDescriptors;
using Valigator.OpenApi.Core.Extensions;

namespace Valigator.OpenApi.Core.ValueDescriptorHandlers
{
	internal class UniqueDescriptorHandler : IHandleValigatorValueDescriptor
	{
		public bool CanHandleValueDescriptor(IValueDescriptor valueDescriptor)
			=> valueDescriptor is UniqueDescriptor;

		public void HandleDescriptor(IValueDescriptor valueDescriptor, Type valueType, JsonSchema resourceSchemaProperty)
			=> valueDescriptor
				.As<UniqueDescriptor>()
				.Do(d => resourceSchemaProperty.UniqueItems = true);

		public void Invert(IValueDescriptor valueDescriptor, Type valueType, JsonSchema resourceSchemaProperty)
			=> valueDescriptor
				.As<UniqueDescriptor>()
				.Do(d => resourceSchemaProperty.UniqueItems = false);
	}
}
