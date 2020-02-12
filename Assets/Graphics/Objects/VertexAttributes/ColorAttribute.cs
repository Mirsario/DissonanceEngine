using System;
using Dissonance.Framework.OpenGL;

namespace Dissonance.Engine.Graphics
{
	public class ColorAttribute : CustomVertexAttribute<ColorBuffer>
	{
		public override void Init(out string nameId,out VertexAttribPointerType pointerType,out bool isNormalized,out int size,out int offset)
		{
			nameId = "color";
			pointerType = VertexAttribPointerType.Float;
			isNormalized = false;
			size = 4;
			offset = 0;
		}
	}
}
