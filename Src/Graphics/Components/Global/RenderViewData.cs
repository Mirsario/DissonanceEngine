using System.Collections.Generic;

namespace Dissonance.Engine.Graphics
{
	public struct RenderViewData : IRenderComponent
	{
		public struct RenderView
		{
			public float NearClip;
			public float FarClip;
			public RectInt Viewport;
			public Transform Transform;
			public Matrix4x4 ViewMatrix;
			public Matrix4x4 ProjectionMatrix;
			public Matrix4x4 InverseViewMatrix;
			public Matrix4x4 InverseProjectionMatrix;
			public LayerMask LayerMask = LayerMask.All;
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
