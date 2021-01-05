using System;

namespace Valigator.OpenApi.AspNetCore.ValigatorUnwrapping
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
