using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dissonance.Engine.IO
{
	internal sealed class EntityPrefabJsonConverter : JsonConverter
	{
		private static readonly MethodInfo EntityPrefabSetMethod = typeof(EntityPrefab).GetMethod(nameof(EntityPrefab.Set), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

		public override bool CanWrite => false;

		public override bool CanConvert(Type objectType)
			=> objectType == typeof(EntityPrefab) || objectType == typeof(Entity);

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			bool needsPrefab = objectType == typeof(EntityPrefab);

			if (reader.TokenType == JsonToken.String) {
				string contentName = (string)reader.Value;
				var prefab = Assets.Find<EntityPrefab>(contentName).GetValueImmediately();

				return needsPrefab ? prefab : (Entity)prefab;
			}

			if (reader.TokenType == JsonToken.StartObject) {
				var jObject = JObject.Load(reader);
				var entity = WorldManager.PrefabWorld.CreateEntity();
				var prefab = new EntityPrefab(entity.Id);

				ParseComponents(prefab, jObject);

				return needsPrefab ? prefab : entity;
			}

			throw new InvalidOperationException($"Expected a JSON string or object, but got '{reader.TokenType}' instead.");
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
			=> throw new NotImplementedException();

		private static void ParseComponents(EntityPrefab prefab, JObject jObject)
		{
			object[] parameterArray = new object[1];

			foreach (var pair in jObject) {
				var jsonElement = pair.Value;
				var componentType = ComponentManager.GetComponentTypeFromName(pair.Key);

				parameterArray[0] = jsonElement.ToObject(componentType);

				//TODO: Optimize.
				EntityPrefabSetMethod
					.MakeGenericMethod(componentType)
					.Invoke(prefab, parameterArray);
			}
		}
	}
}
