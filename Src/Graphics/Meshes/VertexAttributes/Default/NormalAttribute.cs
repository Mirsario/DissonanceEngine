using Dissonance.Engine.Graphics.Meshes.Buffers.Default;
using Dissonance.Framework.Graphics;

namespace Dissonance.Engine.Graphics.Meshes.VertexAttributes.Default
{
	public class NormalAttribute : CustomVertexAttribute<NormalBuffer>
	{
		public override void Init(out string nameId,out VertexAttribPointerType pointerType,out bool isNormalized,out int size,out int stride,out int offset)
		{
			nameId = "normal";
			pointerType = VertexAttribPointerType.Float;
			isNormalized = true;
			size = 3;
			stride = 0;
			offset = 0;
		}
	}
}
