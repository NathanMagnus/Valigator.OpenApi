using Functional;
using NJsonSchema;
using System;
using Valigator.Core;
using Valigator.Core.ValueDescriptors;
using Valigator.OpenApi.AspNetCore.Extensions;

namespace Valigator.OpenApi.AspNetCore.ValueDescriptorHandlers
{
	internal class RangeDescriptorHandler : IHandleValigatorValueDescriptor
	{
		public const string MinimumExtensionKey = "x-Minimum";
		public const string MaximumExtensionKey = "x-Maximum";

		public bool CanHandleValueDescriptor(IValueDescriptor valueDescriptor)
			=> valueDescriptor is RangeDescriptor;

		public void HandleDescriptor(IValueDescriptor valueDescriptor, Type valueType, JsonSchema resourceSchemaProperty)
			=> valueDescriptor
				.As<RangeDescriptor>()
				.Do(_ => resourceSchemaProperty.RemoveExtensionData(MinimumExtensionKey))
				.Do(_ => resourceSchemaProperty.RemoveExtensionData(MaximumExtensionKey))
				.Do(descriptor => descriptor
					.GreaterThanValue
					.Do(value =>
					{
						if (value.GetType().IsNumericType())
							resourceSchemaProperty.Minimum = Convert.ToDecimal(value);
						else
							resourceSchemaProperty.AddExtensionData(MinimumExtensionKey, value);

						resourceSchemaProperty.IsExclusiveMinimum = !descriptor.GreaterThanOrEqualTo;
					})
				)
				.Do(descriptor => descriptor
					.LessThanValue
					.Do(value =>
					{
						if (value.GetType().IsNumericType())
							resourceSchemaProperty.Maximum = Convert.ToDecimal(value);
						else
							resourceSchemaProperty.AddExtensionData(MaximumExtensionKey, value);

						resourceSchemaProperty.IsExclusiveMaximum = !descriptor.LessThanOrEqualTo;
					})
				);

		public void Invert(IValueDescriptor valueDescriptor, Type valueType, JsonSchema resourceSchemaProperty)
			=> valueDescriptor
				.As<RangeDescriptor>()
				.Do(_ => resourceSchemaProperty.RemoveExtensionData(MinimumExtensionKey))
				.Do(_ => resourceSchemaProperty.RemoveExtensionData(MaximumExtensionKey))
				.Do(descriptor => descriptor
					.GreaterThanValue
					.Do(value =>
					{
						if (value.GetType().IsNumericType())
							resourceSchemaProperty.Maximum = Convert.ToDecimal(value);
						else
							resourceSchemaProperty.AddExtensionData(MaximumExtensionKey, value);

						resourceSchemaProperty.IsExclusiveMaximum = descriptor.GreaterThanOrEqualTo;
					})
				)
				.Do(descriptor => descriptor
					.LessThanValue
					.Do(value =>
					{
						if (value.GetType().IsNumericType())
							resourceSchemaProperty.Minimum = Convert.ToDecimal(value);
						else
							resourceSchemaProperty.AddExtensionData(MinimumExtensionKey, value);

						resourceSchemaProperty.IsExclusiveMinimum = descriptor.LessThanOrEqualTo;
					})
				);
	}
}
