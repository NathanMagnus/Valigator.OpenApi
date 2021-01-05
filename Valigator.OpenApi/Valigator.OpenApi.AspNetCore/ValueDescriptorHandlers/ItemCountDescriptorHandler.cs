using Functional;
using NJsonSchema;
using System;
using Valigator.Core;
using Valigator.Core.ValueDescriptors;
using Valigator.OpenApi.AspNetCore.Extensions;

namespace Valigator.OpenApi.AspNetCore.ValueDescriptorHandlers
{
	internal class ItemCountDescriptorHandler : IHandleValigatorValueDescriptor
	{
		public const string ExtensionKey = "x-Equals";

		public bool CanHandleValueDescriptor(IValueDescriptor valueDescriptor)
			=> valueDescriptor is ItemCountDescriptor;

		public void HandleDescriptor(IValueDescriptor valueDescriptor, Type valueType, JsonSchema resourceSchemaProperty)
			=> valueDescriptor
				.As<ItemCountDescriptor>()
				.Do(d =>
				{
					d.MinimumItems.Do(minimum => resourceSchemaProperty.MinItems = minimum);
					d.MaximumItems.Do(maximum => resourceSchemaProperty.MaxItems = maximum);
				});

		public void Invert(IValueDescriptor valueDescriptor, Type valueType, JsonSchema resourceSchemaProperty)
			=> valueDescriptor
				.As<ItemCountDescriptor>()
				.Do(d =>
				{
					d.MinimumItems.Do(maximum => resourceSchemaProperty.MaxItems = maximum);
					d.MaximumItems.Do(minimum => resourceSchemaProperty.MinItems = minimum);
				});
	}
}
