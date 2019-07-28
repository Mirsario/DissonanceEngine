using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using GameEngine.Graphics;

namespace GameEngine
{
	public class ShaderManager : AssetManager<Shader[]>
	{
		[JsonObject]
		private class JSON_ShaderProgram
		{
			#pragma warning disable 649
			//Shaders
			[JsonProperty(Required = Required.Always)] public string vertexShader;
			[JsonProperty(Required = Required.Always)] public string fragmentShader;
			public string geometryShader;
			public string[] shaderDefines;

			//Parameters
			public string cullMode;
			public string polygonMode;
			//public string renderType;
			public BlendingFactor blendFactorSrc = BlendingFactor.One;
			public BlendingFactor blendFactorDst = BlendingFactor.Zero;

			//Uniforms
			public Dictionary<string,float> floats;
			public Dictionary<string,float[]> vectors;
			public Dictionary<string,string> textures;
			#pragma warning restore 649
		}

		public override string[] Extensions => new [] { ".program" };
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
				if(!Enum.TryParse(jsonShader.cullMode,true,out shader.cullMode)) {
					shader.cullMode = CullMode.Front;
				}
				if(!Enum.TryParse(jsonShader.polygonMode,true,out shader.polygonMode)) {
					shader.polygonMode = PolygonMode.Fill;
				}

				shader.blendFactorSrc = jsonShader.blendFactorSrc;
				shader.blendFactorDst = jsonShader.blendFactorDst;

				shaders.Add(shader);
			}
			return shaders.ToArray();
		}
	}
}