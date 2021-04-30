using System;
using Validator.OpenApi.Integration.Tests.TestResources;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Validator.OpenApi.Integration.Tests.Setup
{
	public class CartIdentifierConverter : JsonConverter
	{
		public static readonly CartIdentifierConverter Instance = new CartIdentifierConverter();

		public override bool CanRead { get; } = true;
		public override bool CanWrite { get; } = true;

		public override bool CanConvert(Type objectType)
			=> objectType == typeof(GuidIdentifier);

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
			=> GuidIdentifier
				.Create(Guid.Parse((string)reader.Value))
				.Match(_ => _, e => throw new Exception(e));

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
			=> JToken.FromObject(((GuidIdentifier)value).Value).WriteTo(writer);
	}
}
