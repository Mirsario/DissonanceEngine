using System;
using System.IO;
using Newtonsoft.Json;

namespace Dissonance.Engine.IO
{
	internal sealed class Vector4JsonConverter : JsonConverter
	{
		public override bool CanRead => true;
		public override bool CanWrite => true;

		public override bool CanConvert(Type objectType)
			=> objectType == typeof(Vector4);

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType != JsonToken.StartArray) {
				throw new InvalidDataException("Expected a json array.");
			}

			var result = new Vector4(
				(float)reader.ReadAsDouble(),
				(float)reader.ReadAsDouble(),
				(float)reader.ReadAsDouble(),
				(float)reader.ReadAsDouble()
			);

			if (!reader.Read() || reader.TokenType != JsonToken.EndArray) {
				throw new InvalidDataException("Expected the json array to end after 4 values.");
			}

			return result;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var vector = (Vector4)value;

			writer.WriteStartArray();

			writer.WriteValue(vector.X);
			writer.WriteValue(vector.Y);
			writer.WriteValue(vector.Z);
			writer.WriteValue(vector.W);

			writer.WriteEndArray();
		}
	}
}
