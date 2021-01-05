using Functional;

namespace Valigator.OpenApi.AspNetCore.Extensions
{
	internal static class ObjectExtensions
	{
		public static Option<T> As<T>(this object item)
			=> Option.Create(item is T, () => (T)item);
	}
}
