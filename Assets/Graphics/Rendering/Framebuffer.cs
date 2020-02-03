using System;
using System.Collections.Generic;
using GameEngine.Utils;
using Dissonance.Framework.OpenGL;

namespace GameEngine.Graphics
{
	//TODO: WIP
	//TODO: Some fields shouldn't be public
	public class Framebuffer : IDisposable
	{
		internal static Framebuffer activeBuffer;

		public static Framebuffer ActiveBuffer => activeBuffer;
		
		public readonly string Name;
		public readonly uint Id;

		private readonly Dictionary<RenderTexture,FramebufferAttachment> textureToAttachment;

		public List<RenderTexture> renderTextures;
		public Renderbuffer[] renderbuffers;
		public DrawBuffersEnum[] drawBuffers;

		internal FramebufferAttachment nextDefaultAttachment = FramebufferAttachment.ColorAttachment0;
		internal int maxTextureWidth;
		internal int maxTextureHeight;

		protected Framebuffer(string name)
		{
			Name = name;
			Id = GL.GenFramebuffer();

			renderTextures = new List<RenderTexture>();

			textureToAttachment = new Dictionary<RenderTexture,FramebufferAttachment>();
		}

		public void AttachRenderTexture(RenderTexture texture,FramebufferAttachment? attachmentType = null)
		{
			Bind(this);

			var attachment = attachmentType ?? nextDefaultAttachment++;

			GL.FramebufferTexture2D(FramebufferTarget.Framebuffer,attachment,TextureTarget.Texture2D,texture.Id,0);
			
			renderTextures.Add(texture);

			textureToAttachment[texture] = attachment;

			var drawBuffersEnum = (DrawBuffersEnum)attachment;

			if(Enum.IsDefined(typeof(DrawBuffersEnum),drawBuffersEnum)) {
				ArrayUtils.Add(ref drawBuffers,drawBuffersEnum);
			}

			maxTextureWidth = Math.Max(maxTextureWidth,texture.Width);
			maxTextureHeight = Math.Max(maxTextureHeight,texture.Height);
		}
		public void AttachRenderTextures(params RenderTexture[] textures) => AttachRenderTextures((IEnumerable<RenderTexture>)textures);
		public void AttachRenderTextures(IEnumerable<RenderTexture> textures)
		{
			foreach(var texture in textures) {
				AttachRenderTexture(texture);
			}
		}
		public void AttachRenderbuffer(Renderbuffer renderbuffer,FramebufferAttachment? attachmentType = null)
		{
			Bind(this);

			var attachment = attachmentType ?? nextDefaultAttachment++;

			GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer,attachment,RenderbufferTarget.Renderbuffer,Id);

			Rendering.CheckFramebufferStatus();

			ArrayUtils.Add(ref renderbuffers,renderbuffer);

			var drawBuffersEnum = (DrawBuffersEnum)attachment;

			if(Enum.IsDefined(typeof(DrawBuffersEnum),drawBuffersEnum)) {
				ArrayUtils.Add(ref drawBuffers,drawBuffersEnum);
			}
		}

		public void DetachRenderTexture(RenderTexture texture)
		{
			if(!textureToAttachment.TryGetValue(texture,out var attachment)) {
				return;
			}

			Bind(this);

			GL.FramebufferTexture(FramebufferTarget.Framebuffer,attachment,0,0);

			renderTextures.Remove(texture);
			textureToAttachment.Remove(texture);

			var drawBuffersEnum = (DrawBuffersEnum)attachment;
			if(Enum.IsDefined(typeof(DrawBuffersEnum),drawBuffersEnum)) {
				int index = Array.IndexOf(drawBuffers,drawBuffersEnum);
				if(index>=0) {
					ArrayUtils.Remove(ref drawBuffers,index);
				}
			}

			nextDefaultAttachment--;

			Bind(null);
		}

		public void PrepareAttachments()
		{
			if(renderTextures!=null) {
				for(int i = 0;i<renderTextures.Count;i++) {
					renderTextures[i].UpdateSize();
				}
			}
		}

		public Framebuffer WithRenderTexture(RenderTexture texture,FramebufferAttachment? attachmentType = null)
		{
			AttachRenderTexture(texture,attachmentType);
			return this;
		}
		public Framebuffer WithRenderTextures(params RenderTexture[] textures)
		{
			AttachRenderTextures(textures);
			return this;
		}
		public Framebuffer WithRenderbuffer(Renderbuffer renderbuffer,FramebufferAttachment? attachmentType = null)
		{
			AttachRenderbuffer(renderbuffer,attachmentType);
			return this;
		}

		public void Dispose()
		{
			GL.DeleteFramebuffer(Id);

			renderTextures = null;
			renderbuffers = null;
			drawBuffers = null;
		}

		public static Framebuffer Create(string name,Action<Framebuffer> initializer = null)
		{
			var fb = new Framebuffer(name);

			initializer?.Invoke(fb);

			return fb;
		}
		public static void Bind(Framebuffer fb,FramebufferTarget target = FramebufferTarget.Framebuffer)
		{
			GL.BindFramebuffer(target,fb?.Id ?? 0);

			if(target==FramebufferTarget.Framebuffer) {
				activeBuffer = fb;
			}
		}
		public static void BindWithDrawBuffers(Framebuffer fb,FramebufferTarget target = FramebufferTarget.Framebuffer)
		{
			Bind(fb,target);

			if(fb!=null) {
				GL.DrawBuffers(fb.drawBuffers.Length,fb.drawBuffers);
			}else{
				GL.DrawBuffer(DrawBufferMode.FrontLeft);
			}
		}
	}
}