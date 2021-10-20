using System;
using System.Reflection;
using Newtonsoft.Json;

namespace Dissonance.Engine.IO
{
	internal sealed class AssetJsonConverter : JsonConverter
	{
		public static string BaseAssetPath { get; set; }

		private static MethodInfo assetsGetMethodA = typeof(Assets).GetMethod(nameof(Assets.Get), new[] { typeof(string), typeof(AssetRequestMode) });
		private static MethodInfo assetsGetMethodB = typeof(Assets).GetMethod(nameof(Assets.Get), new[] { typeof(string), typeof(string), typeof(AssetRequestMode) });

		public override bool CanRead => true;
		public override bool CanWrite => false;

		public override bool CanConvert(Type objectType)
			=> objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Asset<>);

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			string assetPath = (string)reader.Value;
			var assetType = objectType.GetGenericArguments()[0];

			if (BaseAssetPath == null) {
				return assetsGetMethodA.MakeGenericMethod(assetType).Invoke(null, new object[] { assetPath, AssetRequestMode.DoNotLoad });
			} else {
				return assetsGetMethodB.MakeGenericMethod(assetType).Invoke(null, new object[] { assetPath, BaseAssetPath, AssetRequestMode.DoNotLoad });
			}
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
			=> throw new NotImplementedException();
	}
}
