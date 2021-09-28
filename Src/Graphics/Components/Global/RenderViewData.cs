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

			public RenderView(Transform transform, RectInt viewport, float nearClip, float farClip, Matrix4x4 viewMatrix, Matrix4x4 projectionMatrix, Matrix4x4 inverseViewMatrix, Matrix4x4 inverseProjectionMatrix)
			{
				Transform = transform;
				Viewport = viewport;
				NearClip = nearClip;
				FarClip = farClip;
				ViewMatrix = viewMatrix;
				ProjectionMatrix = projectionMatrix;
				InverseViewMatrix = inverseViewMatrix;
				InverseProjectionMatrix = inverseProjectionMatrix;
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
