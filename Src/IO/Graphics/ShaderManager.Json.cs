using Dissonance.Framework.Graphics;
using Dissonance.Engine.Graphics;
using Newtonsoft.Json;
using System.Collections.Generic;

#pragma warning disable CS0649

namespace Dissonance.Engine.IO.Graphics
{
	public partial class ShaderManager
	{
		[JsonObject]
		private class JsonShaderProgram
		{
			//Shaders
			[JsonRequired]
			public string vertexShader;

			[JsonRequired]
			public string fragmentShader;

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
	}
}