using Dissonance.Framework.OpenGL;
using System;

#pragma warning disable 0649

namespace Dissonance.Engine.Graphics
{
	public static class DrawUtils
	{
		private static Mesh bufferMesh;

		public static void DrawQuadUv0()
		{
			PrimitiveMeshes.ScreenQuad.Render();
		}
		public static void DrawQuadUv0(Vector4 vertices,Vector4 uv0)
		{
			bufferMesh.Vertices[0] = new Vector3(vertices.x,vertices.y,0f);
			bufferMesh.Vertices[1] = new Vector3(vertices.x,vertices.w,0f);
			bufferMesh.Vertices[2] = new Vector3(vertices.z,vertices.w,0f);
			bufferMesh.Vertices[3] = new Vector3(vertices.z,vertices.y,0f);

			bufferMesh.Uv0[0] = new Vector2(uv0.x,uv0.w);
			bufferMesh.Uv0[1] = new Vector2(uv0.x,uv0.y);
			bufferMesh.Uv0[2] = new Vector2(uv0.z,uv0.y);
			bufferMesh.Uv0[3] = new Vector2(uv0.z,uv0.w);

			bufferMesh.Apply();
			bufferMesh.Render();
		}

		internal static void Init()
		{
			bufferMesh?.Dispose();

			bufferMesh = new Mesh {
				Vertices = new Vector3[4],
				Uv0 = new Vector2[4],
				triangles = new int[] {
					0,1,2,
					0,2,3
				}
			};
		}

		/*internal static void DrawTexture(RectFloat rect,Texture texture,Vector4? color = null)
		{
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D,texture.Id);

			if(Shader.ActiveShader.hasDefaultUniform[DefaultShaderUniforms.Color]) {
				var col = color ?? Vector4.One;
				GL.Uniform4(Shader.ActiveShader.defaultUniformIndex[DefaultShaderUniforms.Color],col.x,col.y,col.z,col.w);
			}
			
			var vector = new Vector4(rect.x,rect.y,rect.x+rect.width,rect.y+rect.height);

			if(!GL.TryGetAttribLocation(Shader.ActiveShader.Id,"uv",out uint uvAttrib)) {
				return;
			}

			GL.Begin(PrimitiveType.Quads);

			GL.Vertex2(vector.x,1f-vector.y); GL.VertexAttrib2(uvAttrib,0f,0f);
			GL.Vertex2(vector.x,1f-vector.w); GL.VertexAttrib2(uvAttrib,0f,1f);
			GL.Vertex2(vector.z,1f-vector.w); GL.VertexAttrib2(uvAttrib,1f,1f);
			GL.Vertex2(vector.z,1f-vector.y); GL.VertexAttrib2(uvAttrib,1f,0f);

			GL.End();
		}*/
	}
}