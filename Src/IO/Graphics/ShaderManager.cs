using Dissonance.Framework.Graphics;
using Dissonance.Engine.Graphics;
using Dissonance.Engine.Utils.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Dissonance.Engine.IO;

namespace Dissonance.Engine.IO.Graphics
{
	public class ShaderManager : AssetManager<Shader[]>
	{
		[JsonObject]
		private class JSON_ShaderProgram
		{
			//Shaders
			[JsonProperty(Required = Required.Always)] public string vertexShader;
			[JsonProperty(Required = Required.Always)] public string fragmentShader;
			public string geometryShader;
			public string[] shaderDefines;

			//Parameters
			public int queue;
			public CullMode cullMode = CullMode.Front;
			public PolygonMode polygonMode = PolygonMode.Fill;
			public BlendingFactor blendFactorSrc = BlendingFactor.One;
			public BlendingFactor blendFactorDst = BlendingFactor.Zero;

			//Uniforms
			public Dictionary<string,float> floats;
			public Dictionary<string,float[]> vectors;
			public Dictionary<string,string> textures;
		}

		public override string[] Extensions => new[] { ".program" };
		public override bool Autoload(string file) => true;

		public override Shader[] Import(Stream stream,string fileName)
		{
			string jsonText;

			using(var reader = new StreamReader(stream)) {
				jsonText = reader.ReadToEnd();
			}

			var shaders = new List<Shader>();
			var jsonShaders = JsonConvert.DeserializeObject<Dictionary<string,JSON_ShaderProgram>>(jsonText);

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