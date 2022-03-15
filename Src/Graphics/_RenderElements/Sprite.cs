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

		internal Vector4 vertices = new(-0.5f, -0.5f, 0.5f, 0.5f);
		internal bool verticesNeedRecalculation = true;

		private Vector2 origin = new(0.5f, 0.5f);
		private float pixelSize = DefaultPixelSize;
		private Vector2 sizeInPixels = Vector2.One;

		public Asset<Material> Material { get; set; } = default;
		public RectFloat SourceRectangle { get; set; } = RectFloat.Default;
		public SpriteEffects Effects { get; set; } = default;
		public Bounds AABB { get; private set; } = default;

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

		//TODO: Maybe move into a system?
		//if (material != null && material.GetTexture("mainTex", out var texture)) {
		//	sizeInPixels = (Vector2)texture.Size;
		//	verticesNeedRecalculation = true;
		//}

		public Sprite() { }
	}
}
