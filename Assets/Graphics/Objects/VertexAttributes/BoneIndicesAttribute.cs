using System;
using Dissonance.Framework.OpenGL;

namespace GameEngine.Graphics
{
	public class BoneIndicesAttribute : CustomVertexAttribute<BoneWeightsBuffer>
	{
		public override void Init(out string nameId,out VertexAttribPointerType pointerType,out bool isNormalized,out int size,out int offset)
		{
			nameId = "boneIndices";
			pointerType = VertexAttribPointerType.Int;
			isNormalized = false;
			size = 4;
			offset = 0;
		}
	}
}
