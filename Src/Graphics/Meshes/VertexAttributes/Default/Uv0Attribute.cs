using Dissonance.Engine.Graphics.Meshes.Buffers.Default;
using Dissonance.Framework.Graphics;

namespace Dissonance.Engine.Graphics.Meshes.VertexAttributes.Default
{
	public class Uv0Attribute : CustomVertexAttribute<Uv0Buffer>
	{
		public override void Init(out string nameId, out VertexAttribPointerType pointerType, out bool isNormalized, out int size, out int stride, out int offset)
		{
			nameId = "uv0";
			pointerType = VertexAttribPointerType.Float;
			isNormalized = false;
			size = 2;
			stride = 0;
			offset = 0;
		}
	}
}
