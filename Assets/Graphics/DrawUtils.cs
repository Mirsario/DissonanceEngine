using OpenTK.Graphics.OpenGL;
using PrimitiveTypeGL = OpenTK.Graphics.OpenGL.PrimitiveType;

#pragma warning disable 0649

namespace GameEngine.Graphics
{
	public static class DrawUtils
	{
		/*public static void Clear(ClearMask mask) => GL.Clear((ClearBufferMask)mask);
		public static void ClearColor(Vector4 color) => GL.ClearColor(color.x,color.y,color.z,color.w);
		public static void SetShader(Shader shader) => Shader.SetShader(shader);

		public static void Viewport(int x,int y,int width,int height) => GL.Viewport(x,y,width,height);
		public static void LoadIdentity() => GL.LoadIdentity();
		public static void LoadMatrix(Matrix4x4 matrix) => GL.LoadMatrix(matrix);*/

		internal static void DrawTexture(RectFloat rect,Texture texture,Vector4? color = null)
		{
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D,texture.Id);

			if(Shader.activeShader.hasDefaultUniform[DefaultShaderUniforms.Color]) {
				var col = color ?? Vector4.One;
				GL.Uniform4(Shader.activeShader.defaultUniformIndex[DefaultShaderUniforms.Color],col.x,col.y,col.z,col.w);
			}
			
			int uvAttrib = GL.GetAttribLocation(Shader.activeShader.Id,"uv");
			var vector = new Vector4(
				rect.x,
				rect.y,
				rect.x+rect.width,
				rect.y+rect.height
			);

			GL.Begin(PrimitiveTypeGL.Quads);

			GL.Vertex2(vector.x,1f-vector.y); GL.VertexAttrib2(uvAttrib,0f,0f);
			GL.Vertex2(vector.x,1f-vector.w); GL.VertexAttrib2(uvAttrib,0f,1f);
			GL.Vertex2(vector.z,1f-vector.w); GL.VertexAttrib2(uvAttrib,1f,1f);
			GL.Vertex2(vector.z,1f-vector.y); GL.VertexAttrib2(uvAttrib,1f,0f);

			GL.End();
		}
	}
}