using System.Collections.Generic;
using Newtonsoft.Json;
using GameEngine.Graphics;

#pragma warning disable CS0649

namespace GameEngine
{
	[JsonObject]
	internal class JSON_RenderSettings
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
			public Dictionary<string,Texture> textures;
		}

		public class RenderPass
		{
			[JsonProperty(Required = Required.Always)]
			public string type;
			public string framebuffer;
			public Dictionary<string,string[]> passedTextures = new Dictionary<string,string[]>();
			public Dictionary<string,string> shaders;
			public string shader;
		}

		public Dictionary<string,Framebuffer> framebuffers = new Dictionary<string,Framebuffer>();
		[JsonProperty(Required = Required.Always)]
		public Dictionary<string,RenderPass> pipeline;
	}
}
