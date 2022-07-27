using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Dissonance.Engine.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#pragma warning disable CS0649 // Field is never assigned to

namespace Dissonance.Engine.IO;

[AutoloadRequirement(typeof(ShaderReader))]
public class MaterialReader : IAssetReader<Material>
{
	[JsonObject]
	private class JsonMaterial
	{
		public Dictionary<string, float> Floats;
		public Dictionary<string, float[]> Vectors;
		public Dictionary<string, string> Textures;

		[JsonProperty(Required = Required.Always)]
		public string Name;

		[JsonProperty(Required = Required.Always)]
		public string Shader;
	}

	public string[] Extensions { get; } = { ".material" };

	public async ValueTask<Material> ReadAsset(AssetFileEntry assetFile, MainThreadCreationContext switchToMainThread)
	{
		string assetPath = assetFile.Path;
		string directory = Assets.FilterPath(Path.GetDirectoryName(assetPath));

		var jsonMat = Assets.Get<JObject>(assetPath, AssetRequestMode.ImmediateLoad).Value.ToObject<JsonMaterial>();

		string materialName = FilterText(jsonMat.Name, assetPath);
		string materialShaderName = FilterText(jsonMat.Shader, assetPath);

		var shader = Assets.Find<Shader>(materialShaderName);

		if (shader == null) {
			throw new Exception($"Shader {jsonMat.Shader} couldn't be found.");
		}

		var material = new Material(materialName, shader);

		if (jsonMat.Textures != null) {
			foreach (var pair in jsonMat.Textures) {
				material.SetTexture(pair.Key, Assets.Get<Texture>(pair.Value, directory));
			}
		}

		if (jsonMat.Floats != null) {
			foreach (var pair in jsonMat.Floats) {
				material.SetFloat(pair.Key, pair.Value);
			}
		}

		if (jsonMat.Vectors != null) {
			foreach (var pair in jsonMat.Vectors) {
				material.SetVector(pair.Key, pair.Value);
			}
		}

		return material;
	}

	private static string FilterText(string str, string file)
	{
		str = str.Replace("$File$", Path.GetFileName(file));
		str = str.Replace("$FileName$", Path.GetFileNameWithoutExtension(file));

		return str;
	}
}
