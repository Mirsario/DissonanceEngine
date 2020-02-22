using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Dissonance.Framework;
using Dissonance.Framework.Graphics;
using Dissonance.Engine.IO;

namespace Dissonance.Engine.Graphics.RenderingPipelines
{
	public class JSONRenderingPipeline : RenderingPipeline
	{
		public override void Setup(List<Framebuffer> framebuffers,List<RenderPass> renderPasses)
		{	
			const string FileName = "rendersettings.json";
			string filePath = Directory.GetFiles("Assets","*.json").FirstOrDefault(file => Path.GetFileName(file).ToLower()==FileName) ?? Resources.BuiltInAssetsFolder+FileName;
			string jsonText = Resources.ImportText(filePath);

			ParseJSON(jsonText,framebuffers,renderPasses);
		}

		internal static void ParseJSON(string jsonText,List<Framebuffer> framebuffers,List<RenderPass> renderPasses)
		{
			//TODO: Stop creating textures here, only make settings!
			//TODO: Make textures resizable.
			var jsonSettings = JsonConvert.DeserializeObject<JSON_RenderSettings>(jsonText);

			ParseJSONFramebuffers(jsonSettings,framebuffers);
			ParseJSONRenderPasses(jsonSettings,framebuffers,renderPasses);
		}
		internal static void ParseJSONFramebuffers(JSON_RenderSettings jsonSettings,List<Framebuffer> framebuffers)
		{
			//Framebuffers
			foreach(var fbPair in jsonSettings.framebuffers) {
				string fbName = fbPair.Key;
				var fb = fbPair.Value;

				var framebuffer = Framebuffer.Create(fbName);

				Framebuffer.Bind(framebuffer);

				foreach(var texPair in fb.textures) {
					//bool noWrite = false;
					string texName = texPair.Key;
					var tex = texPair.Value;

					switch(tex.type) {
						case TextureAttachmentType.Color:
							break;
						case TextureAttachmentType.Depth:
							//noWrite = true;
							break;
						case TextureAttachmentType.DepthStencil: {
							framebuffer.AttachRenderbuffer(new Renderbuffer(texName,RenderbufferStorage.Depth24Stencil8),FramebufferAttachment.DepthStencilAttachment);
							continue;
						}
						default:
							throw new NotImplementedException();
					}

					framebuffer.AttachRenderTexture(new RenderTexture(texName,Screen.Width,Screen.Height,textureFormat:tex.format));

					Rendering.CheckFramebufferStatus();
				}

				framebuffers.Add(framebuffer);
			}
		}
		internal static void ParseJSONRenderPasses(JSON_RenderSettings jsonSettings,List<Framebuffer> framebuffers,List<RenderPass> renderPasses)
		{
			//TODO: Ignore case
			//TODO: Is this needed anywhere else?
			Framebuffer FindFramebuffer(string name,bool throwException = true)
			{
				Framebuffer framebuffer = null;
				for(int i = 0;i<framebuffers.Count;i++) {
					if(framebuffers[i].Name==name) {
						framebuffer = framebuffers[i];
					}
				}

				if(framebuffer==null && throwException) {
					throw new Exception("Couldn't find framebuffer named "+name);
				}

				return framebuffer;
			}

			//RenderPasses
			foreach(var passPair in jsonSettings.pipeline) {
				string passName = passPair.Key;
				var pass = passPair.Value;

				if(!RenderPass.fullNameToType.TryGetValue(pass.type,out Type passType)) {
					throw new Exception($"Couldn't find type {pass.type}, or it does not derive from RenderPass.");
				}

				var passInfo = RenderPass.fullNameToInfo[pass.type] ?? RenderPassInfoAttribute.Default;
				var textureList = new List<RenderTexture>();
				var bufferList = new List<Renderbuffer>();

				foreach(var texPair in pass.passedTextures) {
					string fbName = texPair.Key;
					var texArray = texPair.Value;
					var texFB = FindFramebuffer(fbName);

					for(int i = 0;i<texArray.Length;i++) {
						string texName = texArray[i];
						bool callContinue = false;

						for(int j = 0;j<texFB.renderTextures.Count;j++) {
							var fb = texFB.renderTextures[j];

							if(fb.name==texName) {
								textureList.Add(fb);
								callContinue = true;
								break;
							}
						}

						if(callContinue) {
							continue;
						}

						for(int j = 0;j<texFB.renderbuffers.Length;j++) {
							var rb = texFB.renderbuffers[j];

							if(rb.Name==texName) {
								bufferList.Add(rb);
								callContinue = true;
								break;
							}
						}

						if(callContinue) {
							continue;
						}

						throw new Exception($"Couldn't find texture or a renderbuffer named '{texName}' in framebuffer '{fbName}'.");
					}
				}

				var shadersArr = passInfo.acceptedShaderNames;
				bool shaderRequired = passInfo.requiresShader;

				Shader passShader = null;
				Shader[] passShaders = null;
				if(shadersArr==null) {
					//Single shader
					if(pass.shaders!=null) {
						throw new GraphicsException($"Render pass type ''{pass.type}'' cannot have a ''shaders'' field -- only ''shader'' field is allowed.");
					}

					string shaderName = pass.shader;
					passShader = shaderName==null ? null : Resources.Find<Shader>(shaderName);

					if(passShader==null) {
						if(shaderName!=null) {
							throw new GraphicsException($"Couldn't find shader named ''{shaderName}''.");
						}
						if(shaderRequired) {
							throw new GraphicsException("Render pass type always requires a valid shader, provided in a ''shader'' field.");
						}
					}
				}else{
					//Multiple shaders
					if(pass.shader!=null) {
						throw new GraphicsException($"Render pass type ''{pass.type}'' cannot have a ''shader'' field--only ''shaders'' field is allowed.");
					}

					passShaders = new Shader[shadersArr.Length];

					foreach(var pair in pass.shaders) {
						int index = Array.IndexOf(shadersArr,pair.Key);
						if(index<0) {
							throw new GraphicsException($"Unknown shader type ''{pair.Key}'' for render pass {passName} ({pass.type}).");
						}

						string shaderName = pair.Value;
						if(shaderName==null) {
							continue;
						}

						if((passShaders[index] = Resources.Find<Shader>(shaderName))==null && shaderName!=null) {
							throw new GraphicsException($"Couldn't find shader named ''{shaderName}''.");
						}
					}
				}

				renderPasses.Add(RenderPass.Create(passType,passName,p => {
					p.Framebuffer = !string.IsNullOrWhiteSpace(pass.framebuffer) && pass.framebuffer.ToLower()!="none" ? FindFramebuffer(pass.framebuffer) : null;
					p.PassedTextures = textureList.ToArray();
					p.renderbuffers = bufferList.ToArray();

					if(passShaders!=null) {
						p.Shaders = passShaders;
					} else if(passShader!=null) {
						p.Shader = passShader;
					}
				}));
			}
		}
	}
}