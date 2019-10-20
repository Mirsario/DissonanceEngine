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

		protected Material material;

		public override Material Material {
			get => material;
			set => material = value;
		}

		public override bool GetRenderData(Vector3 rendererPosition,Vector3 cameraPosition,out Material material,out Bounds bounds,out object renderObject)
		{
			var mesh = spriteEffects switch {
				SpriteEffects.FlipHorizontally|SpriteEffects.FlipVertically => PrimitiveMeshes.quadXYFlipped,
				SpriteEffects.FlipHorizontally => PrimitiveMeshes.quadXFlipped,
				SpriteEffects.FlipVertically => PrimitiveMeshes.quadXFlipped,
				_ => PrimitiveMeshes.quad
			};

			material = this.material;
			bounds = mesh.bounds;
			renderObject = mesh;

			return true;
		}
		public override void Render(object renderObject) => ((Mesh)renderObject).DrawMesh();
	}
}