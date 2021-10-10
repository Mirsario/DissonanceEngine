using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Dissonance.Engine.Graphics;
using Dissonance.Engine.Utilities;

#pragma warning disable CS0649

namespace Dissonance.Engine.IO
{
	[AutoloadRequirement(typeof(ShaderReader))]
	public class MaterialReader : IAssetReader<Material>
	{
		[JsonObject]
		private class JSON_Material
		{
			public Dictionary<string, float> floats;
			public Dictionary<string, float[]> vectors;
			public Dictionary<string, string> textures;

			[JsonProperty(Required = Required.Always)]
			public string name;

			[JsonProperty(Required = Required.Always)]
			public string shader;
		}

		public string[] Extensions { get; } = { ".material" };

		public Material ReadFromStream(Stream stream, string assetPath)
		{
			string jsonText;

			using(var reader = new StreamReader(stream)) {
				jsonText = reader.ReadToEnd();
			}

			var jsonMat = JsonConvert.DeserializeObject<JSON_Material>(jsonText);

			jsonMat.name = FilterText(jsonMat.name, assetPath);
			jsonMat.shader = FilterText(jsonMat.shader, assetPath);

			var shader = Resources.Get<Shader>(jsonMat.shader);

			if (shader == null) {
				throw new Exception($"Shader {jsonMat.shader} couldn't be found.");
			}

			var material = new Material(jsonMat.name, shader);

			if (jsonMat.textures != null) {
				foreach (var pair in jsonMat.textures) {
					material.SetTexture(FilterText(pair.Key, assetPath), Resources.Get<Texture>(FilterText(pair.Value, assetPath)));
				}
			}

			if (jsonMat.floats != null) {
				foreach (var pair in jsonMat.floats) {
					material.SetFloat(FilterText(pair.Key, assetPath), pair.Value);
				}
			}

			if (jsonMat.vectors != null) {
				foreach (var pair in jsonMat.vectors) {
					material.SetVector(FilterText(pair.Key, assetPath), pair.Value);
				}
			}

			return material;
		}
		private static string FilterText(string str, string file) => str.ReplaceCaseInsensitive(
			("$FILE$", Path.GetFileName(file)),
			("$FILENAME$", Path.GetFileNameWithoutExtension(file))
		);
	}
}
