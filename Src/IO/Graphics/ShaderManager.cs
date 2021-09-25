using Dissonance.Engine.Graphics;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Dissonance.Engine.IO
{
	public partial class ShaderManager : AssetManager<Shader[]>
	{
		public override string[] Extensions { get; } = new[] { ".program" };

		public override bool Autoload(string file)
			=> !Game.Instance.NoGraphics;

		public override Shader[] Import(Stream stream, string filePath)
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

				string vertexCode = Resources.ImportText(jsonShader.vertexShader);
				string fragmentCode = Resources.ImportText(jsonShader.fragmentShader);
				string geometryCode = !string.IsNullOrWhiteSpace(jsonShader.geometryShader) ? Resources.ImportText(jsonShader.geometryShader) : null;

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
