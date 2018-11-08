using System;
using OpenTK.Graphics.OpenGL;

namespace GameEngine
{
	public class Framebuffer : IDisposable
	{
		public static Framebuffer activeBuffer;
		
		public string name;
		public RenderTexture[] textures;
		public RenderBuffer[] renderBuffers;
		public DrawBuffersEnum[] drawBuffers;

		public int Id { get; protected set; }

		public Framebuffer(string name)
		{
			this.name = name;
			Id = GL.GenFramebuffer();
		}
		public void Dispose()
		{
			GL.DeleteFramebuffer(Id);
		}

		public static void Bind(Framebuffer fb,FramebufferTarget target = FramebufferTarget.FramebufferExt)
		{
			GL.Ext.BindFramebuffer(target,fb?.Id ?? 0);
			if((int)target==(int)FramebufferTarget.Framebuffer) {
				activeBuffer = fb;
			}
		}
	}
}