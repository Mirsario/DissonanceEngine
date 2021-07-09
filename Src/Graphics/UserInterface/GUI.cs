using Dissonance.Framework.Graphics;
using System.Linq;
using Dissonance.Engine.IO;
using Dissonance.Engine.Input;

namespace Dissonance.Engine.Graphics
{
	[Autoload(DisablingGameFlags = GameFlags.NoGraphics)]
	[ModuleDependency(typeof(Resources), typeof(Rendering))]
	public sealed class GUI : EngineModule
	{
		public static Font font;
		public static GUISkin skin;
		public static Texture texDefaultInactive;
		public static Texture texDefault;
		public static Texture texDefaultHover;
		public static Texture texDefaultActive;

		internal static bool canDraw;

		private static Mesh textBufferMesh;

		protected override void Init()
		{
			texDefaultInactive = Resources.Import<Texture>("BuiltInAssets/GUI/DefaultInactive.png");
			texDefault = Resources.Import<Texture>("BuiltInAssets/GUI/Default.png");
			texDefaultHover = Resources.Import<Texture>("BuiltInAssets/GUI/DefaultHover.png");
			texDefaultActive = Resources.Import<Texture>("BuiltInAssets/GUI/DefaultActive.png");
			skin = new GUISkin();

			textBufferMesh = new Mesh();
		}

		public static void Box(RectFloat rect, Vector4? color)
		{
			Draw(rect, skin.boxStyle.texNormal, color, skin.boxStyle);
		}

		public static bool Button(RectFloat rect, string text = null, bool active = true, Vector4? color = null)
		{
			bool hover = rect.Contains(InputEngine.MousePosition, true);
			bool anyPress = InputEngine.GetMouseButton(0);

			var style = skin.buttonStyle;
			var tex = active ? hover ? anyPress ? style.texActive : style.texHover : style.texNormal : style.texInactive;

			Draw(rect, tex, color, style);

			if(!string.IsNullOrEmpty(text)) {
				var textRect = new RectFloat(
					rect.x + style.border.left,
					rect.y + style.border.top,
					rect.width - style.border.left - style.border.right,
					rect.height - style.border.top - style.border.bottom
				);

				DrawString(font, style.fontSize, textRect, text, alignment: style.textAlignment);
			}

			return active && hover && InputEngine.GetMouseButtonUp(0) && rect.Contains(InputEngine.MousePosition, true);
		}

		public static void DrawText(RectFloat rect, string text, Vector4? color = null, TextAlignment alignment = TextAlignment.UpperLeft, float fontSize = -1)
		{
			if(fontSize == -1) {
				fontSize = font.size;
			}

			DrawString(font, fontSize, rect, text, color, alignment);
		}

		public static void DrawTexture(RectFloat rect, Texture texture, Vector4? color = null)
		{
			Draw(rect, texture, color);
		}

		internal static void Draw(RectFloat rect, Texture texture, Vector4? color = null, GUIStyle style = null)
		{
			var vector = new Vector4(
				rect.x / Screen.Width,
				rect.y / Screen.Height,
				(rect.x + rect.width) / Screen.Width,
				(rect.y + rect.height) / Screen.Height
			);

			if(Shader.ActiveShader.hasDefaultUniform[DefaultShaderUniforms.Color]) {
				var col = color ?? Vector4.One;

				GL.Uniform4(Shader.ActiveShader.defaultUniformIndex[DefaultShaderUniforms.Color], col.x, col.y, col.z, col.w);
			}

			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, texture.Id);

			if(style == null || style.border.left == 0) {
				DrawUtils.DrawQuadUv0(
					new Vector4(vector.x, 1f - vector.w, vector.z, 1f - vector.y),
					new Vector4(0f, 0f, 1f, 1f)
				);

				return;
			}

			var textureSize = new Vector2(texture.Width, texture.Height);
			var center = new Vector4(
				vector.x + style.border.left / Screen.Width, vector.y + style.border.top / Screen.Height,
				vector.z - style.border.right / Screen.Width, vector.w - style.border.bottom / Screen.Height
			);
			var centerUV = new Vector4(
				style.border.left / textureSize.x, style.border.top / textureSize.y,
				1f - style.border.right / textureSize.x, 1f - style.border.bottom / textureSize.y
			);

			for(int y = 0; y < 3; y++) {
				for(int x = 0; x < 3; x++) {
					Vector4 vertices, uv;

					switch(x) {
						default:
							vertices.x = vector.x;
							vertices.z = center.x;
							uv.x = 0f;
							uv.z = centerUV.x;
							break;
						case 1:
							vertices.x = center.x;
							vertices.z = center.z;
							uv.x = centerUV.x;
							uv.z = centerUV.z;
							break;
						case 2:
							vertices.x = center.z;
							vertices.z = vector.z;
							uv.x = centerUV.z;
							uv.z = 1f;
							break;
					}

					switch(y) {
						default:
							vertices.y = 1f - vector.y;
							vertices.w = 1f - center.y;
							uv.y = centerUV.y;
							uv.w = 0f;
							break;
						case 1:
							vertices.y = 1f - center.y;
							vertices.w = 1f - center.w;
							uv.y = centerUV.w;
							uv.w = centerUV.y;
							break;
						case 2:
							vertices.y = 1f - center.w;
							vertices.w = 1f - vector.w;
							uv.y = 1f;
							uv.w = centerUV.w;
							break;
					}

					DrawUtils.DrawQuadUv0(vertices, uv);
				}
			}
		}

		//TODO: Move this
		internal static void DrawString(Font font, float fontSize, RectFloat rect, string text, Vector4? color = null, TextAlignment alignment = TextAlignment.UpperLeft)
		{
			if(string.IsNullOrEmpty(text)) {
				return;
			}

			if(Shader.ActiveShader.hasDefaultUniform[DefaultShaderUniforms.Color]) {
				var col = color ?? Vector4.One;

				GL.Uniform4(Shader.ActiveShader.defaultUniformIndex[DefaultShaderUniforms.Color], col.x, col.y, col.z, col.w);
			}

			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, font.texture.Id);
			GL.Uniform1(Shader.ActiveShader.defaultUniformIndex[DefaultShaderUniforms.MainTex], 0);

			float scale = fontSize / font.charSize.y;
			var position = new Vector2(rect.x, rect.y);
			
			if(alignment == TextAlignment.UpperCenter || alignment == TextAlignment.MiddleCenter || alignment == TextAlignment.LowerCenter) {
				position.x += rect.width / 2f - font.charSize.x * scale * text.Length / 2f;
			}

			if(alignment == TextAlignment.MiddleLeft || alignment == TextAlignment.MiddleCenter || alignment == TextAlignment.MiddleRight) {
				position.y += rect.height / 2f - fontSize / 2f;
			}

			float xPos = position.x / Screen.Width;
			float yPos = position.y / Screen.Height;
			float width = font.charSize.x / Screen.Width * scale;
			float height = font.charSize.y / Screen.Height * scale;

			int numCharacters = text.Count(c => !char.IsWhiteSpace(c) && font.charToUv.ContainsKey(c));

			textBufferMesh.Vertices = new Vector3[numCharacters * 4];
			textBufferMesh.Uv0 = new Vector2[numCharacters * 4];
			textBufferMesh.Indices = new uint[numCharacters * 6];

			uint vertex = 0;
			uint triangle = 0;

			for(int i = 0; i < text.Length; i++) {
				char c = text[i];

				if(!char.IsWhiteSpace(c) && font.charToUv.TryGetValue(c, out var uvs)) {
					textBufferMesh.Vertices[vertex] = new Vector3(xPos, 1f - yPos, 0f);
					textBufferMesh.Vertices[vertex + 1] = new Vector3(xPos + width, 1f - yPos, 0f);
					textBufferMesh.Vertices[vertex + 2] = new Vector3(xPos + width, 1f - yPos - height, 0f);
					textBufferMesh.Vertices[vertex + 3] = new Vector3(xPos, 1f - yPos - height, 0f);

					textBufferMesh.Uv0[vertex] = uvs[0];
					textBufferMesh.Uv0[vertex + 1] = uvs[1];
					textBufferMesh.Uv0[vertex + 2] = uvs[2];
					textBufferMesh.Uv0[vertex + 3] = uvs[3];

					textBufferMesh.Indices[triangle++] = vertex + 2;
					textBufferMesh.Indices[triangle++] = vertex + 1;
					textBufferMesh.Indices[triangle++] = vertex;
					textBufferMesh.Indices[triangle++] = vertex + 3;
					textBufferMesh.Indices[triangle++] = vertex + 2;
					textBufferMesh.Indices[triangle++] = vertex;

					vertex += 4;
				}

				xPos += width;
			}

			textBufferMesh.Apply();
			textBufferMesh.Render();
		}
	}
}

