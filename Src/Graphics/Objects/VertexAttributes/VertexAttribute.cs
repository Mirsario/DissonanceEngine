using Dissonance.Framework.Graphics;

namespace Dissonance.Engine.Graphics
{
	public class VertexAttribute : CustomVertexAttribute<VertexBuffer>
	{
		public override void Init(out string nameId,out VertexAttribPointerType pointerType,out bool isNormalized,out int size,out int offset)
		{
			nameId = "vertex";
			pointerType = VertexAttribPointerType.Float;
			isNormalized = false;
			size = 3;
			offset = 0;
		}
	}
}
