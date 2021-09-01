using System.Collections.Generic;

namespace Dissonance.Engine.Graphics
{
	public struct RenderViewData : IRenderComponent
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

		public List<RenderView> RenderViews { get; private set; }

		public void Reset()
		{
			if (RenderViews != null) {
				RenderViews.Clear();
			} else {
				RenderViews = new();
			}
		}
	}
}
