using Functional;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Valigator.OpenApi.AspNetCore.Extensions
{
	internal static class EnumerableExtensions
	{
		public static T[] Do<T>(this IEnumerable<T> enumerable, Action<T> action)
			=> enumerable.Select(e => { action(e); return e; }).ToArray();
	}
}
