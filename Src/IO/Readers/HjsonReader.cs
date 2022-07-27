using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Hjson;
using Newtonsoft.Json.Linq;

namespace Dissonance.Engine.IO;

public class HjsonReader : IAssetReader<JObject>, IAssetReader<JsonDocument>
{
	public string[] Extensions { get; } = new[] { "*", ".hjson" };

	// Newtonsoft.Json
	async ValueTask<JObject> IAssetReader<JObject>.ReadAsset(AssetFileEntry assetFile, MainThreadCreationContext switchToMainThread)
	{
		string assetPath = assetFile.Path;
		string hjsonText = Assets.Get<string>(assetPath, AssetRequestMode.ImmediateLoad).Value;
		using var hjsonReader = new StringReader(hjsonText);

		string jsonText = HjsonValue.Load(hjsonReader).ToString();
		var jsonObject = JObject.Parse(jsonText);

		return jsonObject;
	}

	// System.Text.Json
	async ValueTask<JsonDocument> IAssetReader<JsonDocument>.ReadAsset(AssetFileEntry assetFile, MainThreadCreationContext switchToMainThread)
	{
		string assetPath = assetFile.Path;
		string hjsonText = Assets.Get<string>(assetPath, AssetRequestMode.ImmediateLoad).Value;
		using var hjsonReader = new StringReader(hjsonText);

		string jsonText = HjsonValue.Load(hjsonReader).ToString();
		var jsonDocument = JsonDocument.Parse(jsonText);

		return jsonDocument;
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
