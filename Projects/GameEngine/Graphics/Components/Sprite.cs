using GameEngine.Graphics;

namespace GameEngine
{
	public class Sprite : Renderer
	{
		public enum SpriteEffects
		{
			FlipHorizontally = 1,
			FlipVertically = 2
		}

		public SpriteEffects spriteEffects;

		internal Material material;

		public override Material Material {
			get => material;
			set => material = value;
		}

		protected override bool GetRenderData(Vector3 rendererPosition,Vector3 cameraPosition,out Mesh mesh,out Material material)
		{
			//TODO: Cache this?
			mesh = spriteEffects switch {
				SpriteEffects.FlipHorizontally|SpriteEffects.FlipVertically => PrimitiveMeshes.quadXYFlipped,
				SpriteEffects.FlipHorizontally => PrimitiveMeshes.quadXFlipped,
				SpriteEffects.FlipVertically => PrimitiveMeshes.quadXFlipped,
				_ => PrimitiveMeshes.quad
			};

			material = this.material;

			return true;
		}
	}
}