using System;
using Newtonsoft.Json;

namespace Dissonance.Engine.IO;

internal sealed class LayerJsonConverter : JsonConverter
{
	public override bool CanWrite => false;

	public override bool CanConvert(Type objectType)
		=> objectType == typeof(Layer);

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		if (reader.TokenType == JsonToken.String) {
			string layerName = (string)reader.Value;

			return Layers.GetLayer(layerName);
		}

		throw new InvalidOperationException($"Expected a JSON string, but got '{reader.TokenType}' instead.");
	}

	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		=> throw new NotImplementedException();
}
