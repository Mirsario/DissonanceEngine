using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using OpenTK.Graphics.OpenGL;

#pragma warning disable 0649

namespace GameEngine
{
	//TODO: Allow users access to OpenTK instead?

	[Flags]
	public enum ClearMask
	{
		None = 0,
		DepthBufferBit = 256,
		AccumBufferBit = 512,
		StencilBufferBit = 1024,
		ColorBufferBit = 16384,
		CoverageBufferBitNv = 32768
	}
	public static class GLDraw
	{
		internal static List<Action> drawActions = new List<Action>();
		
		internal static void Draw()
		{
			//test
			/*DrawDelayed(delegate {
				Begin(PrimitiveType.Quads);
				Vertex2(-0.5f,0.5f);
				Color3(1f,1f,1f);
				Vertex2(-0.5f,-0.5f);
				Color3(1f,0f,0f);
				Vertex2(0.5f,-0.5f);
				Color3(0f,1f,0f);
				Vertex2(0.5f,0.5f);
				Color3(0f,0f,1f);
				End();
			});*/

			for(int i=0;i<drawActions.Count;i++) {
				drawActions[i]?.Invoke();
			}
			drawActions.Clear();
		}
		public static void DrawDelayed(Action action)
		{
			drawActions.Add(action);
		}

		//Begin/End
		public static void Begin(PrimitiveType primitiveType) => GL.Begin((OpenTK.Graphics.OpenGL.PrimitiveType)(int)primitiveType);
		public static void End() => GL.End();

		//uniforms
		public static void Uniform4(string uniformName,Vector4[] array) => GL.Uniform4(GL.GetUniformLocation(Shader.activeShader.program,uniformName),array.Length,array.SelectMany(v => v.ToArray()).ToArray());

		public static void Clear(ClearMask mask) => GL.Clear((ClearBufferMask)mask);
		public static void ClearColor(Vector4 color) => GL.ClearColor(color.x,color.y,color.z,color.w);
		public static void SetShader(Shader shader) => Shader.SetShader(shader);

		public static void Viewport(int x,int y,int width,int height) => GL.Viewport(x,y,width,height);
		public static void LoadIdentity() => GL.LoadIdentity();
		public static void LoadMatrix(Matrix4x4 matrix) => GL.LoadMatrix(matrix);

		#region Textures
		public static void SetRenderTarget(RenderTexture rt)
		{
			Framebuffer.Bind(rt?.framebuffer);
		}
		public static void SetTextures(Texture[] textures)
		{
			for(int i=0;i<textures.Length && i<32;i++) {
				var texture = textures[i];
				
				GL.ActiveTexture((TextureUnit)((int)TextureUnit.Texture0+i));
				GL.BindTexture(TextureTarget.Texture2D,texture.Id);
			}
		}
		public static void SetTextures(Dictionary<string,Texture> textures)
		{
			var arr = textures.ToArray();
			for(int i=0;i<arr.Length && i<32;i++) {
				string textureName = arr[i].Key;
				var texture = arr[i].Value;
				
				GL.ActiveTexture((TextureUnit)((int)TextureUnit.Texture0+i));
				GL.BindTexture(TextureTarget.Texture2D,texture.Id);
				GL.Uniform1(GL.GetUniformLocation(Shader.activeShader.program,textureName),i);
			}
		}
		#endregion
		#region Geometry
		//Vertex
		public static void Vertex2(Vector2 v) => GL.Vertex2(v);
		public static void Vertex2(int x,int y) => GL.Vertex2(x,y);
		public static void Vertex2(float x,float y) => GL.Vertex2(x,y);
		public static void Vertex2(double x,double y) => GL.Vertex2(x,y);
		//UV
		public static void TexCoord2(Vector2 v)
		{
			if(Shader.activeShader==null) {
				GL.TexCoord2(v);
			}else{
				GL.VertexAttrib2((int)AttributeId.Uv0,v);
			}
		}
		public static void TexCoord2(int x,int y) => TexCoord2(new Vector2(x,y));
		public static void TexCoord2(float x,float y) => TexCoord2(new Vector2(x,y));
		public static void TexCoord2(double x,double y) => TexCoord2(new Vector2((float)x,(float)y));
		//other
		//public static void VertexAttrib2() => GL.VertexAttrib2(
		//Color
		//RGB
		public static void Color3(byte r,byte g,byte b) => GL.Color3(r,g,b);
		public static void Color3(float r,float g,float b) => GL.Color3(r,g,b);
		public static void Color3(double r,double g,double b) => GL.Color3(r,g,b);
		public static void Color3(Vector3 rgb) => GL.Color3(rgb.x,rgb.y,rgb.z);
		//RGBA
		public static void Color4(byte r,byte g,byte b,byte a) => GL.Color4(r,g,b,a);
		public static void Color4(float r,float g,float b,float a) => GL.Color4(r,g,b,a);
		public static void Color4(double r,double g,double b,double a) => GL.Color4(r,g,b,a);
		public static void Color3(Vector4 rgba) => GL.Color4(rgba.x,rgba.y,rgba.z,rgba.w);
		#endregion
	}
	public enum PrimitiveType
	{
		Points,
		Lines,
		LineLoop,
		LineStrip,
		Triangles,
		TriangleStrip,
		TriangleFan,
		Quads,
		QuadsExt7,
		QuadStrip,
		Polygon,
		LinesAdjacency,
		LinesAdjacencyArb10,
		LinesAdjacencyExt10,
		LineStripAdjacency,
		LineStripAdjacencyArb11,
		LineStripAdjacencyExt11,
		TrianglesAdjacency,
		TrianglesAdjacencyArb12,
		TrianglesAdjacencyExt12,
		TriangleStripAdjacency,
		TriangleStripAdjacencyArb = 13,
		TriangleStripAdjacencyExt = 13,
		Patches,
		PatchesExt = 14
	}
}