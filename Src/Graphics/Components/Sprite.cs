using System;
using Dissonance.Engine.IO;

namespace Dissonance.Engine.Graphics
{
	public struct Sprite
	{
		[Flags]
		public enum SpriteEffects
		{
			FlipHorizontally = 1,
			FlipVertically = 2
		}

		public static float DefaultPixelSize { get; set; } = 1f;

		internal Vector4 vertices;
		internal bool verticesNeedRecalculation;

		private Vector2 origin;
		private float pixelSize;
		private Vector2 sizeInPixels;
		private Asset<Material> material;

		public RectFloat SourceRectangle { get; set; }
		public SpriteEffects Effects { get; set; }
		public Bounds AABB { get; private set; }

		public Vector2 FrameSize {
			get => sizeInPixels;
			set {
				sizeInPixels = value;
				verticesNeedRecalculation = true;
			}
		}
		public Vector2 FrameSizeInUnits {
			get => sizeInPixels * pixelSize;
			set {
				sizeInPixels = value / pixelSize;
				verticesNeedRecalculation = true;
			}
		}
		public Vector2 Origin {
			get => origin;
			set {
				origin = value;
				verticesNeedRecalculation = true;
			}
		}
		public float PixelSize {
			get => pixelSize;
			set {
				pixelSize = value;
				verticesNeedRecalculation = true;
			}
		}
		public Asset<Material> Material {
			get => material;
			set {
				material = value;

				//TODO: Maybe move into a system?
				//if (material != null && material.GetTexture("mainTex", out var texture)) {
				//	sizeInPixels = (Vector2)texture.Size;
				//	verticesNeedRecalculation = true;
				//}
			}
		}

		public Sprite(Asset<Material> material) : this()
		{
			vertices = new Vector4(-0.5f, -0.5f, 0.5f, 0.5f);
			origin = new Vector2(0.5f, 0.5f);
			sizeInPixels = Vector2.One;
			pixelSize = DefaultPixelSize;

			SourceRectangle = RectFloat.Default;
			Material = material;
		}
	}
}
