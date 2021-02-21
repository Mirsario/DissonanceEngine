using System;

namespace Dissonance.Engine.Graphics
{
	public abstract class Renderer : Component
	{
		public abstract Bounds AABB { get; }
		public abstract Material Material { get; set; }

		protected override void OnDispose()
		{
			Material = null;
		}

		public virtual void ApplyUniforms(Shader shader) { }

		public abstract bool GetRenderData(Vector3 rendererPosition, Vector3 cameraPosition, out Material material, out object renderObject);
		public abstract void Render(object renderObject);
	}
}
