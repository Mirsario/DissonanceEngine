using Dissonance.Engine.Graphics;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Dissonance.Engine.IO
{
	public partial class ShaderManager : IAssetReader<Shader[]>
	{
		public string[] Extensions { get; } = { ".program" };

		public Shader[] ReadFromStream(Stream stream, string filePath)
		{
			string jsonText;

			using(var reader = new StreamReader(stream)) {
				jsonText = reader.ReadToEnd();
			}

			var shaders = new List<Shader>();
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

				shaders.Add(shader);
			}

			return shaders.ToArray();
		}
	}
}
