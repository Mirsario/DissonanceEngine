using System.Collections.Generic;

namespace Dissonance.Engine.Graphics;

public struct RenderViewData : IRenderComponent
{
	public struct RenderView
	{
		public float NearClip = default;
		public float FarClip = default;
		public RectInt Viewport = default;
		public Transform Transform = default;
		public Matrix4x4 ViewMatrix = default;
		public Matrix4x4 ProjectionMatrix = default;
		public Matrix4x4 InverseViewMatrix = default;
		public Matrix4x4 InverseProjectionMatrix = default;
		public LayerMask LayerMask = LayerMask.All;

		public RenderView() { }
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
