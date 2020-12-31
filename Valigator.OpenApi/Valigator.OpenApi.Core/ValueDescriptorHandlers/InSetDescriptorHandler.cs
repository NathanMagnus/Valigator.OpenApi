﻿using Functional;
using NJsonSchema;
using System;
using System.Linq;
using Valigator.Core;
using Valigator.Core.ValueDescriptors;
using Valigator.OpenApi.Core.Extensions;

namespace Valigator.OpenApi.Core.ValueDescriptorHandlers
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
				.Do(d => resourceSchemaProperty.RemoveAndAddExtensionData(InvertedExtensionKey, ExtensionKey, $"{{{string.Join(", ", d.Options.ToArray())}}}"));

		public void Invert(IValueDescriptor valueDescriptor, Type valueType, JsonSchema resourceSchemaProperty)
			=> valueDescriptor
				.As<InSetDescriptor>()
				.Do(d => resourceSchemaProperty.RemoveAndAddExtensionData(ExtensionKey, InvertedExtensionKey, $"{{{string.Join(", ", d.Options.ToArray())}}}"));
	}
}
