namespace Dissonance.Engine.Graphics
{
	public class Sprite : Renderer
	{
		public enum SpriteEffects
		{
			FlipHorizontally = 1,
			FlipVertically = 2
		}

		protected static readonly Bounds DefaultBounds = new Bounds(Vector3.Zero, Vector3.One);

		public static float DefaultPixelSize { get; set; } = 1f;

		protected static Mesh bufferMesh;

		public SpriteEffects spriteEffects;

		protected RectFloat sourceRectangle = RectFloat.Default;
		protected Vector4 sourceUV = new Vector4(0f, 0f, 1f, 1f);
		protected Vector4 vertices = new Vector4(-0.5f, -0.5f, 0.5f, 0.5f);
		protected Vector2 origin = new Vector2(0.5f, 0.5f);
		protected Vector2 sizeInPixels = Vector2.One;
		protected float pixelSize = DefaultPixelSize;
		protected bool setSize;
		protected Material material;

		public RectFloat SourceRectangle {
			get => sourceRectangle;
			set {
				sourceRectangle = value;
				sourceUV = new Vector4(value.x, value.y, value.Right, value.Bottom);
			}
		}
		public Vector2 FrameSize {
			get => sizeInPixels;
			set {
				sizeInPixels = value;
				setSize = true;

				RecalculateVertices();
			}
		}
		public Vector2 FrameSizeInUnits {
			get => sizeInPixels * pixelSize;
			set {
				sizeInPixels = value / pixelSize;
				setSize = true;

				RecalculateVertices();
			}
		}
		public Vector2 Origin {
			get => origin;
			set {
				origin = value;

				RecalculateVertices();
			}
		}
		public float PixelSize {
			get => pixelSize;
			set {
				pixelSize = value;

				RecalculateVertices();
			}
		}

		public override Material Material {
			get => material;
			set {
				material = value;

				if(!setSize && material != null && material.GetTexture("mainTex", out var texture)) {
					sizeInPixels = (Vector2)texture.Size;

					RecalculateVertices();
				}
			}
		}

		public override bool GetRenderData(Vector3 rendererPosition, Vector3 cameraPosition, out Material material, out Bounds bounds, out object renderObject)
		{
			material = this.material;

			bounds = DefaultBounds;
			renderObject = null;

			return true;
		}
		public override void Render(object renderObject)
		{
			Vector4 uvPoints = spriteEffects switch
			{
				SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically => new Vector4(sourceUV.z, sourceUV.w, sourceUV.x, sourceUV.y),
				SpriteEffects.FlipHorizontally => new Vector4(sourceUV.z, sourceUV.y, sourceUV.x, sourceUV.w),
				SpriteEffects.FlipVertically => new Vector4(sourceUV.x, sourceUV.w, sourceUV.z, sourceUV.y),
				_ => sourceUV
			};

			DrawUtils.DrawQuadUv0(vertices, uvPoints);
		}

		protected void RecalculateVertices()
		{
			float xSize = sizeInPixels.x * pixelSize;
			float ySize = sizeInPixels.y * pixelSize;

			float yOrigin = 1f - origin.y;

			vertices = new Vector4(
				-origin.x * xSize,
				-yOrigin * ySize,
				(1f - origin.x) * xSize,
				(1f - yOrigin) * ySize
			);
		}
	}
}
