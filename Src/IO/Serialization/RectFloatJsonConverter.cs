using System;
using System.IO;
using Newtonsoft.Json;

namespace Dissonance.Engine.IO;

internal sealed class RectFloatJsonConverter : JsonConverter
{
	public override bool CanConvert(Type objectType)
		=> objectType == typeof(RectFloat);

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		if (reader.TokenType != JsonToken.StartArray) {
			throw new InvalidDataException("Expected a json array.");
		}

		var result = new RectFloat(
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
		var rect = (RectFloat)value;

		writer.WriteStartArray();

		writer.WriteValue(rect.X);
		writer.WriteValue(rect.Y);
		writer.WriteValue(rect.Width);
		writer.WriteValue(rect.Height);

		writer.WriteEndArray();
	}
}
