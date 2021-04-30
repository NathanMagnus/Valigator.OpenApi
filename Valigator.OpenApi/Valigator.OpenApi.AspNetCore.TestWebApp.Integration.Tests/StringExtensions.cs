namespace Validator.OpenApi.Integration.Tests
{
	public static class StringExtensions
	{
		public static string ToCamelCase(this string str)
			=> str.Substring(0, 1).ToLower() + str.Substring(1);
	}
}
