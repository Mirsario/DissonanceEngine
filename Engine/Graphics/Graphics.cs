using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using ImagingPixelFormat = System.Drawing.Imaging.PixelFormat;
using PrimitiveTypeGL = OpenTK.Graphics.OpenGL.PrimitiveType;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace GameEngine
{
	//TODO: Add submeshes to Mesh.cs
	//TODO: Add some way to sort objects in a way that'd let the engine skip boxinfrustum checks for objects which are in non-visible chunks.
	public static class Graphics
	{
		#region PixelFormats
		public static Dictionary<TextureFormat,(PixelFormat formatGeneral,PixelInternalFormat formatInternal,PixelType pixelType,Type dataType)> textureFormatInfo = new Dictionary<TextureFormat,(PixelFormat,PixelInternalFormat,PixelType,Type)> {
			#region A
			{ TextureFormat.A8,					(PixelFormat.Alpha,				PixelInternalFormat.Alpha8,				PixelType.UnsignedByte,		typeof(byte)) },	//8
			{ TextureFormat.A16,				(PixelFormat.Alpha,				PixelInternalFormat.Alpha16,			PixelType.UnsignedByte,		typeof(byte)) },	//16
			#endregion
			#region R
			{ TextureFormat.R8,					(PixelFormat.Red,				PixelInternalFormat.R8,					PixelType.UnsignedByte,		typeof(byte)) },	//8
			{ TextureFormat.R8i,				(PixelFormat.RedInteger,		PixelInternalFormat.R8i,				PixelType.Byte,				typeof(sbyte)) },
			{ TextureFormat.R8ui,				(PixelFormat.RedInteger,		PixelInternalFormat.R8ui,				PixelType.UnsignedByte,		typeof(byte)) },
			{ TextureFormat.R16,				(PixelFormat.Red,				PixelInternalFormat.R16,				PixelType.UnsignedShort,	typeof(ushort)) },	//16
			{ TextureFormat.R16f,				(PixelFormat.Red,				PixelInternalFormat.R16f,				PixelType.HalfFloat,		typeof(Half)) },
			{ TextureFormat.R16i,				(PixelFormat.RedInteger,		PixelInternalFormat.R16i,				PixelType.Short,			typeof(short)) },
			{ TextureFormat.R16ui,				(PixelFormat.RedInteger,		PixelInternalFormat.R16ui,				PixelType.UnsignedShort,	typeof(ushort)) },
			{ TextureFormat.R32f,				(PixelFormat.Red,				PixelInternalFormat.R32f,				PixelType.Float,			typeof(float)) },	//32
			{ TextureFormat.R32i,				(PixelFormat.RedInteger,		PixelInternalFormat.R32i,				PixelType.Int,				typeof(int)) },
			{ TextureFormat.R32ui,				(PixelFormat.RedInteger,		PixelInternalFormat.R32ui,				PixelType.UnsignedInt,		typeof(uint)) },
			#endregion
			#region RG
			{ TextureFormat.RG8,				(PixelFormat.Rg,				PixelInternalFormat.Rg8,				PixelType.UnsignedByte,		typeof(byte)) },	//8
			{ TextureFormat.RG8i,				(PixelFormat.RgInteger,			PixelInternalFormat.Rg8i,				PixelType.Byte,				typeof(sbyte)) },
			{ TextureFormat.RG8ui,				(PixelFormat.RgInteger,			PixelInternalFormat.Rg8ui,				PixelType.UnsignedByte,		typeof(byte)) },
			{ TextureFormat.RG16,				(PixelFormat.Rg,				PixelInternalFormat.Rg16,				PixelType.UnsignedShort,	typeof(ushort)) },	//16
			{ TextureFormat.RG16f,				(PixelFormat.Rg,				PixelInternalFormat.Rg16f,				PixelType.HalfFloat,		typeof(Half)) },
			{ TextureFormat.RG16i,				(PixelFormat.RgInteger,			PixelInternalFormat.Rg16i,				PixelType.Short,			typeof(short)) },
			{ TextureFormat.RG16ui,				(PixelFormat.RgInteger,			PixelInternalFormat.Rg16ui,				PixelType.UnsignedShort,	typeof(ushort)) },
			{ TextureFormat.RG32f,				(PixelFormat.Rg,				PixelInternalFormat.Rg32f,				PixelType.Float,			typeof(float)) },	//32
			{ TextureFormat.RG32i,				(PixelFormat.RgInteger,			PixelInternalFormat.Rg32i,				PixelType.Int,				typeof(int)) },
			{ TextureFormat.RG32ui,				(PixelFormat.RgInteger,			PixelInternalFormat.Rg32ui,				PixelType.UnsignedInt,		typeof(uint)) },
			#endregion
			#region RGB
			{ TextureFormat.RGB8,				(PixelFormat.Rgb,				PixelInternalFormat.Rgb8,				PixelType.UnsignedByte,		typeof(byte)) },	//8
			{ TextureFormat.RGB8i,				(PixelFormat.RgbInteger,		PixelInternalFormat.Rgb8i,				PixelType.Byte,				typeof(sbyte)) },
			{ TextureFormat.RGB8ui,				(PixelFormat.RgbInteger,		PixelInternalFormat.Rgb8ui,				PixelType.UnsignedByte,		typeof(byte)) },
			{ TextureFormat.RGB16,				(PixelFormat.Rgb,				PixelInternalFormat.Rgb16,				PixelType.UnsignedShort,	typeof(ushort)) },	//16
			{ TextureFormat.RGB16f,				(PixelFormat.Rgb,				PixelInternalFormat.Rgb16f,				PixelType.HalfFloat,		typeof(Half)) },
			{ TextureFormat.RGB16i,				(PixelFormat.RgbInteger,		PixelInternalFormat.Rgb16,				PixelType.Short,			typeof(short)) },
			{ TextureFormat.RGB16ui,			(PixelFormat.RgbInteger,		PixelInternalFormat.Rgb16ui,			PixelType.UnsignedShort,	typeof(ushort)) },
			{ TextureFormat.RGB32f,				(PixelFormat.Rgb,				PixelInternalFormat.Rgb32f,				PixelType.Float,			typeof(float)) },	//32
			{ TextureFormat.RGB32i,				(PixelFormat.RgbInteger,		PixelInternalFormat.Rgb32i,				PixelType.Int,				typeof(int)) },
			{ TextureFormat.RGB32ui,			(PixelFormat.RgbInteger,		PixelInternalFormat.Rgb32ui,			PixelType.UnsignedInt,		typeof(uint)) },
			#endregion
			#region RGBA
			{ TextureFormat.RGBA8,				(PixelFormat.Rgba,				PixelInternalFormat.Rgba8,				PixelType.UnsignedByte,		typeof(byte)) },	//8
			{ TextureFormat.RGBA8i,				(PixelFormat.RgbaInteger,		PixelInternalFormat.Rgba8i,				PixelType.Byte,				typeof(sbyte)) },
			{ TextureFormat.RGBA8ui,			(PixelFormat.RgbaInteger,		PixelInternalFormat.Rgba8ui,			PixelType.UnsignedByte,		typeof(byte)) },
			{ TextureFormat.RGBA16,				(PixelFormat.Rgba,				PixelInternalFormat.Rgba16,				PixelType.UnsignedShort,	typeof(ushort)) },	//16
			{ TextureFormat.RGBA16f,			(PixelFormat.Rgba,				PixelInternalFormat.Rgba16f,			PixelType.HalfFloat,		typeof(Half)) },
			{ TextureFormat.RGBA16i,			(PixelFormat.RgbaInteger,		PixelInternalFormat.Rgba16,				PixelType.Short,			typeof(short)) },
			{ TextureFormat.RGBA16ui,			(PixelFormat.RgbaInteger,		PixelInternalFormat.Rgba16ui,			PixelType.UnsignedShort,	typeof(ushort)) },
			{ TextureFormat.RGBA32f,			(PixelFormat.Rgba,				PixelInternalFormat.Rgba32f,			PixelType.Float,			typeof(float)) },	//32
			{ TextureFormat.RGBA32i,			(PixelFormat.RgbaInteger,		PixelInternalFormat.Rgba32i,			PixelType.Int,				typeof(int)) },
			{ TextureFormat.RGBA32ui,			(PixelFormat.RgbaInteger,		PixelInternalFormat.Rgba32ui,			PixelType.UnsignedInt,		typeof(uint)) },
			#endregion
			#region DepthComponent
			{ TextureFormat.Depth16,			(PixelFormat.DepthComponent,	PixelInternalFormat.DepthComponent16,	PixelType.UnsignedShort,	typeof(ushort)) },	//16
			{ TextureFormat.Depth32,			(PixelFormat.DepthComponent,	PixelInternalFormat.DepthComponent32,	PixelType.UnsignedInt,		typeof(uint)) },
			{ TextureFormat.Depth32f,			(PixelFormat.DepthComponent,	PixelInternalFormat.DepthComponent32f,	PixelType.Float,			typeof(float)) },	//32
			#endregion
			#region DepthStencil
			{ TextureFormat.Depth24Stencil8,	(PixelFormat.DepthStencil,		PixelInternalFormat.Depth24Stencil8,	PixelType.UnsignedInt248,	typeof(int)) },			//16+8
			{ TextureFormat.Depth32fStencil8,	(PixelFormat.DepthStencil,		PixelInternalFormat.Depth32fStencil8,	PixelType.Float32UnsignedInt248Rev,	typeof(float)) },	//24+8
			#endregion
		};
		public static Dictionary<PixelInternalFormat,PixelFormat> pixelFormatConversion = new Dictionary<PixelInternalFormat,PixelFormat> {
			{PixelInternalFormat.Rgb16,						PixelFormat.Rgb},
			{PixelInternalFormat.Rgb16f,					PixelFormat.Rgb},
			{PixelInternalFormat.Rgb32f,					PixelFormat.Rgb},
			{PixelInternalFormat.Rgba16,					PixelFormat.Rgba},
			{PixelInternalFormat.Rgba16f,					PixelFormat.Rgba},
			{PixelInternalFormat.Rgba32f,					PixelFormat.Rgba},
			{PixelInternalFormat.DepthComponent16,			PixelFormat.DepthComponent},
			{PixelInternalFormat.DepthComponent24,			PixelFormat.DepthComponent},
			{PixelInternalFormat.DepthComponent32,			PixelFormat.DepthComponent},
			{PixelInternalFormat.DepthComponent32f,			PixelFormat.DepthComponent},
			{PixelInternalFormat.Depth24Stencil8,			PixelFormat.DepthStencil},
			{PixelInternalFormat.Depth32fStencil8,			PixelFormat.DepthStencil},
			{PixelInternalFormat.DepthStencil,				PixelFormat.DepthStencil},
		};
		#endregion

		//Screen
		//TODO: Avoid extra math by changing this to automatic properties, updated somewhere.
		public static int ScreenX => window.Location.X;
		public static int ScreenY => window.Location.Y;
		public static int ScreenWidth => window.Width;
		public static int ScreenHeight => window.Height;
		public static Vector2 ScreenCenter => new Vector2(window.Width*0.5f,window.Height*0.5f);
		public static Vector2 WindowCenter => new Vector2(window.Location.X+window.Width*0.5f,window.Location.Y+window.Height*0.5f);

		internal static GameWindow window;
		internal static List<Camera> cameraList;
		internal static List<Renderer> rendererList;
		internal static List<Light> lightList;
		internal static RenderSettings renderSettings;
		
		//TODO: Move this
		internal static Texture whiteTexture;

		#region Hardcoded
		//TODO: Fix this badcode
		private static Shader _guiShader;
		public static Shader GUIShader => _guiShader ?? (_guiShader = Resources.Find<Shader>("GUI"));
		internal static int textBufferId = -1;
		internal static DrawBuffersEnum[] nullDrawBuffers = { DrawBuffersEnum.ColorAttachment0 };
		public static int defaultStencil = 0;
		public static Vector3 ambientColor = new Vector3(0.1f,0.1f,0.1f);//new Vector3(0.5f,0.5f,0.5f);
		#endregion

		#region Initialization
		internal static void Init()
		{
			var glVersion = GetOpenGLVersion();
			var minVersion = new Version("3.3");
			if(glVersion<minVersion) {
				throw new GraphicsException($"Please update your graphics drivers.\nMinimum OpenGL version required to run this application is: {minVersion}\nYour OpenGL version is: {glVersion}");
			}
			CheckGLErrors();

			#region FontImport
			//TODO: Add AssetManager for fonts and remove this hardcode
			var tex = Resources.Import<Texture>("BuiltInAssets/GUI/Fonts/DefaultFont.png");
			GUI.font = new Font(@" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~",tex,new Vector2(12f,16f),0) { size = 16 };
			#endregion

			cameraList = new List<Camera>();
			rendererList = new List<Renderer>();
			lightList = new List<Light>();
			
			GL.Enable(EnableCap.Texture2D);
			GL.CullFace(CullFaceMode.Back);
			GL.ClearDepth(1f);
			GL.DepthFunc(DepthFunction.Lequal);
			
			#region RenderSettings
			renderSettings = RenderSettings.FromFile(Directory.GetFiles("Assets","*.json").FirstOrDefault(file => Path.GetFileName(file).ToLower()=="rendersettings.json") ?? Resources.builtInAssetsFolder+"rendersettings.json");
			#endregion

			PrimitiveMeshes.GenerateDefaultMeshes();
			
			whiteTexture = new Texture(1,1);
		}
		
		#endregion
		#region Rendering
		internal static void Render()
		{
			//Debug.Log("Render()");
			CheckGLErrors();

			//Calculate view and projection matrices,culling frustums
			for(int i=0;i<cameraList.Count;i++) {
				var camera = cameraList[i];
				float aspectRatio = camera.ViewPixel.Width/(float)camera.ViewPixel.Height;
				camera.matrix_view = Matrix4x4.LookAt(camera.Transform.Position,camera.Transform.Position+camera.Transform.Forward,camera.Transform.Up);
				if(camera.orthographic) {
					float max = Mathf.Max(ScreenWidth,ScreenHeight);
					camera.matrix_proj = Matrix4x4.CreateOrthographic(ScreenWidth/max*camera.orthographicSize,ScreenHeight/max*camera.orthographicSize,camera.nearClip,camera.farClip);
				}else{
					camera.matrix_proj = Matrix4x4.CreatePerspectiveFOV(camera.fov*Mathf.Deg2Rad,aspectRatio,camera.nearClip,camera.farClip);
				}
				camera.matrix_viewInverse = Matrix4x4.Invert(camera.matrix_view);
				camera.matrix_projInverse = Matrix4x4.Invert(camera.matrix_proj);
				camera.CalculateFrustum(camera.matrix_view*camera.matrix_proj);
			}
			//Clear buffers
			//GL.Enable(EnableCap.StencilTest);
			for(int i=0;i<=renderSettings.framebuffers.Length;i++) {
				Framebuffer.Bind(i==renderSettings.framebuffers.Length ? null : renderSettings.framebuffers[i]);

				GL.Viewport(0,0,ScreenWidth,ScreenHeight);

				GL.ClearColor(0f,0f,0f,0f);
				GL.StencilMask(~0);
				GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
			}
			//Render passes
			//GL.StencilFunc(StencilFunction.Always,0,0);
			for(int i=0;i<renderSettings.renderPasses.Length;i++) {
				renderSettings.renderPasses[i].Render();
				CheckGLErrors();
			}
			//GL.Disable(EnableCap.StencilTest);

			#region RenderTargetDebug
			if(Input.GetKey(Keys.F)) {
				int textureCount = 0;
				for(int i=0;i<renderSettings.framebuffers.Length;i++) {
					for(int j=0;j<renderSettings.framebuffers[i].textures.Length;j++) {
						var tex = renderSettings.framebuffers[i].textures[j];
						if(tex.attachment!=FramebufferAttachment.DepthAttachment) {
							textureCount++;
						}
					}
				}
				//uhh when did i do this
				int size = 1;
				while(size*size<textureCount) {
					size++;
				}
				//
				//int textureId = 0;
				int x = 0;
				int y = 0;
				for(int i=0;i<renderSettings.framebuffers.Length;i++) {
					var framebuffer = renderSettings.framebuffers[i];
					Framebuffer.Bind(framebuffer,FramebufferTarget.ReadFramebuffer);
					for(int j=0;j<framebuffer.textures.Length;j++) {
						if(framebuffer.textures[j].attachment!=FramebufferAttachment.DepthAttachment) {
							GL.ReadBuffer((ReadBufferMode)((int)ReadBufferMode.ColorAttachment0+j));
							GL.BlitFramebuffer(
								0,0,ScreenWidth,ScreenHeight,
								x*(ScreenWidth/size),(size-y-1)*(ScreenHeight/size),(x+1)*(ScreenWidth/size),(size-y)*(ScreenHeight/size),
								ClearBufferMask.ColorBufferBit,BlitFramebufferFilter.Nearest
							);
							CheckGLErrors();
							//textureId++;
							x++;
							if(x>=size) {
								x = 0;
								y++;
							}
						}
					}
				}
			}
			#endregion

			GUIPass();
			
			window.SwapBuffers();
			GL.Flush();
			CheckGLErrors();
		}
		internal static void GUIPass()
		{
			CheckGLErrors();
			Framebuffer.Bind(null);
			
			CheckGLErrors();
			Shader.SetShader(GUIShader);
			
			CheckGLErrors();
			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactor.SrcAlpha,BlendingFactor.OneMinusSrcAlpha);
			
			GUI.canDraw = true;
			CheckGLErrors();
			Game.instance.OnGUI();	//<<<
			CheckGLErrors();
			for(int i=0;i<GameObject.gameObjects.Count;i++) {
				var gameObject = GameObject.gameObjects[i];
				gameObject.OnGUI();
			}
			GUI.canDraw = false;
			
			GL.BlendFunc(BlendingFactor.One,BlendingFactor.Zero);
			GL.Disable(EnableCap.Blend);
		}
		#endregion
		#region Utils
		#region ErrorChecks
		public static bool CheckGLErrors(bool throwException = true,object prefix = null)
		{
			if(prefix==null) {
				prefix = "";
			}
			var error = GL.GetError();
			switch(error) {
				case ErrorCode.NoError:
					//Debug.Log(prefix.ToString()+"itsfine",stackframeOffset:2);
					return false;
				default:
					if(throwException) {
						throw new GraphicsException(prefix.ToString()+error);
					}else{
						Debug.Log(prefix.ToString()+error,stackframeOffset:2);
					}
					return true;
			}
		}
		internal static void CheckFramebufferStatus()
		{
			var errorCode = GL.CheckFramebufferStatus(FramebufferTarget.FramebufferExt);
			switch(errorCode) {
				case FramebufferErrorCode.FramebufferComplete:
					return;
				case FramebufferErrorCode.FramebufferIncompleteAttachment:
					throw new Exception("An attachment could not be bound to frame buffer object!");
				case FramebufferErrorCode.FramebufferIncompleteMissingAttachment:
					throw new Exception("Attachments are missing! At least one image (texture) must be bound to the frame buffer object!");
				case FramebufferErrorCode.FramebufferIncompleteDimensionsExt:
					throw new Exception("The dimensions of the buffers attached to the currently used frame buffer object do not match!");
				case FramebufferErrorCode.FramebufferIncompleteFormatsExt:
					throw new Exception("The formats of the currently used frame buffer object are not supported or do not fit together!");
				case FramebufferErrorCode.FramebufferIncompleteDrawBuffer:
					throw new Exception("A Draw buffer is incomplete or undefinied. All draw buffers must specify attachment points that have images attached.");
				case FramebufferErrorCode.FramebufferIncompleteReadBuffer:
					throw new Exception("A Read buffer is incomplete or undefinied. All read buffers must specify attachment points that have images attached.");
				case FramebufferErrorCode.FramebufferIncompleteMultisample:
					throw new Exception("All images must have the same number of multisample samples.");
				case FramebufferErrorCode.FramebufferIncompleteLayerTargets :
					throw new Exception("If a layered image is attached to one attachment,then all attachments must be layered attachments. The attached layers do not have to have the same number of layers,nor do the layers have to come from the same kind of texture.");
				case FramebufferErrorCode.FramebufferUnsupported:
					throw new Exception("Attempt to use an unsupported format combinaton!");
				default:
					throw new Exception("Unknown error while attempting to create frame buffer object!");
			}
		}
		#endregion
		public static Version GetOpenGLVersion()
		{
			string versionStr = GL.GetString(StringName.Version);
			var strings = new List<string>();
			bool recording = false;
			for(int i=0;i<versionStr.Length;i++) {
				char c = versionStr[i];
				if(char.IsDigit(c) || c=='.' && recording) {
					if(!recording) {
						strings.Add(char.ToString(c));
						recording = true;
					}else{
						strings[strings.Count-1] += c;
					}
				}else{
					recording = false;
				}
			}
			var versions = strings.Select(s => new Version(s)).ToArray();
			var testVer = new Version(6,0);
			return versions.First(v => v<testVer);
		}
		#endregion
		
		//TODO: Move this
		internal static void DrawString(Font font,float fontSize,Rect rect,string text,TextAlignment alignment = TextAlignment.UpperLeft)
		{
			if(string.IsNullOrEmpty(text)) {
				return;
			}
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D,font.texture.Id);
			GL.Uniform1(GL.GetUniformLocation(GUIShader.program,"mainTex"),0);
			
			float scale = fontSize/font.charSize.y;
			var position = new Vector2(rect.x,rect.y);
			if(alignment==TextAlignment.UpperCenter || alignment==TextAlignment.MiddleCenter || alignment==TextAlignment.LowerCenter) {
				position.x += rect.width/2f-font.charSize.x*scale*text.Length/2f;
			}
			if(alignment==TextAlignment.MiddleLeft || alignment==TextAlignment.MiddleCenter || alignment==TextAlignment.MiddleRight) {
				position.y += rect.height/2f-fontSize/2f;
			}
			
			float xPos = position.x/ScreenWidth;
			float yPos = position.y/ScreenHeight;
			float width = font.charSize.x/ScreenWidth*scale;
			float height = font.charSize.y/ScreenHeight*scale;
			int uvAttrib = GL.GetAttribLocation(GUIShader.program,"uv");
			GL.Begin(PrimitiveTypeGL.Quads);
			for(int i=0;i<text.Length;i++) {
				char c = text[i];
				if(!char.IsWhiteSpace(c) && font.charToUv.TryGetValue(c,out var uvs)) {
					GL.VertexAttrib2(uvAttrib,uvs[0]);
					GL.Vertex2(xPos,1f-yPos);
					GL.VertexAttrib2(uvAttrib,uvs[1]);
					GL.Vertex2(xPos+width,1f-yPos);
					GL.VertexAttrib2(uvAttrib,uvs[2]);
					GL.Vertex2(xPos+width,1f-yPos-height);
					GL.VertexAttrib2(uvAttrib,uvs[3]);
					GL.Vertex2(xPos,1f-yPos-height);
				}
				xPos += width;
			}
			GL.End();
		}
		//TODO: Actually make window resizing a thing.
		internal static void Resize(object sender,EventArgs e) {}
		
		#region Testing
		/*internal static void TestStencils()
		{
			GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt,1);	
			GL.Enable(EnableCap.StencilTest);
			GL.ClearStencil(0);
			GL.Clear(ClearBufferMask.StencilBufferBit);
			
			GL.StencilFunc(StencilFunction.Always,1,1);	//Not testing,but writing
			GL.StencilOp(StencilOp.Keep,StencilOp.Keep,StencilOp.Replace);//Write options
			GL.StencilMask(1);
			TestStencil(0.25f,true,new Vector3(1f,0.8f,0f));
			
			GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt,2);	
			GL.StencilFunc(StencilFunction.Notequal,1,1);
			GL.StencilOp(StencilOp.Keep,StencilOp.Keep,StencilOp.Keep);//Write options
			TestStencil(0.5f);
			GL.Disable(EnableCap.StencilTest);
		}*/
		/*public static void TestStencils2()
		{
			GL.StencilFunc(StencilFunction.Notequal,0x01,0x01);
			TestStencil(1f);
		}*/
		/*public static void TestStencil(float s = 1f,bool reset = true,Vector3? col = null)
		{
			if(reset) {
				//GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt,0);
				//GL.DrawBuffers(1,nullDrawBuffers);
				GL.UseProgram(0);
				GL.ActiveTexture(TextureUnit.Texture0);
				GL.BindTexture(TextureTarget.Texture2D,0);
			}
			Vector3 col2 = col==null ? new Vector3(0f,0f,0.5f) : col.Value;
			GL.Begin(PrimitiveTypeGL.Quads);
				GL.Color3(col2.x,col2.y,col2.z);
				GL.Vertex2(-s,s);	GL.TexCoord2(0,s);
				GL.Vertex2(-s,-s);	GL.TexCoord2(0,0);
				GL.Vertex2( s,-s);	GL.TexCoord2(s,0);
				GL.Vertex2( s,s);	GL.TexCoord2(s,s);
			GL.End();
		}*/
		#endregion

		internal static void Dispose() {}
	}
}