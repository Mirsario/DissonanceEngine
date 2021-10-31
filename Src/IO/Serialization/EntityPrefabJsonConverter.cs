using System;
using Newtonsoft.Json;

namespace Dissonance.Engine.IO
{
	internal sealed class EntityPrefabJsonConverter : JsonConverter
	{
		public override bool CanWrite => false;

		public override bool CanConvert(Type objectType)
			=> objectType == typeof(EntityPrefab);

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			string contentName = (string)reader.Value;

			return GameContent.Find<EntityPrefab>(contentName);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
			=> throw new NotImplementedException();
	}
}
