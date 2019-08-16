using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
