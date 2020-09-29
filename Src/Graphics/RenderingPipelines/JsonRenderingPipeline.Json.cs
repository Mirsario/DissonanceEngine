using System.Collections.Generic;
using Dissonance.Engine.Graphics.Enums;
using Newtonsoft.Json;

#pragma warning disable CS0649

namespace Dissonance.Engine.Graphics.RenderingPipelines
{
	partial class JsonRenderingPipeline
	{
		[JsonObject]
		private class Json
		{
			public class Framebuffer
			{
				public class Texture
				{
					[JsonProperty(Required = Required.Always)]
					public TextureAttachmentType type;

					[JsonProperty(Required = Required.Always)]
					public TextureFormat format;
				}

				[JsonProperty(Required = Required.Always)]
				public Dictionary<string, Texture> textures;
			}

			public class RenderPass
			{
				public string framebuffer;
				public Dictionary<string, string[]> passedTextures = new Dictionary<string, string[]>();
				public Dictionary<string, string> shaders;
				public string shader;

				[JsonProperty(Required = Required.Always)]
				public string type;
			}

			public Dictionary<string, Framebuffer> framebuffers = new Dictionary<string, Framebuffer>();

			[JsonProperty(Required = Required.Always)]
			public Dictionary<string, RenderPass> pipeline;
		}
	}
}
