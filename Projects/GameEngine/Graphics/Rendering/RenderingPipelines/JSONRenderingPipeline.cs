using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using OpenTK;

namespace GameEngine.Graphics.RenderingPipelines
{
	public class JSONRenderingPipeline : RenderingPipeline
	{
		public override void Setup(out Framebuffer[] buffers,out RenderPass[] passes)
		{	
			const string FileName = "rendersettings.json";
			string filePath = Directory.GetFiles("Assets","*.json").FirstOrDefault(file => Path.GetFileName(file).ToLower()==FileName) ?? Resources.builtInAssetsFolder+FileName;
			string jsonText = Resources.ImportText(filePath);

			ParseJSON(jsonText,out buffers,out passes);
		}

		internal static void ParseJSON(string jsonText,out Framebuffer[] buffers,out RenderPass[] passes)
		{
			//TODO: Stop creating textures here, only make settings!
			//TODO: Make textures be resizable btw.
			var jsonSettings = JsonConvert.DeserializeObject<JSON_RenderSettings>(jsonText);

			ParseJSONFramebuffers(jsonSettings,out buffers);
			ParseJSONRenderPasses(jsonSettings,buffers,out passes);

			Debug.Log("e");

			//Make sure Renderbuffers are attached to all FBOs
			//TODO: ^ this is weirdly located v
			//TODO: Also, is this something really really lazy? I don't remember.
			/*for(int i=0;i<buffers.Length;i++) {
				var fb = buffers[i];
				for(int j=0;j<fb.renderbuffers.Length;j++) {
					for(int k=0;k<buffers.Length;k++) {
						if(i!=k) {
							Framebuffer.Bind(buffers[k]);

							var renderbuffer = fb.renderbuffers[j];
							GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer,FramebufferAttachment.DepthAttachment,RenderbufferTarget.Renderbuffer,renderbuffer.Id);
							GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer,FramebufferAttachment.StencilAttachment,RenderbufferTarget.Renderbuffer,renderbuffer.Id);
						}
					}
				}
			}*/
		}
		internal static void ParseJSONFramebuffers(JSON_RenderSettings jsonSettings,out Framebuffer[] framebuffers)
		{
			//Framebuffers
			var framebufferList = new List<Framebuffer>();
			foreach(var fbPair in jsonSettings.framebuffers) {
				string fbName = fbPair.Key;
				var fb = fbPair.Value;

				var framebuffer = new Framebuffer(fbName);
				Framebuffer.Bind(framebuffer);

				//var textureList = new List<RenderTexture>();
				//var renderbuffers = new List<Renderbuffer>();
				//var drawBuffers = new List<DrawBuffersEnum>();
				//int colorTexAmount = 0;

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
							var renderbuffer = new Renderbuffer(texName,RenderbufferStorage.Depth24Stencil8);
							framebuffer.AttachRenderbuffer(renderbuffer,FramebufferAttachment.DepthStencilAttachment);
							//renderbuffers.Add(renderbuffer);
							continue;
						}
						default:
							throw new NotImplementedException();
					}

					/*if(!noWrite) {
						drawBuffers.Add((DrawBuffersEnum)attachmentType);
					}*/

					var renderTexture = new RenderTexture(texName,Screen.Width,Screen.Height,textureFormat:tex.format);
					framebuffer.AttachRenderTexture(renderTexture);
					Rendering.CheckFramebufferStatus();
					//textureList.Add(renderTexture);
				}

				//framebuffer.renderTextures = textureList.ToArray();
				//framebuffer.drawBuffers = drawBuffers.ToArray();
				//framebuffer.renderbuffers = renderbuffers.ToArray();
				framebufferList.Add(framebuffer);
			}

			framebuffers = framebufferList.ToArray();
		}
		internal static void ParseJSONRenderPasses(JSON_RenderSettings jsonSettings,Framebuffer[] framebuffers,out RenderPass[] renderPasses)
		{
			//TODO: Ignore case
			//TODO: Is this needed anywhere else?
			Framebuffer FindFramebuffer(string name,bool throwException = true)
			{
				Framebuffer framebuffer = null;
				for(int i=0;i<framebuffers.Length;i++) {
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
			var passList = new List<RenderPass>();
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

					for(int i=0;i<texArray.Length;i++) {
						string texName = texArray[i];
						bool callContinue = false;
						for(int j=0;j<texFB.renderTextures.Length;j++) {
							if(texFB.renderTextures[j].name==texName) {
								textureList.Add(texFB.renderTextures[j]);
								callContinue = true;
								break;
							}
						}
						if(callContinue) {
							continue;
						}
						for(int j=0;j<texFB.renderbuffers.Length;j++) {
							if(texFB.renderbuffers[j].Name==texName) {
								bufferList.Add(texFB.renderbuffers[j]);
								callContinue = true;
								break;
							}
						}
						if(callContinue) {
							continue;
						}
						throw new Exception("Couldn't find texture or a renderbuffer named "+texName+" in framebuffer "+fbName);
					}
				}
				var shadersArr = passInfo.acceptedShaderNames;
				bool shaderRequired = passInfo.requiresShader;

				Shader passShader = null;
				Shader[] passShaders = null;
				if(shadersArr==null) {
					//Single shader
					if(pass.shaders!=null) {
						throw new GraphicsException("Render pass type ''"+pass.type+"'' cannot have a ''shaders'' field -- only ''shader'' field is allowed.");
					}
					string shaderName = pass.shader;
					passShader = shaderName==null ? null : Resources.Find<Shader>(shaderName);
					if(passShader==null) {
						if(shaderName!=null) {
							throw new GraphicsException("Couldn't find shader named ''"+shaderName+"''.");
						}
						if(shaderRequired) {
							throw new GraphicsException("Render pass type always requires a valid shader, provided in a ''shader'' field.");
						}
					}
				}else{
					//Multiple shaders
					if(pass.shader!=null) {
						throw new GraphicsException("Render pass type ''"+pass.type+"'' cannot have a ''shader'' field--only ''shaders'' field is allowed.");
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
							throw new GraphicsException("Couldn't find shader named ''"+shaderName+"''.");
						}
					}
				}

				var newPass = (RenderPass)Activator.CreateInstance(passType,passName); //newPass.name = passName;
				newPass.framebuffer = !string.IsNullOrWhiteSpace(pass.framebuffer) && pass.framebuffer.ToLower()!="none" ? FindFramebuffer(pass.framebuffer) : null;
				newPass.passedTextures = textureList.ToArray();
				newPass.renderbuffers = bufferList.ToArray();
				newPass.passShader = passShader;
				newPass.shaders = passShaders;
				passList.Add(newPass);
			}

			renderPasses = passList.ToArray();
		}
	}
}