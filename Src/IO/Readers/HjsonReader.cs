using System.IO;
using Hjson;
using Newtonsoft.Json.Linq;

namespace Dissonance.Engine.IO
{
	public class HjsonReader : IAssetReader<JObject>
	{
		public string[] Extensions { get; } = new[] { ".hjson" };

		public JObject ReadFromStream(Stream stream, string assetPath)
		{
			string hjsonText = Assets.Get<string>(assetPath, AssetRequestMode.ImmediateLoad).Value;
			using var hjsonReader = new StringReader(hjsonText);

			string jsonText = HjsonValue.Load(hjsonReader).ToString();
			var jsonObject = JObject.Parse(jsonText);

			return jsonObject;
		}

		/*
		public void WriteToStream(JObject jObject, Stream stream)
		{
			string jsonText = jObject.ToString();
			using var jsonReader = new StringReader(jsonText);
			string hjsonText = JsonValue.Load(jsonReader).ToString(Stringify.Hjson);

			using var writer = new StreamWriter(stream);

			writer.Write(hjsonText);
		}
		*/
	}
}
