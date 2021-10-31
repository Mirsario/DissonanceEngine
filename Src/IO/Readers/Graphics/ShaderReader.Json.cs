using Dissonance.Framework.Graphics;
using Newtonsoft.Json;
using System.Collections.Generic;
using Dissonance.Engine.Graphics;

#pragma warning disable CS0649 // Field is never assigned to

namespace Dissonance.Engine.IO
{
	public partial class ShaderReader
	{
		[JsonObject]
		private class JsonShaderProgram
		{
			// Shaders
			[JsonRequired]
			public string VertexShader;

			[JsonRequired]
			public string FragmentShader;

			public string GeometryShader;
			public string[] ShaderDefines;
			// Parameters
			public int Queue;
			public CullMode CullMode = CullMode.Front;
			public PolygonMode PolygonMode = PolygonMode.Fill;
			public BlendingFactor BlendFactorSrc = BlendingFactor.One;
			public BlendingFactor BlendFactorDst = BlendingFactor.Zero;
			public DepthFunction DepthTest = DepthFunction.Less;
			public bool DepthWrite = true;
			// Uniforms
			public Dictionary<string, float> Floats;
			public Dictionary<string, float[]> Vectors;
			public Dictionary<string, string> Textures;
		}
	}
}
