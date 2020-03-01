using Dissonance.Engine.Graphics;
using Dissonance.Engine.Utils.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Dissonance.Engine.IO;

namespace Dissonance.Engine.IO.Graphics
{
	public partial class ShaderManager : AssetManager<Shader[]>
	{
		public override string[] Extensions => new[] { ".program" };
		public override bool Autoload(string file) => true;

		public override Shader[] Import(Stream stream,string filePath)
		{
			string jsonText;

			using(var reader = new StreamReader(stream)) {
				jsonText = reader.ReadToEnd();
			}

			var shaders = new List<Shader>();
			var jsonShaders = JsonConvert.DeserializeObject<Dictionary<string,JsonShaderProgram>>(jsonText);

			foreach(var pair in jsonShaders) {
				string name = pair.Key;
				var jsonShader = pair.Value;

				string vertexCode = Resources.ImportText(jsonShader.vertexShader);
				string fragmentCode = Resources.ImportText(jsonShader.fragmentShader);
				string geometryCode = jsonShader.geometryShader.IsEmptyOrNull() ? "" : Resources.ImportText(jsonShader.geometryShader);

				var shader = Shader.FromCode(name,vertexCode,fragmentCode,geometryCode,jsonShader.shaderDefines);

				shader.queue = jsonShader.queue;
				shader.cullMode = jsonShader.cullMode;
				shader.polygonMode = jsonShader.polygonMode;
				shader.blendFactorSrc = jsonShader.blendFactorSrc;
				shader.blendFactorDst = jsonShader.blendFactorDst;

				shaders.Add(shader);
			}

			return shaders.ToArray();
		}
	}
}