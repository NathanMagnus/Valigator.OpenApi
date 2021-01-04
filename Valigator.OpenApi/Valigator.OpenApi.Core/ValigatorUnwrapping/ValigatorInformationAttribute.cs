using System;

namespace Valigator.OpenApi.Core.ValigatorUnwrapping
{
	internal class ValigatorInformationAttribute : Attribute
	{
		public ValigatorInformationAttribute(Type parentType, string propertyName)
		{
			ParentType = parentType;
			PropertyName = propertyName;
		}

		public Type ParentType { get; }
		public string PropertyName { get; }
	}
}
