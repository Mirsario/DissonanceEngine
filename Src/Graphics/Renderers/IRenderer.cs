using System;

namespace Dissonance.Engine.Graphics
{
	public interface IRenderer : IComponent
	{
		Material Material { get; set; }

		Bounds GetAabb(Transform transform);
		bool GetRenderData(Vector3 rendererPosition, Vector3 cameraPosition, out Material material, out object renderObject);
		void Render(object renderObject);

		void ApplyUniforms(Shader shader) { }
	}
}
