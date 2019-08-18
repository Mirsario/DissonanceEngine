//#define NO_CHUNK_LODS

using GameEngine;
using GameEngine.Physics;
using static SurvivalGame.Chunk;

namespace SurvivalGame
{
	public class ChunkRenderer : GameObject
	{
		public Chunk chunk;
		public Vector2Int position;
		public MeshRenderer renderer;
		public MeshRenderer[] grassRenderers;

		public override void OnInit()
		{
			layer = Layers.GetLayerIndex("World");

			Transform.Position = new Vector3(position.x*ChunkWorldSize,0f,position.y*ChunkWorldSize);
			
			renderer = AddComponent<MeshRenderer>(c => {
				c.Material = chunk.Material;
				c.Mesh = chunk.GetMesh();
			});
		}
		public override void OnDispose() => renderer.Dispose();

		public static ChunkRenderer Create(Chunk chunk,int x,int y)
		{
			var renderer = Instantiate<ChunkRenderer>(init:false);
			renderer.chunk = chunk;
			renderer.position = new Vector2Int(x,y);
			renderer.Init();

			return renderer;
		}
	}
}
