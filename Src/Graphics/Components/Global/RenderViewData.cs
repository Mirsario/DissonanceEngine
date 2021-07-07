using System.Collections.Generic;

namespace Dissonance.Engine.Graphics
{
	public struct RenderViewData
	{
		public struct RenderView
		{
			public Camera camera;
			public Transform transform;

			public RenderView(Camera camera, Transform transform)
			{
				this.camera = camera;
				this.transform = transform;
			}
		}

		public List<RenderView> RenderViews { get; set; }
	}
}
