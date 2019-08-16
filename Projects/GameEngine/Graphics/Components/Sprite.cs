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
		
		protected override void OnDispose()
		{
			Rendering.rendererList.Remove(this);
			Materials = null;
		}
		protected override void OnInit()
		{
			Materials = new Material[1];
			Material = Material.defaultMat;
		}
		
		//TODO: is dis slew
		protected override Mesh GetRenderMesh(Vector3 rendererPosition,Vector3 cameraPosition)
		{
			bool flipX = ((int)spriteEffects&1)==1;
			bool flipY = ((int)spriteEffects&2)==2;
			if(flipX && flipY) {
				return PrimitiveMeshes.quadXYFlipped;
			}
			if(flipX) {
				return PrimitiveMeshes.quadXFlipped;
			}
			if(flipY) {
				return PrimitiveMeshes.quadYFlipped;
			}
			return PrimitiveMeshes.quad;
		}
	}
}