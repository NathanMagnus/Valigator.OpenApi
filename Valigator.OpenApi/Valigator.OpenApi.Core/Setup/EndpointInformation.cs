namespace Valigator.OpenApi.Core
{
	/// <summary>
	/// Information about an endpoint
	/// </summary>
	public record EndpointInformation(string Path, string OperationId, string HttpMethod, bool HasAdditionalUserScopes)
	{
		/// <summary>
		/// Create an operation ID
		/// </summary>
		/// <param name="controllerName">The name of the controller</param>
		/// <param name="methodName">The name of the method</param>
		/// <returns>Operation ID that can be used when creating EndpointInformation objects</returns>
		public static string CreateOperationId(string controllerName, string methodName)
		{
			var index = controllerName.LastIndexOf("Controller");
			return $"{controllerName.Substring(0, index > -1 ? index : controllerName.Length)}_{methodName}";
		}
	}

}