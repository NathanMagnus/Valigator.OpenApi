﻿using Functional;
using NJsonSchema;
using System;
using Valigator.Core;
using Valigator.Core.ValueDescriptors;
using Valigator.OpenApi.Core.Extensions;

namespace Valigator.OpenApi.Core.ValueDescriptorHandlers
{
	internal class NotNullDescriptorHandler : IHandleValigatorValueDescriptor
	{
		public bool CanHandleValueDescriptor(IValueDescriptor valueDescriptor)
			=> valueDescriptor is NotNullDescriptor;

		public void HandleDescriptor(IValueDescriptor valueDescriptor, Type valueType, JsonSchema resourceSchemaProperty)
			=> valueDescriptor
				.As<NotNullDescriptor>()
				.Do(d => resourceSchemaProperty.IsNullableRaw = false);

		public void Invert(IValueDescriptor valueDescriptor, Type valueType, JsonSchema resourceSchemaProperty)
			=> valueDescriptor
				.As<NotNullDescriptor>()
				.Do(d => resourceSchemaProperty.IsNullableRaw = true);
	}
}
