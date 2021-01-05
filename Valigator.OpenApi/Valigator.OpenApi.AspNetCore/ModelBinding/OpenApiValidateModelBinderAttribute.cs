namespace Valigator.OpenApi.AspNetCore.ModelBinding
{
	/// <summary>
	/// Class used to create a new ModelBinder.
	/// </summary>
	public abstract class OpenApiValidateModelBinderAttribute : ValidateModelBinderAttribute
	{
		/// <summary>
		/// Get the data from the model. This is type Data&lt;T&gt;.
		/// </summary>
		/// <returns>Valigator Data</returns>
		public abstract object GetData();
	}
}