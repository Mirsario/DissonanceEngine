using System;
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

		public Framebuffer(string name)
		{
			Name = name;
			Id = GL.GenFramebuffer();
		}
		public void Dispose()
		{
			GL.DeleteFramebuffer(Id);
			renderTextures = null;
			renderbuffers = null;
			drawBuffers = null;
		}

		public Framebuffer WithRenderTexture(RenderTexture texture,FramebufferAttachment? attachmentType = null)
		{
			AttachRenderTexture(texture,attachmentType);
			return this;
		}
		public Framebuffer WithRenderTexture(RenderTexture texture,out RenderTexture textureOut,FramebufferAttachment? attachmentType = null)
		{
			AttachRenderTexture(textureOut = texture,attachmentType);
			return this;
		}
		public void AttachRenderTexture(RenderTexture texture,FramebufferAttachment? attachmentType = null)
		{
			Bind(this);

			var attachment = attachmentType ?? nextDefaultAttachment++;
			GL.FramebufferTexture2D(FramebufferTarget.Framebuffer,(GLFramebufferAttachment)attachment,TextureTarget.Texture2D,texture.Id,0);
			Rendering.CheckFramebufferStatus();
			
			InternalUtils.ArrayAdd(ref renderTextures,texture);
			var drawBuffersEnum = (DrawBuffersEnum)attachment;
			if(Enum.IsDefined(typeof(DrawBuffersEnum),drawBuffersEnum)) {
				InternalUtils.ArrayAdd(ref drawBuffers,drawBuffersEnum);
			}
		}

		public Framebuffer WithRenderbuffer(Renderbuffer renderbuffer,FramebufferAttachment? attachmentType = null)
		{
			AttachRenderbuffer(renderbuffer,attachmentType);
			return this;
		}
		public Framebuffer WithRenderbuffer(Renderbuffer renderbuffer,out Renderbuffer renderbufferOut,FramebufferAttachment? attachmentType = null)
		{
			AttachRenderbuffer(renderbufferOut = renderbuffer,attachmentType);
			return this;
		}
		public void AttachRenderbuffer(Renderbuffer renderbuffer,FramebufferAttachment? attachmentType = null)
		{
			Bind(this);

			var attachment = attachmentType ?? nextDefaultAttachment++;
			GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer,(GLFramebufferAttachment)attachment,RenderbufferTarget.Renderbuffer,Id);
			Rendering.CheckFramebufferStatus();
			
			InternalUtils.ArrayAdd(ref renderbuffers,renderbuffer);
			var drawBuffersEnum = (DrawBuffersEnum)attachment;
			if(Enum.IsDefined(typeof(DrawBuffersEnum),drawBuffersEnum)) {
				InternalUtils.ArrayAdd(ref drawBuffers,drawBuffersEnum);
			}
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