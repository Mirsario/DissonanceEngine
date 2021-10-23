using System;
using System.IO;
using Newtonsoft.Json;

namespace Dissonance.Engine.IO
{
	internal sealed class Vector2JsonConverter : JsonConverter
	{
		public override bool CanRead => true;
		public override bool CanWrite => true;

		public override bool CanConvert(Type objectType)
			=> objectType == typeof(Vector2);

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType != JsonToken.StartArray) {
				throw new InvalidDataException("Expected a json array.");
			}

			var result = new Vector2(
				(float)reader.ReadAsDouble(),
				(float)reader.ReadAsDouble()
			);

			if (!reader.Read() || reader.TokenType != JsonToken.EndArray) {
				throw new InvalidDataException("Expected the json array to end after 2 values.");
			}

			return result;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var vector = (Vector2)value;

			writer.WriteStartArray();

			writer.WriteValue(vector.X);
			writer.WriteValue(vector.Y);

			writer.WriteEndArray();
		}
	}
}
