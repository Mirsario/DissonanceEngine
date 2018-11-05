using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using PrimitiveTypeGL = OpenTK.Graphics.OpenGL.PrimitiveType;

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
		
		public static void Box(Rect rect)
		{
			Draw(rect,skin.boxStyle.texNormal,skin.boxStyle);
		}
		public static bool Button(Rect rect,string text = null,bool active = true)
		{
			bool hover = rect.Contains(Input.MousePosition,true);
			bool anyPress = Input.GetMouseButton(0);
			var style = skin.buttonStyle;
			var tex = active ? hover ? anyPress ? style.texActive : style.texHover : style.texNormal : style.texInactive;
			Draw(rect,tex,style);
			if(text!=null && text.Length>0) {
				var textRect = new Rect(
					rect.x+style.border.left,
					rect.y+style.border.top,
					rect.width-style.border.left-style.border.right,
					rect.height-style.border.top-style.border.bottom
				);
				Graphics.DrawString(font,style.fontSize,textRect,text,style.textAlignment);
			}
			
			return active && hover && Input.GetMouseButtonUp(0) && rect.Contains(Input.MousePosition,true);
		}
		public static void DrawText(Rect rect,string text,TextAlignment alignment = TextAlignment.UpperLeft,float fontSize = -1)
		{
			if(fontSize==-1) {
				fontSize = font.size;
			}
			Graphics.DrawString(font,fontSize,rect,text,alignment);
		}
		public static void DrawTexture(Rect rect,Texture texture)
		{
			Draw(rect,texture,null);
		}
		internal static void Draw(Rect rect,Texture texture,GUIStyle style = null)
		{
			var vector = new Vector4(
				rect.x/Graphics.ScreenWidth,
				rect.y/Graphics.ScreenHeight,
				(rect.x+rect.width)/Graphics.ScreenWidth,
				(rect.y+rect.height)/Graphics.ScreenHeight
			);
			
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D,texture.Id);
			GL.Uniform1(GL.GetUniformLocation(Graphics.GUIShader.program,"mainTex2"),0);
			
			int index = GL.GetAttribLocation(Graphics.GUIShader.program,"uv");
			if(style==null || style.border.left==0) {
				GL.Begin(PrimitiveTypeGL.Quads);
					GL.Vertex2(vector.x,1f-vector.y);
					GL.VertexAttrib2(index,0.0f,0.0f);//1
					GL.Vertex2(vector.x,1f-vector.w);
					GL.VertexAttrib2(index,0.0f,1.0f);//2
					GL.Vertex2(vector.z,1f-vector.w);
					GL.VertexAttrib2(index,1.0f,1.0f);//3
					GL.Vertex2(vector.z,1f-vector.y);
					GL.VertexAttrib2(index,1.0f,0.0f);//4
				GL.End();
			}else{
				var textureSize = new Vector2(texture.Width,texture.Height);
				var center = new Vector4(
					vector.x+style.border.left/Graphics.ScreenWidth,vector.y+style.border.top/Graphics.ScreenHeight,
					vector.z-style.border.right/Graphics.ScreenWidth,vector.w-style.border.bottom/Graphics.ScreenHeight
				);
				var centerUV = new Vector4(
					style.border.left/textureSize.x,		style.border.top/textureSize.y,
					1f-style.border.right/textureSize.x,	1f-style.border.bottom/textureSize.y
				);
				GL.Begin(PrimitiveTypeGL.Quads);
				for(int y=0;y<3;y++) {
					for(int x=0;x<3;x++) {
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
						GL.VertexAttrib2(index,uv.x,uv.y);//1
						GL.Vertex2(vertex.x,1f-vertex.y);
						GL.VertexAttrib2(index,uv.x,uv.w);//2
						GL.Vertex2(vertex.x,1f-vertex.w);
						GL.VertexAttrib2(index,uv.z,uv.w);//3
						GL.Vertex2(vertex.z,1f-vertex.w);
						GL.VertexAttrib2(index,uv.z,uv.y);//4
						GL.Vertex2(vertex.z,1f-vertex.y);
					}
				}
				GL.End();
			}
		}
	}
}

