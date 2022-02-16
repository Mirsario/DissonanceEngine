using Dissonance.Framework.Graphics;

namespace Dissonance.Engine.Graphics
{
	public sealed class Uv0Attribute : CustomVertexAttribute<Uv0Buffer>
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

	public sealed class Uv1Attribute : CustomVertexAttribute<Uv1Buffer>
	{
		public override void Init(out string nameId, out VertexAttribPointerType pointerType, out bool isNormalized, out int size, out int stride, out int offset)
		{
			nameId = "uv1";
			pointerType = VertexAttribPointerType.Float;
			isNormalized = false;
			size = 2;
			stride = 0;
			offset = 0;
		}
	}

	public sealed class Uv2Attribute : CustomVertexAttribute<Uv2Buffer>
	{
		public override void Init(out string nameId, out VertexAttribPointerType pointerType, out bool isNormalized, out int size, out int stride, out int offset)
		{
			nameId = "uv2";
			pointerType = VertexAttribPointerType.Float;
			isNormalized = false;
			size = 2;
			stride = 0;
			offset = 0;
		}
	}

	public sealed class Uv3Attribute : CustomVertexAttribute<Uv3Buffer>
	{
		public override void Init(out string nameId, out VertexAttribPointerType pointerType, out bool isNormalized, out int size, out int stride, out int offset)
		{
			nameId = "uv3";
			pointerType = VertexAttribPointerType.Float;
			isNormalized = false;
			size = 2;
			stride = 0;
			offset = 0;
		}
	}
}
