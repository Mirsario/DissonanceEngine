using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using GLPrimitiveType = OpenTK.Graphics.OpenGL.PrimitiveType;

namespace GameEngine.Graphics
{
	public static class GLUtils
	{
		public static void DrawQuadUV() => DrawQuadUV(new Vector4(-1f,-1f,1f,1f),new Vector4(0f,0f,1f,1f));
		public static void DrawQuadUV(Vector4 vertices,Vector4 uvs)
		{
			GL.Begin(GLPrimitiveType.Quads);

			DrawQuadUVDirect(vertices,uvs);

			GL.End();
		}
		public static void DrawQuadUVDirect(Vector4 vertices,Vector4 uvs)
		{
			GL.TexCoord2(uvs.x,uvs.w); GL.Vertex2(vertices.x,vertices.w); 
			GL.TexCoord2(uvs.x,uvs.y); GL.Vertex2(vertices.x,vertices.y); 
			GL.TexCoord2(uvs.z,uvs.y); GL.Vertex2(vertices.z,vertices.y); 
			GL.TexCoord2(uvs.z,uvs.w); GL.Vertex2(vertices.z,vertices.w); 
		}
		public static void DrawQuadUVAttrib() => DrawQuadUVAttrib(new Vector4(-1f,-1f,1f,1f),new Vector4(0f,0f,1f,1f));
		public static void DrawQuadUVAttrib(Vector4 vertices,Vector4 uvs)
		{
			GL.Begin(GLPrimitiveType.Quads);

			DrawQuadUVAttribDirect(vertices,uvs);

			GL.End();
		}
		public static void DrawQuadUVAttribDirect(Vector4 vertices,Vector4 uvs)
		{
			int uvAttrib = (int)AttributeId.Uv0;

			GL.VertexAttrib2(uvAttrib,uvs.x,uvs.w); GL.Vertex2(vertices.x,vertices.w);
			GL.VertexAttrib2(uvAttrib,uvs.x,uvs.y); GL.Vertex2(vertices.x,vertices.y);
			GL.VertexAttrib2(uvAttrib,uvs.z,uvs.y); GL.Vertex2(vertices.z,vertices.y);
			GL.VertexAttrib2(uvAttrib,uvs.z,uvs.w); GL.Vertex2(vertices.z,vertices.w);
		}
	}
}
