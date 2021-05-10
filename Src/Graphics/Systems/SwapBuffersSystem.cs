using System;
using System.Collections.Generic;
using System.Text;

namespace Dissonance.Engine.Graphics.Systems
{
	[SystemDependency(typeof(RenderFramebufferDebugSystem))]
	[SystemDependency(typeof(RenderPassSystem))]
	public sealed class SwapBuffersSystem : SystemBase
	{
		public override void RenderUpdate()
		{
			Rendering.windowing.SwapBuffers();

			Rendering.CheckGLErrors("After swapping buffers");
		}
	}
}
