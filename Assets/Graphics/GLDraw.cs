using System;
using System.Linq;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using GLEnableCap = OpenTK.Graphics.OpenGL.EnableCap;

#pragma warning disable 0649

namespace GameEngine.Graphics
{
	//TODO: This was a really bad idea.. let games access OpenTK instead.

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

	public enum GraphicsFeature
	{
		PointSmooth = 2832,
		LineSmooth = 2848,
		LineStipple = 2852,
		PolygonSmooth = 2881,
		PolygonStipple = 2882,
		CullFace = 2884,
		Lighting = 2896,
		ColorMaterial = 2903,
		Fog = 2912,
		DepthTest = 2929,
		StencilTest = 2960,
		Normalize = 2977,
		AlphaTest = 3008,
		Dither = 3024,
		Blend = 3042,
		IndexLogicOp = 3057,
		ColorLogicOp = 3058,
		ScissorTest = 3089,
		TextureGenS = 3168,
		TextureGenT = 3169,
		TextureGenR = 3170,
		TextureGenQ = 3171,
		AutoNormal = 3456,
		Map1Color4 = 3472,
		Map1Index = 3473,
		Map1Normal = 3474,
		Map1TextureCoord1 = 3475,
		Map1TextureCoord2 = 3476,
		Map1TextureCoord3 = 3477,
		Map1TextureCoord4 = 3478,
		Map1Vertex3 = 3479,
		Map1Vertex4 = 3480,
		Map2Color4 = 3504,
		Map2Index = 3505,
		Map2Normal = 3506,
		Map2TextureCoord1 = 3507,
		Map2TextureCoord2 = 3508,
		Map2TextureCoord3 = 3509,
		Map2TextureCoord4 = 3510,
		Map2Vertex3 = 3511,
		Map2Vertex4 = 3512,
		Texture1D = 3552,
		Texture2D = 3553,
		PolygonOffsetPoint = 10753,
		PolygonOffsetLine = 10754,
		ClipDistance0 = 12288,
		ClipPlane0 = 12288,
		ClipDistance1 = 12289,
		ClipPlane1 = 12289,
		ClipDistance2 = 12290,
		ClipPlane2 = 12290,
		ClipDistance3 = 12291,
		ClipPlane3 = 12291,
		ClipDistance4 = 12292,
		ClipPlane4 = 12292,
		ClipDistance5 = 12293,
		ClipPlane5 = 12293,
		ClipDistance6 = 12294,
		ClipDistance7 = 12295,
		Light0 = 16384,
		Light1 = 16385,
		Light2 = 16386,
		Light3 = 16387,
		Light4 = 16388,
		Light5 = 16389,
		Light6 = 16390,
		Light7 = 16391,
		Convolution1D = 32784,
		Convolution1DExt = 32784,
		Convolution2D = 32785,
		Convolution2DExt = 32785,
		Separable2D = 32786,
		Separable2DExt = 32786,
		Histogram = 32804,
		HistogramExt = 32804,
		MinmaxExt = 32814,
		PolygonOffsetFill = 32823,
		RescaleNormal = 32826,
		RescaleNormalExt = 32826,
		Texture3DExt = 32879,
		VertexArray = 32884,
		NormalArray = 32885,
		ColorArray = 32886,
		IndexArray = 32887,
		TextureCoordArray = 32888,
		EdgeFlagArray = 32889,
		InterlaceSgix = 32916,
		Multisample = 32925,
		MultisampleSgis = 32925,
		SampleAlphaToCoverage = 32926,
		SampleAlphaToMaskSgis = 32926,
		SampleAlphaToOne = 32927,
		SampleAlphaToOneSgis = 32927,
		SampleCoverage = 32928,
		SampleMaskSgis = 32928,
		TextureColorTableSgi = 32956,
		ColorTable = 32976,
		ColorTableSgi = 32976,
		PostConvolutionColorTable = 32977,
		PostConvolutionColorTableSgi = 32977,
		PostColorMatrixColorTable = 32978,
		PostColorMatrixColorTableSgi = 32978,
		Texture4DSgis = 33076,
		PixelTexGenSgix = 33081,
		SpriteSgix = 33096,
		ReferencePlaneSgix = 33149,
		IrInstrument1Sgix = 33151,
		CalligraphicFragmentSgix = 33155,
		FramezoomSgix = 33163,
		FogOffsetSgix = 33176,
		SharedTexturePaletteExt = 33275,
		DebugOutputSynchronous = 33346,
		AsyncHistogramSgix = 33580,
		PixelTextureSgis = 33619,
		AsyncTexImageSgix = 33628,
		AsyncDrawPixelsSgix = 33629,
		AsyncReadPixelsSgix = 33630,
		FragmentLightingSgix = 33792,
		FragmentColorMaterialSgix = 33793,
		FragmentLight0Sgix = 33804,
		FragmentLight1Sgix = 33805,
		FragmentLight2Sgix = 33806,
		FragmentLight3Sgix = 33807,
		FragmentLight4Sgix = 33808,
		FragmentLight5Sgix = 33809,
		FragmentLight6Sgix = 33810,
		FragmentLight7Sgix = 33811,
		FogCoordArray = 33879,
		ColorSum = 33880,
		SecondaryColorArray = 33886,
		TextureRectangle = 34037,
		TextureCubeMap = 34067,
		ProgramPointSize = 34370,
		VertexProgramPointSize = 34370,
		VertexProgramTwoSide = 34371,
		DepthClamp = 34383,
		TextureCubeMapSeamless = 34895,
		PointSprite = 34913,
		SampleShading = 35894,
		RasterizerDiscard = 35977,
		PrimitiveRestartFixedIndex = 36201,
		FramebufferSrgb = 36281,
		SampleMask = 36433,
		PrimitiveRestart = 36765,
		DebugOutput = 37600
	}

	public static class GLDraw
	{
		internal static List<Action> drawActions = new List<Action>();
		
		internal static void Draw()
		{
			//test
			/*DrawDelayed(delegate {
				Begin(PrimitiveType.Quads);
				Vertex2(-0.5f,0.5f);	Color3(1f,1f,1f);
				Vertex2(-0.5f,-0.5f);	Color3(1f,0f,0f);
				Vertex2(0.5f,-0.5f);	Color3(0f,1f,0f);
				Vertex2(0.5f,0.5f);		Color3(0f,0f,1f);
				End();
			});*/

			if(drawActions.Count>0) {
				for(int i = 0;i<drawActions.Count;i++) {
					drawActions[i]?.Invoke();
				}
				drawActions.Clear();
			}
		}
		public static void DrawDelayed(Action action)
		{
			drawActions.Add(action);
		}

		//Begin/End
		public static void Begin(PrimitiveType primitiveType) => GL.Begin((OpenTK.Graphics.OpenGL.PrimitiveType)(int)primitiveType);
		public static void End() => GL.End();

		//uniforms
		public static void Uniform1(string uniformName,int value) => GL.Uniform1(GL.GetUniformLocation(Shader.activeShader.Id,uniformName),value);
		public static void Uniform1(string uniformName,float value) => GL.Uniform1(GL.GetUniformLocation(Shader.activeShader.Id,uniformName),value);
		public static void Uniform4(string uniformName,Vector4 vec) => GL.Uniform4(GL.GetUniformLocation(Shader.activeShader.Id,uniformName),vec);
		public static void Uniform4(string uniformName,Vector4[] array) => GL.Uniform4(GL.GetUniformLocation(Shader.activeShader.Id,uniformName),array.Length,array.SelectMany(v => v.ToArray()).ToArray());

		public static void Clear(ClearMask mask) => GL.Clear((ClearBufferMask)mask);
		public static void ClearColor(Vector4 color) => GL.ClearColor(color.x,color.y,color.z,color.w);
		public static void SetShader(Shader shader) => Shader.SetShader(shader);

		public static void Viewport(int x,int y,int width,int height) => GL.Viewport(x,y,width,height);
		public static void LoadIdentity() => GL.LoadIdentity();
		public static void LoadMatrix(Matrix4x4 matrix) => GL.LoadMatrix(matrix);
		
		public static void Enable(GraphicsFeature cap)
		{
			GL.Enable((GLEnableCap)cap);
		}
		public static void Disable(GraphicsFeature cap)
		{
			GL.Disable((GLEnableCap)cap);
		}
		public static void BlendFunc(BlendingFactor sourceFunc,BlendingFactor destFunc)
		{
			Rendering.SetBlendFunc(sourceFunc,destFunc);
		}

		#region Textures
		public static void SetRenderTarget(Framebuffer rt)
		{
			Framebuffer.BindWithDrawBuffers(rt);
		}
		public static void SetTextures(Texture[] textures)
		{
			for(int i = 0;i<textures.Length && i<32;i++) {
				var texture = textures[i];
				
				GL.ActiveTexture((TextureUnit)((int)TextureUnit.Texture0+i));
				GL.BindTexture(TextureTarget.Texture2D,texture.Id);
			}
		}
		public static void SetTextures(Dictionary<string,Texture> textures)
		{
			var arr = textures.ToArray();
			for(int i = 0;i<arr.Length && i<32;i++) {
				string textureName = arr[i].Key;
				var texture = arr[i].Value;
				
				GL.ActiveTexture((TextureUnit)((int)TextureUnit.Texture0+i));
				GL.BindTexture(TextureTarget.Texture2D,texture.Id);
				GL.Uniform1(GL.GetUniformLocation(Shader.activeShader.Id,textureName),i);
			}
		}
		#endregion
		#region Geometry
		//Vertex
		public static void Vertex2(Vector2 v) => GL.Vertex2(v);
		public static void Vertex2(int x,int y) => GL.Vertex2(x,y);
		public static void Vertex2(float x,float y) => GL.Vertex2(x,y);
		public static void Vertex2(double x,double y) => GL.Vertex2(x,y);
		//VertexAttrib
		public static void VertexAttrib1(AttributeId attribute,float val) => GL.VertexAttrib1((int)attribute,val);
		public static void VertexAttrib4(AttributeId attribute,Vector4 vec4) => GL.VertexAttrib4((int)attribute,vec4);
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