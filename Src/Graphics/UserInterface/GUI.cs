using Dissonance.Framework.Graphics;
using System.Linq;
using Dissonance.Engine.IO;
using Dissonance.Engine.Input;

namespace Dissonance.Engine.Graphics
{
	[ModuleAutoload(DisablingGameFlags = GameFlags.NoGraphics)]
	[ModuleDependency(typeof(Resources), typeof(Rendering))]
	public sealed class GUI : EngineModule
	{
		public static Font Font { get; set; }
		public static GUISkin Skin { get; set; }
		public static Texture TexDefaultInactive { get; set; }
		public static Texture TexDefault { get; set; }
		public static Texture TexDefaultHover { get; set; }
		public static Texture TexDefaultActive { get; set; }

		internal static bool canDraw;

		private static Mesh textBufferMesh;

		protected override void Init()
		{
			TexDefaultInactive = Resources.Import<Texture>("BuiltInAssets/GUI/DefaultInactive.png");
			TexDefault = Resources.Import<Texture>("BuiltInAssets/GUI/Default.png");
			TexDefaultHover = Resources.Import<Texture>("BuiltInAssets/GUI/DefaultHover.png");
			TexDefaultActive = Resources.Import<Texture>("BuiltInAssets/GUI/DefaultActive.png");
			Skin = new GUISkin();

			textBufferMesh = new Mesh();
		}

		public static void Box(RectFloat rect, Vector4? color)
		{
			Draw(rect, Skin.BoxStyle.TexNormal, color, Skin.BoxStyle);
		}

		public static bool Button(RectFloat rect, string text = null, bool active = true, Vector4? color = null)
		{
			bool hover = rect.Contains(InputEngine.MousePosition, true);
			bool anyPress = InputEngine.GetMouseButton(0);

			var style = Skin.ButtonStyle;
			var tex = active ? hover ? anyPress ? style.TexActive : style.TexHover : style.TexNormal : style.TexInactive;

			Draw(rect, tex, color, style);

			if (!string.IsNullOrEmpty(text)) {
				var textRect = new RectFloat(
					rect.x + style.Border.left,
					rect.y + style.Border.top,
					rect.width - style.Border.left - style.Border.right,
					rect.height - style.Border.top - style.Border.bottom
				);

				DrawString(Font, style.FontSize, textRect, text, alignment: style.TextAlignment);
			}

			return active && hover && InputEngine.GetMouseButtonUp(0) && rect.Contains(InputEngine.MousePosition, true);
		}

		public static void DrawText(RectFloat rect, string text, Vector4? color = null, TextAlignment alignment = TextAlignment.UpperLeft, float fontSize = -1)
		{
			if (fontSize == -1) {
				fontSize = Font.Size;
			}

			DrawString(Font, fontSize, rect, text, color, alignment);
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

			if (Shader.ActiveShader.TryGetUniformLocation("color", out int colorLocation)) {
				var col = color ?? Vector4.One;

				GL.Uniform4(colorLocation, col.x, col.y, col.z, col.w);
			}

			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, texture.Id);

			if (style == null || style.Border.left == 0) {
				DrawUtils.DrawQuadUv0(
					new Vector4(vector.x, 1f - vector.w, vector.z, 1f - vector.y),
					new Vector4(0f, 0f, 1f, 1f)
				);

				return;
			}

			var textureSize = new Vector2(texture.Width, texture.Height);
			var center = new Vector4(
				vector.x + style.Border.left / Screen.Width, vector.y + style.Border.top / Screen.Height,
				vector.z - style.Border.right / Screen.Width, vector.w - style.Border.bottom / Screen.Height
			);
			var centerUV = new Vector4(
				style.Border.left / textureSize.x, style.Border.top / textureSize.y,
				1f - style.Border.right / textureSize.x, 1f - style.Border.bottom / textureSize.y
			);

			for (int y = 0; y < 3; y++) {
				for (int x = 0; x < 3; x++) {
					Vector4 vertices, uv;

					switch (x) {
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

					switch (y) {
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
			if (string.IsNullOrEmpty(text)) {
				return;
			}

			// This uniform code is weird
			if (!Shader.ActiveShader.TryGetUniformLocation("mainTex", out int mainTexLocation)) {
				return;
			}

			if (Shader.ActiveShader.TryGetUniformLocation("color", out int colorLocation)) {
				var col = color ?? Vector4.One;

				GL.Uniform4(colorLocation, col.x, col.y, col.z, col.w);
			}

			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, font.Texture.Id);
			GL.Uniform1(mainTexLocation, 0);

			float scale = fontSize / font.CharSize.y;
			var position = new Vector2(rect.x, rect.y);
			
			if (alignment == TextAlignment.UpperCenter || alignment == TextAlignment.MiddleCenter || alignment == TextAlignment.LowerCenter) {
				position.x += rect.width / 2f - font.CharSize.x * scale * text.Length / 2f;
			}

			if (alignment == TextAlignment.MiddleLeft || alignment == TextAlignment.MiddleCenter || alignment == TextAlignment.MiddleRight) {
				position.y += rect.height / 2f - fontSize / 2f;
			}

			float xPos = position.x / Screen.Width;
			float yPos = position.y / Screen.Height;
			float width = font.CharSize.x / Screen.Width * scale;
			float height = font.CharSize.y / Screen.Height * scale;

			int numCharacters = text.Count(c => !char.IsWhiteSpace(c) && font.CharToUv.ContainsKey(c));

			textBufferMesh.Vertices = new Vector3[numCharacters * 4];
			textBufferMesh.Uv0 = new Vector2[numCharacters * 4];
			textBufferMesh.Indices = new uint[numCharacters * 6];

			uint vertex = 0;
			uint triangle = 0;

			for (int i = 0; i < text.Length; i++) {
				char c = text[i];

				if (!char.IsWhiteSpace(c) && font.CharToUv.TryGetValue(c, out var uvs)) {
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

