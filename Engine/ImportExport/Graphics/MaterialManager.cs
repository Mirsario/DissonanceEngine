using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using ImagingPixelFormat = System.Drawing.Imaging.PixelFormat;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;
using Newtonsoft.Json;

namespace GameEngine
{
	[AutoloadRequirement(typeof(ShaderManager))]
	public class MaterialManager : AssetManager<Material>
	{
		[JsonObject]
		private class JSON_Material
		{
			#pragma warning disable 649
			[JsonProperty(Required = Required.Always)] public string name;
			[JsonProperty(Required = Required.Always)] public string shader;
			public Dictionary<string,float> floats;
			public Dictionary<string,float[]> vectors;
			public Dictionary<string,string> textures;
			#pragma warning restore 649
		}

		public override string[] Extensions => new [] { ".material" };
		public override bool Autoload(string file) => true;

		public override Material Import(Stream stream,string fileName)
		{
			string jsonText;
			using(var reader = new StreamReader(stream)) {
				jsonText = reader.ReadToEnd();
			}
			var jsonMat = JsonConvert.DeserializeObject<JSON_Material>(jsonText);
			jsonMat.name = FilterText(jsonMat.name,fileName);
			jsonMat.shader = FilterText(jsonMat.shader,fileName);
			var shader = Resources.Find<Shader>(jsonMat.shader);
			if(shader==null) {
				throw new Exception("Shader "+jsonMat.shader+" couldn't be found.");
			}
			var material = new Material(jsonMat.name,shader);
			if(jsonMat.textures!=null) {
				foreach(var pair in jsonMat.textures) {
					material.SetTexture(FilterText(pair.Key,fileName),Resources.Import<Texture>(FilterText(pair.Value,fileName)));
				}
			}
			if(jsonMat.vectors!=null) {
				foreach(var pair in jsonMat.vectors) {
					material.SetVector(FilterText(pair.Key,fileName),pair.Value);
				}
			}
			return material;
		}
		private static string FilterText(string str,string file)
		{
			return str.ReplaceCaseInsensitive(
				("$FILE$",		file),
				("$FILENAME$",	Path.GetFileNameWithoutExtension(file))
			);
		}
    }
}