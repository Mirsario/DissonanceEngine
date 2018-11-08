using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace GameEngine
{
	public class RenderSettings
	{
		public Framebuffer[] framebuffers;
		public RenderPass[] renderPasses;
		
		public static RenderSettings FromFile(string filePath) => FromJSON(Resources.ImportText(filePath));
		public static RenderSettings FromJSON(string json)
		{
			//TODO: Stop creating textures here, only make settings!
			//TODO: Make textures be resizable btw.
			var jsonSettings = JsonConvert.DeserializeObject<JSON_RenderSettings>(json);
			#region Framebuffers
			var framebufferList = new List<Framebuffer>();
			foreach(var fbPair in jsonSettings.framebuffers) {
				string fbName = fbPair.Key;
				var fb = fbPair.Value;

				var framebuffer = new Framebuffer(fbName);
				Framebuffer.Bind(framebuffer);

				var textureList = new List<RenderTexture>();
				var renderBuffers = new List<RenderBuffer>();
				var drawBuffers = new List<DrawBuffersEnum>();
				int colorTexAmount = 0;
				foreach(var texPair in fb.textures) {
					FramebufferAttachment attachmentType;
					bool noWrite = false;
					string texName = texPair.Key;
					var tex = texPair.Value;

					switch(tex.type) {
						case TextureAttachmentType.Color:
							attachmentType = FramebufferAttachment.ColorAttachment0+colorTexAmount++;
							break;
						case TextureAttachmentType.Depth:
							attachmentType = FramebufferAttachment.DepthAttachment;
							noWrite = true;
							break;
						case TextureAttachmentType.DepthStencil: {
							//TODO: Ew
							uint renderBufferId = (uint)GL.GenRenderbuffer();
							GL.BindRenderbuffer(RenderbufferTarget.RenderbufferExt,renderBufferId);
							GL.RenderbufferStorage(RenderbufferTarget.RenderbufferExt,RenderbufferStorage.Depth24Stencil8,Graphics.ScreenWidth,Graphics.ScreenHeight);
							GL.FramebufferRenderbuffer(FramebufferTarget.FramebufferExt,FramebufferAttachment.DepthAttachment,RenderbufferTarget.RenderbufferExt,renderBufferId);
							GL.FramebufferRenderbuffer(FramebufferTarget.FramebufferExt,FramebufferAttachment.StencilAttachment,RenderbufferTarget.RenderbufferExt,renderBufferId);
							Graphics.CheckFramebufferStatus();
							renderBuffers.Add(new RenderBuffer(renderBufferId,texName));
							continue;
						}
						default:
							throw new NotImplementedException();
					}
					if(!noWrite) {
						drawBuffers.Add((DrawBuffersEnum)attachmentType);
					}
					var renderTexture = new RenderTexture(texName,Graphics.ScreenWidth,Graphics.ScreenHeight,framebuffer,textureFormat:tex.format,fbBinded:true,attachmentType:attachmentType);
					Graphics.CheckFramebufferStatus();
					textureList.Add(renderTexture);
				}
				framebuffer.textures = textureList.ToArray();
				framebuffer.drawBuffers = drawBuffers.ToArray();
				framebuffer.renderBuffers = renderBuffers.ToArray();
				framebufferList.Add(framebuffer);
			}
			var framebuffers = framebufferList.ToArray();
			//Make sure RenderBuffers are attached to all FBOs
			for(int i=0;i<framebuffers.Length;i++) {
				var fb = framebuffers[i];
				for(int j=0;j<fb.renderBuffers.Length;j++) {
					for(int k=0;k<framebuffers.Length;k++) {
						if(i!=k) {
							Framebuffer.Bind(framebuffers[k]);
							GL.FramebufferRenderbuffer(FramebufferTarget.FramebufferExt,FramebufferAttachment.DepthAttachment,	RenderbufferTarget.RenderbufferExt,fb.renderBuffers[j].id);
							GL.FramebufferRenderbuffer(FramebufferTarget.FramebufferExt,FramebufferAttachment.StencilAttachment,RenderbufferTarget.RenderbufferExt,fb.renderBuffers[j].id);
						}
					}
				}
			}
			#endregion
			//TODO: Ignore case
			//TODO: Is this needed anywhere else?
			Framebuffer FindFramebuffer(string name,bool throwException = true)
			{
				Framebuffer framebuffer = null;
				for(int i=0;i<framebuffers.Length;i++) {
					if(framebuffers[i].name==name) {
						framebuffer = framebuffers[i];
					}
				}
				if(framebuffer==null && throwException) {
					throw new Exception("Couldn't find framebuffer named "+name);
				}
				return framebuffer;
			}
			#region RenderPasses
			var passList = new List<RenderPass>();
			foreach(var passPair in jsonSettings.pipeline) {
				string passName = passPair.Key;
				var pass = passPair.Value;

				if(!RenderPass.idToInstance.TryGetValue(pass.type,out var passInstance)) {
					throw new Exception($"{pass.type} is not a valid render pass type");
				}

				Framebuffer usedFramebuffer = null;
				if(pass.framebuffer!="none" && pass.framebuffer!="") {
					usedFramebuffer = FindFramebuffer(pass.framebuffer);
				}
					
				var textureList = new List<RenderTexture>();
				var bufferList = new List<RenderBuffer>();
				foreach(var texPair in pass.passedTextures) {
					string fbName = texPair.Key;
					var texArray = texPair.Value;
					var texFB = FindFramebuffer(fbName);
					for(int i=0;i<texArray.Length;i++) {
						string texName = texArray[i];
						bool callContinue = false;
						for(int j=0;j<texFB.textures.Length;j++) {
							if(texFB.textures[j].name==texName) {
								textureList.Add(texFB.textures[j]);
								callContinue = true;
								break;
							}
						}
						if(callContinue) {
							continue;
						}
						for(int j=0;j<texFB.renderBuffers.Length;j++) {
							if(texFB.renderBuffers[j].name==texName) {
								bufferList.Add(texFB.renderBuffers[j]);
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
				//Hardcoded shit below
				var shadersArr = passInstance.AcceptedShaderNames;
				bool shaderRequired = passInstance.RequiresShader;

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

				var newPass = (RenderPass)Activator.CreateInstance(passInstance.GetType());
				newPass.name = passName;
				newPass.framebuffer = usedFramebuffer;
				newPass.textures = textureList.ToArray();
				newPass.renderBuffers = bufferList.ToArray();
				newPass.passShader = passShader;
				newPass.shaders = passShaders;
				passList.Add(newPass);
			}
			return new RenderSettings {
				framebuffers = framebuffers,
				renderPasses = passList.ToArray()
			};
			#endregion
		}
	}
}