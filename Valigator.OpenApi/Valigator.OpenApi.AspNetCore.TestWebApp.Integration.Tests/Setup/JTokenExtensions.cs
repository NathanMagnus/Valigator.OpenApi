using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Validator.OpenApi.Integration.Tests.Setup
{
	public static class JTokenExtensions
	{
		public static void ValidateValue<T>(this JToken token, T value)
			=> token.Value<T>().Should().Be(value);

		public static T GetPath<T>(this JToken json, params object[] paths)
			where T : class
			=> json.GetPath(paths) as T;

		public static JToken GetPath(this JToken json, params object[] paths)
		{
			try
			{
				var token = json;
				foreach (var path in paths)
				{
					if (token[path] == null)
						return null;
					token = token[path];
				}

				return token;
			}
			catch
			{
				return null;
			}
		}

		public static void CheckExists(this JToken json, params object[] paths)
			=> json.GetPath(paths);

		internal static void ShouldBeEquivalentTo(this JToken actualValue, object expected)
		{
			var anonymousAsJObject = JsonConvert.DeserializeObject<JToken>(JsonConvert.SerializeObject(expected));

			var differences = GetDifferences(anonymousAsJObject, actualValue).ToArray();

			if (differences.Any())
			{
				var differenceMessages = GetDifferences(anonymousAsJObject, actualValue)
					.Select(difference => Environment.NewLine + GetMessageForDifference(difference.expected, difference.message));

				throw new Exception(String.Join("", differenceMessages));
			}
		}

		private static string GetMessageForDifference(JToken expected, string message)
			=> $"{GetPath(expected)} - {message}.";

		private static string GetPath(JToken startToken)
		{
			var builder = new StringBuilder();

			JToken lastToken = null;
			foreach (var token in GetParents(startToken).Reverse())
			{
				if (token is JProperty property)
					builder.Append($".{property.Name}");
				else if (lastToken is JArray array)
					builder.Append($"[{array.IndexOf(token)}]");

				lastToken = token;
			}

			return builder.ToString().Substring(1);
		}

		private static IEnumerable<JToken> GetParents(JToken token)
		{
			while (token != null)
			{
				yield return token;
				token = token.Parent;
			}
		}

		private static IEnumerable<(JToken expected, string message)> GetDifferences(JToken expected, JToken actual)
		{
			if (expected.GetType() != actual.GetType())
				return new[] { (expected, $"Expected type {expected.GetType()} but received {actual.GetType()}") };

			switch (expected)
			{
				case JProperty expectedProperty:
					if (actual is JProperty actualProperty)
					{
						if (expectedProperty.Name != actualProperty.Name)
						{
							var outOfOrderProperty = actualProperty?.Parent?.Children().FirstOrDefault(p => ((JProperty)p).Name == expectedProperty.Name);
							return outOfOrderProperty != null
								? GetDifferences(expected, outOfOrderProperty)
								: new[] { (expected, $"Expected property `{expectedProperty.Name}` but received `{actualProperty.Name}`") };
						}
						else if (expectedProperty.Value.Type != actualProperty.Value.Type)
							return new[] { (expected, $"Expected type `{expectedProperty.Value.Type}` but received `{actualProperty.Value.Type}`") };

						return GetDifferences(expectedProperty.Value, actualProperty.Value);
					}

					return new[] { (expected, $"Expected a property of type {expectedProperty.GetType()} but received an property of type `{expectedProperty.GetType()}`") };
				case JArray expectedArray:
					if (actual is JArray actualArray)
					{
						if (expectedArray.Count != actualArray.Count)
							return new[] { (expected, $"Expected array of length `{expectedArray.Count}` but received an array of length `{actualArray.Count}`") };

						return expectedArray
								.Children()
								.Zip(actualArray.Children(), (c1, c2) => (c1, c2))
								.SelectMany(set => GetDifferences(set.c1, set.c2));
					}

					return new[] { (expected, $"Expected an array but received an object of type `{actual.GetType()}`") };
				case JValue expectedValue:
					if (actual is JValue actualValue)
					{
						if (!expectedValue.Equals(actualValue))
						{
							var formattedExpectedValue = expectedValue.ToString().KeepNewLineEscapeSequences().PerformSubstitutions();
							var formattedActualValue = actual.ToString().KeepNewLineEscapeSequences().PerformSubstitutions();

							return new[] { (expected, $"Expected value `{formattedExpectedValue}` of length {formattedExpectedValue.Length} but received `{formattedActualValue}` of length {formattedActualValue.Length}") };
						}
					}
					else
						return new[] { (expected, $"Expected an object of type {expectedValue.GetType()} but received an object of type `{actual.GetType()}`") };
					break;
				case JObject expectedObj:
					if (actual is JObject actualObj)
					{
						return MatchPropertiesInJObjects(expectedObj, actualObj)
							.SelectMany(set =>
							{
								if (set.matchingActual != null)
									return GetDifferences(set.expected, set.matchingActual);

								return new[] { ((JToken)set.expected, $"Expected property `{set.expected.Name}` but recieved an object with no such property") };
							});
					}

					return new[] { (expected, $"Expected an object but received `{actual.GetType()}`") };
				default:
					throw new Exception($"Unrecognized JToken type: {expected.GetType()}");
			}

			return Enumerable.Empty<(JToken, string)>();
		}

		private static string KeepNewLineEscapeSequences(this string str)
			=> str.Replace("\r", "\\r").Replace("\n", "\\n");

		private static string PerformSubstitutions(this string str)
			=> str.Replace("x-", "x_").Replace("__", "/").Replace("__z__", "").Replace("__x__", "$");

		private static IEnumerable<(JProperty expected, JProperty matchingActual)> MatchPropertiesInJObjects(JObject expected, JObject actual)
			=>
			from expectedChild in expected.Children<JProperty>()
			join actualChild in actual.Children<JProperty>() on expectedChild.Name equals actualChild.Name into matched
			select (expectedChild, matchingActual: matched.FirstOrDefault());
	}
}
