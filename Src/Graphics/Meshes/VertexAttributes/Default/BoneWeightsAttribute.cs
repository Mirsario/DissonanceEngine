using Dissonance.Engine.Graphics.Meshes.Buffers.Default;
using Dissonance.Framework.Graphics;

namespace Dissonance.Engine.Graphics.Meshes.VertexAttributes.Default
{
	public class BoneWeightsAttribute : CustomVertexAttribute<BoneWeightsBuffer>
	{
		public override void Init(out string nameId,out VertexAttribPointerType pointerType,out bool isNormalized,out int size,out int stride,out int offset)
		{
			nameId = "boneWeights";
			pointerType = VertexAttribPointerType.Float;
			isNormalized = false;
			size = 4;
			stride = sizeof(float)*4;
			offset = sizeof(int)*4;
		}
	}
}
