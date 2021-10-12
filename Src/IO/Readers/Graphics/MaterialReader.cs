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
			string directory = Assets.FilterPath(Path.GetDirectoryName(assetPath));

			var jsonMat = new HjsonReader().ReadFromStream(stream, assetPath).ToObject<JSON_Material>();

			string materialName = FilterText(jsonMat.name, assetPath);
			string materialShaderName = FilterText(jsonMat.shader, assetPath);

			var shader = Assets.Find<Shader>(materialShaderName);

			if (shader == null) {
				throw new Exception($"Shader {jsonMat.shader} couldn't be found.");
			}

			var material = new Material(materialName, shader);

			if (jsonMat.textures != null) {
				foreach (var pair in jsonMat.textures) {
					material.SetTexture(pair.Key, Assets.Get<Texture>(pair.Value, directory));
				}
			}

			if (jsonMat.floats != null) {
				foreach (var pair in jsonMat.floats) {
					material.SetFloat(pair.Key, pair.Value);
				}
			}

			if (jsonMat.vectors != null) {
				foreach (var pair in jsonMat.vectors) {
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
}
