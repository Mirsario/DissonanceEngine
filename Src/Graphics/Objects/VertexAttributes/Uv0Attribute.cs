using Dissonance.Framework.OpenGL;

namespace Dissonance.Engine.Graphics
{
	public class Uv0Attribute : CustomVertexAttribute<Uv0Buffer>
	{
		public override void Init(out string nameId,out VertexAttribPointerType pointerType,out bool isNormalized,out int size,out int offset)
		{
			nameId = "uv0";
			pointerType = VertexAttribPointerType.Float;
			isNormalized = false;
			size = 2;
			offset = 0;
		}
	}
}
