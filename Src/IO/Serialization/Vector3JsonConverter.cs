using System;
using System.IO;
using Newtonsoft.Json;

namespace Dissonance.Engine.IO
{
	internal sealed class Vector3JsonConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
			=> objectType == typeof(Vector3);

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType != JsonToken.StartArray) {
				throw new InvalidDataException("Expected a json array.");
			}

			var result = new Vector3(
				(float)reader.ReadAsDouble(),
				(float)reader.ReadAsDouble(),
				(float)reader.ReadAsDouble()
			);

			if (!reader.Read() || reader.TokenType != JsonToken.EndArray) {
				throw new InvalidDataException("Expected the json array to end after 3 values.");
			}

			return result;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var vector = (Vector3)value;

			writer.WriteStartArray();

			writer.WriteValue(vector.X);
			writer.WriteValue(vector.Y);
			writer.WriteValue(vector.Z);

			writer.WriteEndArray();
		}
	}
}
