using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Dissonance.Engine.Graphics;
using Dissonance.Engine.Utilities;

#pragma warning disable CS0649

namespace Dissonance.Engine.IO
{
	[AutoloadRequirement(typeof(ShaderManager))]
	public class MaterialManager : AssetManager<Material>
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

		public override string[] Extensions => new[] { ".material" };
		public override bool Autoload(string file) => !Game.Instance.NoGraphics;

		public override Material Import(Stream stream, string filePath)
		{
			string jsonText;

			using(var reader = new StreamReader(stream)) {
				jsonText = reader.ReadToEnd();
			}

			var jsonMat = JsonConvert.DeserializeObject<JSON_Material>(jsonText);

			jsonMat.name = FilterText(jsonMat.name, filePath);
			jsonMat.shader = FilterText(jsonMat.shader, filePath);

			var shader = Resources.Find<Shader>(jsonMat.shader);

			if(shader == null) {
				throw new Exception($"Shader {jsonMat.shader} couldn't be found.");
			}

			var material = new Material(jsonMat.name, shader);

			if(jsonMat.textures != null) {
				foreach(var pair in jsonMat.textures) {
					material.SetTexture(FilterText(pair.Key, filePath), Resources.Import<Texture>(FilterText(pair.Value, filePath)));
				}
			}

			if(jsonMat.floats != null) {
				foreach(var pair in jsonMat.floats) {
					material.SetFloat(FilterText(pair.Key, filePath), pair.Value);
				}
			}

			if(jsonMat.vectors != null) {
				foreach(var pair in jsonMat.vectors) {
					material.SetVector(FilterText(pair.Key, filePath), pair.Value);
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
