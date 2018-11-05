using OpenTK.Graphics.OpenGL;
using PrimitiveTypeGL = OpenTK.Graphics.OpenGL.PrimitiveType;
using GameEngine.Graphics;

namespace GameEngine
{
	public class GUIStyle
	{
		public Texture texInactive;
		public Texture texNormal;
		public Texture texHover;
		public Texture texActive;
		public RectOffset border;
		public TextAlignment textAlignment;
		public int fontSize = 16;

		public GUIStyle()
		{
			texInactive = GUI.texDefaultInactive;
			texNormal = GUI.texDefault;
			texHover = GUI.texDefaultHover;
			texActive = GUI.texDefaultActive;
			border = new RectOffset(6f,6f,6f,6f);
			textAlignment = TextAlignment.UpperLeft;
		}
	}
	public class GUISkin
	{
		public GUIStyle boxStyle;
		public GUIStyle buttonStyle;
		public GUISkin()
		{
			boxStyle = new GUIStyle();
			buttonStyle = new GUIStyle {
				textAlignment = TextAlignment.MiddleCenter
			};
		}
	}
	public enum TextAlignment
	{
		UpperLeft,
		UpperCenter,
		UpperRight,
		MiddleLeft,
		MiddleCenter,
		MiddleRight,
		LowerLeft,
		LowerCenter,
		LowerRight
	}
	public static class GUI
	{
		internal static bool canDraw;
		public static GUISkin skin;
		public static Font font;
		
		public static Texture texDefaultInactive;
		public static Texture texDefault;
		public static Texture texDefaultHover;
		public static Texture texDefaultActive;
		
		internal static void Init()
		{
			texDefaultInactive = Resources.Import<Texture>("BuiltInAssets/GUI/DefaultInactive.png");
			texDefault = Resources.Import<Texture>("BuiltInAssets/GUI/Default.png");
			texDefaultHover = Resources.Import<Texture>("BuiltInAssets/GUI/DefaultHover.png");
			texDefaultActive = Resources.Import<Texture>("BuiltInAssets/GUI/DefaultActive.png");
			skin = new GUISkin();
		}
		internal static void Update()
		{
			
		}
		
		public static void Box(RectFloat rect,Vector4? color)
		{
			Draw(rect,skin.boxStyle.texNormal,color,skin.boxStyle);
		}
		public static bool Button(RectFloat rect,string text = null,bool active = true,Vector4? color = null)
		{
			bool hover = rect.Contains(Input.MousePosition,true);
			bool anyPress = Input.GetMouseButton(0);
			var style = skin.buttonStyle;
			var tex = active ? hover ? anyPress ? style.texActive : style.texHover : style.texNormal : style.texInactive;
			Draw(rect,tex,color,style);
			if(!string.IsNullOrEmpty(text)) {
				var textRect = new RectFloat(
					rect.x+style.border.left,
					rect.y+style.border.top,
					rect.width-style.border.left-style.border.right,
					rect.height-style.border.top-style.border.bottom
				);
				DrawString(font,style.fontSize,textRect,text,alignment:style.textAlignment);
			}
			
			return active && hover && Input.GetMouseButtonUp(0) && rect.Contains(Input.MousePosition,true);
		}
		public static void DrawText(RectFloat rect,string text,Vector4? color = null,TextAlignment alignment = TextAlignment.UpperLeft,float fontSize = -1)
		{
			if(fontSize==-1) {
				fontSize = font.size;
			}

			DrawString(font,fontSize,rect,text,color,alignment);
		}
		public static void DrawTexture(RectFloat rect,Texture texture,Vector4? color = null)
		{
			Draw(rect,texture,color);
		}
		internal static void Draw(RectFloat rect,Texture texture,Vector4? color = null,GUIStyle style = null)
		{
			var vector = new Vector4(
				rect.x/Screen.Width,
				rect.y/Screen.Height,
				(rect.x+rect.width)/Screen.Width,
				(rect.y+rect.height)/Screen.Height
			);
			if(Shader.activeShader.hasDefaultUniform[DefaultShaderUniforms.Color]) {
				var col = color ?? Vector4.One;
				GL.Uniform4(Shader.activeShader.defaultUniformIndex[DefaultShaderUniforms.Color],col.x,col.y,col.z,col.w);
			}
			
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D,texture.Id);
			
			int index = GL.GetAttribLocation(Shader.activeShader.Id,"uv");
			if(style==null || style.border.left==0) {
				GL.Begin(PrimitiveTypeGL.Quads);
					GL.VertexAttrib2(index,0.0f,0.0f);	GL.Vertex2(vector.x,1f-vector.y);
					GL.VertexAttrib2(index,0.0f,1.0f);	GL.Vertex2(vector.x,1f-vector.w);
					GL.VertexAttrib2(index,1.0f,1.0f);	GL.Vertex2(vector.z,1f-vector.w);
					GL.VertexAttrib2(index,1.0f,0.0f);	GL.Vertex2(vector.z,1f-vector.y);
				GL.End();
			}else{
				var textureSize = new Vector2(texture.Width,texture.Height);
				var center = new Vector4(
					vector.x+style.border.left/Screen.Width,vector.y+style.border.top/Screen.Height,
					vector.z-style.border.right/Screen.Width,vector.w-style.border.bottom/Screen.Height
				);
				var centerUV = new Vector4(
					style.border.left/textureSize.x,		style.border.top/textureSize.y,
					1f-style.border.right/textureSize.x,	1f-style.border.bottom/textureSize.y
				);
				GL.Begin(PrimitiveTypeGL.Quads);
				for(int y = 0;y<3;y++) {
					for(int x = 0;x<3;x++) {
						var vertex = new Vector4(
							x>0 ? x==1 ? center.x : center.z : vector.x,
							y>0 ? y==1 ? center.y : center.w : vector.y,
							x>0 ? x==1 ? center.z : vector.z : center.x,
							y>0 ? y==1 ? center.w : vector.w : center.y
						);
						var uv = new Vector4(
							x==0 ? 0f : x==1 ? centerUV.x : centerUV.z,
							y==0 ? 0f : y==1 ? centerUV.y : centerUV.w,
							x==0 ? centerUV.x : x==1 ? centerUV.z : 1f,
							y==0 ? centerUV.y : y==1 ? centerUV.w : 1f
						);
						GL.VertexAttrib2(index,uv.x,uv.y); GL.Vertex2(vertex.x,1f-vertex.y);
						GL.VertexAttrib2(index,uv.x,uv.w); GL.Vertex2(vertex.x,1f-vertex.w);
						GL.VertexAttrib2(index,uv.z,uv.w); GL.Vertex2(vertex.z,1f-vertex.w);
						GL.VertexAttrib2(index,uv.z,uv.y); GL.Vertex2(vertex.z,1f-vertex.y);
					}
				}
				GL.End();
			}
		}
		//TODO: Move this
		internal static void DrawString(Font font,float fontSize,RectFloat rect,string text,Vector4? color = null,TextAlignment alignment = TextAlignment.UpperLeft)
		{
			if(string.IsNullOrEmpty(text)) {
				return;
			}

			if(Shader.activeShader.hasDefaultUniform[DefaultShaderUniforms.Color]) {
				var col = color ?? Vector4.One;
				GL.Uniform4(Shader.activeShader.defaultUniformIndex[DefaultShaderUniforms.Color],col.x,col.y,col.z,col.w);
			}

			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D,font.texture.Id);
			GL.Uniform1(Shader.activeShader.defaultUniformIndex[DefaultShaderUniforms.MainTex],0);

			float scale = fontSize/font.charSize.y;
			var position = new Vector2(rect.x,rect.y);
			if(alignment==TextAlignment.UpperCenter || alignment==TextAlignment.MiddleCenter || alignment==TextAlignment.LowerCenter) {
				position.x += rect.width/2f-font.charSize.x*scale*text.Length/2f;
			}
			if(alignment==TextAlignment.MiddleLeft || alignment==TextAlignment.MiddleCenter || alignment==TextAlignment.MiddleRight) {
				position.y += rect.height/2f-fontSize/2f;
			}

			float xPos = position.x/Screen.Width;
			float yPos = position.y/Screen.Height;
			float width = font.charSize.x/Screen.Width*scale;
			float height = font.charSize.y/Screen.Height*scale;
			int uvAttrib = GL.GetAttribLocation(Shader.activeShader.Id,"uv");

			GL.Begin(PrimitiveTypeGL.Quads);

			for(int i = 0;i<text.Length;i++) {
				char c = text[i];
				if(!char.IsWhiteSpace(c) && font.charToUv.TryGetValue(c,out var uvs)) {
					GL.VertexAttrib2(uvAttrib,uvs[0]);	GL.Vertex2(xPos,		1f-yPos);
					GL.VertexAttrib2(uvAttrib,uvs[1]);	GL.Vertex2(xPos+width,	1f-yPos);
					GL.VertexAttrib2(uvAttrib,uvs[2]);	GL.Vertex2(xPos+width,	1f-yPos-height);
					GL.VertexAttrib2(uvAttrib,uvs[3]);	GL.Vertex2(xPos,		1f-yPos-height);
				}

				xPos += width;
			}

			GL.End();
		}
	}
}

