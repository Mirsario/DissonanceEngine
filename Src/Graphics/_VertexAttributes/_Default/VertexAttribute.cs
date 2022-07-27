using Silk.NET.OpenGL;

namespace Dissonance.Engine.Graphics;

public class VertexAttribute : CustomVertexAttribute<VertexBuffer>
{
	public override void Init(out string nameId, out VertexAttribPointerType pointerType, out bool isNormalized, out int size, out uint stride, out int offset)
	{
		nameId = "vertex";
		pointerType = VertexAttribPointerType.Float;
		isNormalized = false;
		size = 3;
		stride = 0;
		offset = 0;
	}
}
