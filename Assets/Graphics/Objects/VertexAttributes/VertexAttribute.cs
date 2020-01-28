using Dissonance.Framework.OpenGL;

namespace GameEngine.Graphics
{
	public abstract class VertexAttribute
	{
		public class VertexAttributeInfo
		{
			public string NameId;
			public VertexAttribPointerType PointerType;
			public int Size;
			public bool IsNormalized;

			public VertexAttributeInfo(string nameId,VertexAttribPointerType pointerType,int size,bool isNormalized)
			{
				NameId = nameId;
				PointerType = pointerType;
				Size = size;
				IsNormalized = isNormalized;
			}
		}

		public readonly VertexAttributeInfo AttributeInfo;

		//internal bool isEnabled;

		protected abstract VertexAttributeInfo Info { get; }

		public abstract void Handle();
	}
}
