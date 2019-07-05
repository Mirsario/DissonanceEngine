using System;

namespace GameEngine.Graphics
{
	public abstract class RenderingPipeline : IDisposable
	{
		internal Framebuffer[] framebuffers;
		public Framebuffer[] Framebuffers {
			get => framebuffers;
			set => framebuffers = value ?? new Framebuffer[0];
		}
		internal RenderPass[] renderPasses;
		public RenderPass[] RenderPasses {
			get => renderPasses;
			set => renderPasses = value ?? new RenderPass[0];
		}

		internal void Init()
		{
			Setup(out var tempFramebuffers,out var tempRenderPasses);

			Rendering.CheckGLErrors(); //Do not remove

			if(tempFramebuffers!=null) {
				foreach(var framebuffer in tempFramebuffers) {
					Framebuffer.Bind(framebuffer);
					Rendering.CheckFramebufferStatus();
				}
			}

			Framebuffers = tempFramebuffers;
			RenderPasses = tempRenderPasses;

			/*if(renderPasses==null || renderPasses.Length==0) {
				throw new Exception($"Cannot initialize rendering pipeline {GetType().Name}: Pipeline must have 1 or more rendering passes.");
			}*/
		}

		public abstract void Setup(out Framebuffer[] framebuffers,out RenderPass[] renderPasses); 
		public abstract void Resize(); 

		public virtual void Dispose()
		{
			if(framebuffers!=null) {
				for(int i = 0;i<framebuffers.Length;i++) {
					framebuffers[i].Dispose();
				}
			}

			for(int i = 0;i<renderPasses.Length;i++) {
				renderPasses[i].Dispose();
			}
		}
	}
}