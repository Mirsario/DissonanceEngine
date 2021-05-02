using System;

namespace Dissonance.Engine.Graphics
{
	public interface IRenderer : IComponent
	{
		Bounds AABB { get; }
		Material Material { get; set; }

		bool GetRenderData(Vector3 rendererPosition, Vector3 cameraPosition, out Material material, out object renderObject);
		void Render(object renderObject);

		void ApplyUniforms(Shader shader) { }
	}
}
