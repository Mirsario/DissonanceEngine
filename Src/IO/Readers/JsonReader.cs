using System.IO;
using Newtonsoft.Json.Linq;

namespace Dissonance.Engine.IO
{
	public class JsonReader : IAssetReader<JObject>
	{
		public string[] Extensions { get; } = new[] { ".json" };

		public JObject ReadFromStream(Stream stream, string assetPath)
		{
			string text = Assets.Get<string>(assetPath, AssetRequestMode.ImmediateLoad).Value;

			return JObject.Parse(text);
		}

		/*
		public void WriteToStream(JObject jObject, Stream stream)
		{
			using var writer = new StreamWriter(stream);

			writer.Write(jObject);
		}
		*/
	}
}
