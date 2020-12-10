namespace Valigator.OpenApi.Core.Setup
{
	public abstract class OpenApiValidateModelBinderAttribute : ValidateModelBinderAttribute
	{
		public abstract object GetData();
	}
}