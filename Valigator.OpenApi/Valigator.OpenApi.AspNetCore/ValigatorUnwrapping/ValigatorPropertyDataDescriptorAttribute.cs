using System;
using Valigator.Core;

namespace Valigator.OpenApi.AspNetCore.ValigatorUnwrapping
{
	internal class ValigatorPropertyDataDescriptorAttribute : Attribute
	{
		public DataDescriptor DataDescriptor { get; }

		public ValigatorPropertyDataDescriptorAttribute(DataDescriptor dataDescriptor)
		{
			DataDescriptor = dataDescriptor;
		}
	}
}
