﻿using Silk.NET.OpenGL;

namespace Dissonance.Engine.Graphics
{
	public class NormalAttribute : CustomVertexAttribute<NormalBuffer>
	{
		public override void Init(out string nameId, out VertexAttribPointerType pointerType, out bool isNormalized, out int size, out uint stride, out int offset)
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
