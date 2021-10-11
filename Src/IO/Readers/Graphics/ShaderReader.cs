using System.Collections.Generic;
using System.IO;
using Dissonance.Engine.Graphics;
using Newtonsoft.Json;

namespace Dissonance.Engine.IO
{
	public partial class ShaderReader : IAssetReader<Asset<Shader>[]>
	{
		public string[] Extensions { get; } = { ".program" };

		public bool AutoloadAssets => true;

		public Asset<Shader>[] ReadFromStream(Stream stream, string filePath)
		{
			using var reader = new StreamReader(stream);
			
			string jsonText = reader.ReadToEnd();
			var shaders = new List<Asset<Shader>>();
			var jsonShaders = JsonConvert.DeserializeObject<Dictionary<string, JsonShaderProgram>>(jsonText);

			foreach (var pair in jsonShaders) {
				string name = pair.Key;
				var jsonShader = pair.Value;

				string vertexCode = Resources.Get<string>(jsonShader.vertexShader, AssetRequestMode.ImmediateLoad).Value;
				string fragmentCode = Resources.Get<string>(jsonShader.fragmentShader, AssetRequestMode.ImmediateLoad).Value;
				string geometryCode = !string.IsNullOrWhiteSpace(jsonShader.geometryShader) ? Resources.Get<string>(jsonShader.geometryShader, AssetRequestMode.ImmediateLoad).Value : null;

				var shader = Shader.FromCode(name, vertexCode, fragmentCode, geometryCode, jsonShader.shaderDefines);

				shader.Priority = jsonShader.queue;
				shader.CullMode = jsonShader.cullMode;
				shader.PolygonMode = jsonShader.polygonMode;
				shader.BlendFactorSrc = jsonShader.blendFactorSrc;
				shader.BlendFactorDst = jsonShader.blendFactorDst;

				var shaderAsset = Asset.FromValue(name, shader);

				AssetLookup<Shader>.Register(name, shaderAsset);

				shaders.Add(shaderAsset);
			}

			return shaders.ToArray();
		}
	}
}
