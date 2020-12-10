using Functional;
using Valigator.Core;
using Valigator.Core.StateDescriptors;

namespace Valigator.OpenApi.Core.Extensions
{
	internal static class DataDescriptorExtensions
	{
		public static Option<DataDescriptor> GetItemDataDescriptor(this DataDescriptor dataDescriptor)
			=> Option.Create(dataDescriptor.StateDescriptor is CollectionStateDescriptor, () => (dataDescriptor.StateDescriptor as CollectionStateDescriptor).ItemDescriptor);
	}
}
