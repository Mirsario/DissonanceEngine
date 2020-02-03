using System;
using Dissonance.Framework.OpenGL;

namespace GameEngine.Graphics
{
	public class NormalAttribute : CustomVertexAttribute<NormalBuffer>
	{
		public override void Init(out string nameId,out VertexAttribPointerType pointerType,out bool isNormalized,out int size,out int offset)
		{
			nameId = "normal";
			pointerType = VertexAttribPointerType.Float;
			isNormalized = true;
			size = 3;
			offset = 0;
		}
	}
}
