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
		public static Asset<Texture> TexDefaultInactive { get; set; }
		public static Asset<Texture> TexDefault { get; set; }
		public static Asset<Texture> TexDefaultHover { get; set; }
		public static Asset<Texture> TexDefaultActive { get; set; }

		internal static bool canDraw;

		private static Mesh textBufferMesh;

		protected override void Init()
		{
			TexDefaultInactive = Resources.Get<Texture>("BuiltInAssets/GUI/DefaultInactive.png");
			TexDefault = Resources.Get<Texture>("BuiltInAssets/GUI/Default.png");
			TexDefaultHover = Resources.Get<Texture>("BuiltInAssets/GUI/DefaultHover.png");
			TexDefaultActive = Resources.Get<Texture>("BuiltInAssets/GUI/DefaultActive.png");
			Skin = new GUISkin();

			textBufferMesh = new Mesh();
		}

		public static void Box(RectFloat rect, Vector4? color)
		{
			if (Skin.BoxStyle.TexNormal.TryGetOrRequestValue(out var boxStyleTexNormal)) {
				Draw(rect, boxStyleTexNormal, color, Skin.BoxStyle);
			}
		}

		public static bool Button(RectFloat rect, string text = null, bool active = true, Vector4? color = null)
		{
			bool hover = rect.Contains(InputEngine.MousePosition, true);
			bool anyPress = InputEngine.GetMouseButton(0);

			var style = Skin.ButtonStyle;
			var textureAsset = active ? hover ? anyPress ? style.TexActive : style.TexHover : style.TexNormal : style.TexInactive;

			if (textureAsset.TryGetOrRequestValue(out var texture)) {
				Draw(rect, texture, color, style);
			}

			if (!string.IsNullOrEmpty(text)) {
				var textRect = new RectFloat(
					rect.X + style.Border.Left,
					rect.Y + style.Border.Top,
					rect.Width - style.Border.Left - style.Border.Right,
					rect.Height - style.Border.Top - style.Border.Bottom
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
				rect.X / Screen.Width,
				rect.Y / Screen.Height,
				(rect.X + rect.Width) / Screen.Width,
				(rect.Y + rect.Height) / Screen.Height
			);

			if (Shader.ActiveShader.hasDefaultUniform[DefaultShaderUniforms.Color]) {
				var col = color ?? Vector4.One;

				GL.Uniform4(Shader.ActiveShader.defaultUniformIndex[DefaultShaderUniforms.Color], col.X, col.Y, col.Z, col.W);
			}

			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, texture.Id);

			if (style == null || style.Border.Left == 0) {
				DrawUtils.DrawQuadUv0(
					new Vector4(vector.X, 1f - vector.W, vector.Z, 1f - vector.Y),
					new Vector4(0f, 0f, 1f, 1f)
				);

				return;
			}

			var textureSize = new Vector2(texture.Width, texture.Height);
			var center = new Vector4(
				vector.X + style.Border.Left / Screen.Width, vector.Y + style.Border.Top / Screen.Height,
				vector.Z - style.Border.Right / Screen.Width, vector.W - style.Border.Bottom / Screen.Height
			);
			var centerUV = new Vector4(
				style.Border.Left / textureSize.X, style.Border.Top / textureSize.Y,
				1f - style.Border.Right / textureSize.X, 1f - style.Border.Bottom / textureSize.Y
			);

			for (int y = 0; y < 3; y++) {
				for (int x = 0; x < 3; x++) {
					Vector4 vertices, uv;

					switch (x) {
						default:
							vertices.X = vector.X;
							vertices.Z = center.X;
							uv.X = 0f;
							uv.Z = centerUV.X;
							break;
						case 1:
							vertices.X = center.X;
							vertices.Z = center.Z;
							uv.X = centerUV.X;
							uv.Z = centerUV.Z;
							break;
						case 2:
							vertices.X = center.Z;
							vertices.Z = vector.Z;
							uv.X = centerUV.Z;
							uv.Z = 1f;
							break;
					}

					switch (y) {
						default:
							vertices.Y = 1f - vector.Y;
							vertices.W = 1f - center.Y;
							uv.Y = centerUV.Y;
							uv.W = 0f;
							break;
						case 1:
							vertices.Y = 1f - center.Y;
							vertices.W = 1f - center.W;
							uv.Y = centerUV.W;
							uv.W = centerUV.Y;
							break;
						case 2:
							vertices.Y = 1f - center.W;
							vertices.W = 1f - vector.W;
							uv.Y = 1f;
							uv.W = centerUV.W;
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

			if (Shader.ActiveShader.hasDefaultUniform[DefaultShaderUniforms.Color]) {
				var col = color ?? Vector4.One;

				GL.Uniform4(Shader.ActiveShader.defaultUniformIndex[DefaultShaderUniforms.Color], col.X, col.Y, col.Z, col.W);
			}

			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, font.Texture.TryGetOrRequestValue(out var tex) ? tex.Id : 0);
			GL.Uniform1(Shader.ActiveShader.defaultUniformIndex[DefaultShaderUniforms.MainTex], 0);

			float scale = fontSize / font.CharSize.Y;
			var position = new Vector2(rect.X, rect.Y);
			
			if (alignment == TextAlignment.UpperCenter || alignment == TextAlignment.MiddleCenter || alignment == TextAlignment.LowerCenter) {
				position.X += rect.Width / 2f - font.CharSize.X * scale * text.Length / 2f;
			}

			if (alignment == TextAlignment.MiddleLeft || alignment == TextAlignment.MiddleCenter || alignment == TextAlignment.MiddleRight) {
				position.Y += rect.Height / 2f - fontSize / 2f;
			}

			float xPos = position.X / Screen.Width;
			float yPos = position.Y / Screen.Height;
			float width = font.CharSize.X / Screen.Width * scale;
			float height = font.CharSize.Y / Screen.Height * scale;

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

