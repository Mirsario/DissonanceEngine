using Dissonance.Framework.Graphics;

namespace Dissonance.Engine.Graphics
{
	public class TangentAttribute : CustomVertexAttribute<TangentBuffer>
	{
		public override void Init(out string nameId, out VertexAttribPointerType pointerType, out bool isNormalized, out int size, out int stride, out int offset)
		{
			nameId = "tangent";
			pointerType = VertexAttribPointerType.Float;
			isNormalized = false;
			size = 4;
			stride = 0;
			offset = 0;
		}
	}
}
