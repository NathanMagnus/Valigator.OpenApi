using Functional;
using NJsonSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using Valigator.Core;
using Valigator.Core.ValueDescriptors;
using Valigator.OpenApi.AspNetCore.Extensions;

namespace Valigator.OpenApi.AspNetCore.ValueDescriptorHandlers
{
	internal class InvertValueDescriptorHandler : IHandleValigatorValueDescriptor
	{
		public const string ExtensionKey = "x-Equals";
		private readonly IEnumerable<IHandleValigatorValueDescriptor> _valueDescriptorHandlers;

		public InvertValueDescriptorHandler(IEnumerable<IHandleValigatorValueDescriptor> valueDescriptorHandlers)
		{
			_valueDescriptorHandlers = valueDescriptorHandlers;
		}

		public bool CanHandleValueDescriptor(IValueDescriptor valueDescriptor)
			=> valueDescriptor is InvertValueDescriptor;

		public void HandleDescriptor(IValueDescriptor valueDescriptor, Type valueType, JsonSchema resourceSchemaProperty)
			=> valueDescriptor
				.As<InvertValueDescriptor>()
				.Do(d =>
				{
					_valueDescriptorHandlers
						.Where(h => h.CanHandleValueDescriptor(d.InvertedDescriptor))
						.Do(handler => handler.Invert(d.InvertedDescriptor, valueType, resourceSchemaProperty));
				});

		public void Invert(IValueDescriptor valueDescriptor, Type valueType, JsonSchema resourceSchemaProperty)
			=> valueDescriptor
				.As<InvertValueDescriptor>()
				.Do(d =>
				{
					_valueDescriptorHandlers
						.Where(h => h.CanHandleValueDescriptor(d.InvertedDescriptor))
						.Do(handler => handler.HandleDescriptor(d.InvertedDescriptor, valueType, resourceSchemaProperty));
				});
	}
}
