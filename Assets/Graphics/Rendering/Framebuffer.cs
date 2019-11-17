using System;
using GameEngine.Utils;
using OpenTK.Graphics.OpenGL;

using GLFramebufferAttachment = OpenTK.Graphics.OpenGL.FramebufferAttachment;

namespace GameEngine.Graphics
{
	public class Framebuffer : IDisposable
	{
		public enum Target
		{
			ReadFramebuffer = 36008,
			DrawFramebuffer = 36009,
			Framebuffer = 36160,
		}
		
		internal static Framebuffer activeBuffer;
		public static Framebuffer ActiveBuffer => activeBuffer;
		
		public readonly string Name;
		public readonly int Id;

		public RenderTexture[] renderTextures;
		public Renderbuffer[] renderbuffers;
		public DrawBuffersEnum[] drawBuffers;

		internal FramebufferAttachment nextDefaultAttachment = FramebufferAttachment.ColorAttachment0;
		internal int maxTextureWidth;
		internal int maxTextureHeight;

		protected Framebuffer(string name)
		{
			Name = name;
			Id = GL.GenFramebuffer();
		}

		public void AttachRenderTexture(RenderTexture texture,FramebufferAttachment? attachmentType = null)
		{
			Bind(this);

			var attachment = attachmentType ?? nextDefaultAttachment++;
			GL.FramebufferTexture2D(FramebufferTarget.Framebuffer,(GLFramebufferAttachment)attachment,TextureTarget.Texture2D,texture.Id,0);
			Rendering.CheckFramebufferStatus();
			
			ArrayUtils.Add(ref renderTextures,texture);

			var drawBuffersEnum = (DrawBuffersEnum)attachment;
			if(Enum.IsDefined(typeof(DrawBuffersEnum),drawBuffersEnum)) {
				ArrayUtils.Add(ref drawBuffers,drawBuffersEnum);
			}

			maxTextureWidth = Math.Max(maxTextureWidth,texture.Width);
			maxTextureHeight = Math.Max(maxTextureHeight,texture.Height);
		}
		public void AttachRenderTextures(params RenderTexture[] textures)
		{
			for(int i = 0;i<textures.Length;i++) {
				AttachRenderTexture(textures[i]);
			}
		}
		public void AttachRenderbuffer(Renderbuffer renderbuffer,FramebufferAttachment? attachmentType = null)
		{
			Bind(this);

			var attachment = attachmentType ?? nextDefaultAttachment++;
			GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer,(GLFramebufferAttachment)attachment,RenderbufferTarget.Renderbuffer,Id);
			Rendering.CheckFramebufferStatus();

			ArrayUtils.Add(ref renderbuffers,renderbuffer);

			var drawBuffersEnum = (DrawBuffersEnum)attachment;
			if(Enum.IsDefined(typeof(DrawBuffersEnum),drawBuffersEnum)) {
				ArrayUtils.Add(ref drawBuffers,drawBuffersEnum);
			}
		}
		public void PrepareAttachments()
		{
			if(renderTextures!=null) {
				for(int i = 0;i<renderTextures.Length;i++) {
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
		public static void Bind(Framebuffer fb,Target target = Target.Framebuffer)
		{
			GL.BindFramebuffer((FramebufferTarget)target,fb?.Id ?? 0);

			if((int)target==(int)Target.Framebuffer) {
				activeBuffer = fb;
			}
		}
		public static void BindWithDrawBuffers(Framebuffer fb,Target target = Target.Framebuffer)
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