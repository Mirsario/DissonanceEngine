using System;
using OpenTK.Graphics.OpenGL;

namespace GameEngine.Graphics
{
	public class NormalAttribute : VertexAttribute
	{
		protected override VertexAttributeInfo Info => new VertexAttributeInfo("normal",VertexAttribPointerType.Float,3,true);

		public override void Handle()
		{
			throw new NotImplementedException();
		}
	}
}
