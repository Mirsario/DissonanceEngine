using Dissonance.Engine.Graphics.Meshes.Buffers.Default;
using Dissonance.Framework.Graphics;

namespace Dissonance.Engine.Graphics.Meshes.VertexAttributes.Default
{
	public class VertexAttribute : CustomVertexAttribute<VertexBuffer>
	{
		public override void Init(out string nameId, out VertexAttribPointerType pointerType, out bool isNormalized, out int size, out int stride, out int offset)
		{
			nameId = "vertex";
			pointerType = VertexAttribPointerType.Float;
			isNormalized = false;
			size = 3;
			stride = 0;
			offset = 0;
		}
	}
}
