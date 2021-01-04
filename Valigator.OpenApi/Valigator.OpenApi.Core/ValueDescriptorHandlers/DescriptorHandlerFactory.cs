using System.Collections.Generic;

namespace Valigator.OpenApi.Core.ValueDescriptorHandlers
{
	internal static class DescriptorHandlerFactory
	{
		public static IEnumerable<IHandleValigatorValueDescriptor> CreateDescriptorHandlers()
		{
			var returnList = new List<IHandleValigatorValueDescriptor>();

			var nonInvertHandlers = CreateNonInvertHandlers();
			returnList.AddRange(nonInvertHandlers);
			returnList.Add(new InvertValueDescriptorHandler(nonInvertHandlers));

			return returnList;
		}

		private static IEnumerable<IHandleValigatorValueDescriptor> CreateNonInvertHandlers()
		{
			yield return new CustomDescriptorHandler();
			yield return new EqualsDescriptorHandler();
			yield return new InSetDescriptorHandler();
			yield return new ItemCountDescriptorHandler();
			yield return new PrecisionDescriptorHandler();
			yield return new RangeDescriptorHandler();
			yield return new StringLengthDescriptorHandler();
			yield return new UniqueDescriptorHandler();
			yield return new NotNullDescriptorHandler();
			yield return new RequiredDescriptorHandler();
			yield return new MultipleOfDescriptorHandler();
			yield return new MappingDescriptorHandler();
		}
	}
}
