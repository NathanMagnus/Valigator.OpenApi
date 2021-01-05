using Functional;
using NJsonSchema;
using System;
using Valigator.Core;
using Valigator.Core.ValueDescriptors;
using Valigator.OpenApi.AspNetCore.Extensions;

namespace Valigator.OpenApi.AspNetCore.ValueDescriptorHandlers
{
	internal class StringLengthDescriptorHandler : IHandleValigatorValueDescriptor
	{
		public bool CanHandleValueDescriptor(IValueDescriptor valueDescriptor)
			=> valueDescriptor is StringLengthDescriptor;

		public void HandleDescriptor(IValueDescriptor valueDescriptor, Type valueType, JsonSchema resourceSchemaProperty)
			=> valueDescriptor
				.As<StringLengthDescriptor>()
				.Do(d =>
				{
					resourceSchemaProperty.MinLength = d.MinimumLength.Match(_ => _, () => (int?)null);
					resourceSchemaProperty.MaxLength = d.MaximumLength.Match(_ => _, () => (int?)null);
				});

		public void Invert(IValueDescriptor valueDescriptor, Type valueType, JsonSchema resourceSchemaProperty)
			=> valueDescriptor
				.As<StringLengthDescriptor>()
				.Do(d =>
				{
					resourceSchemaProperty.MinLength = d.MaximumLength.Match(_ => _, () => (int?)null);
					resourceSchemaProperty.MaxLength = d.MinimumLength.Match(_ => _, () => (int?)null);
				});
	}
}
