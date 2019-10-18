using GameEngine;
using GameEngine.Graphics;
using GameEngine.Physics;

namespace SurvivalGame
{
	//Entity that's bound to a chunk.
	public abstract class StaticEntity : Entity
	{
		public Chunk chunk;
		public uint idInChunk;

		public abstract CollisionMesh CollisionMesh { get; }
		public abstract (Mesh mesh, Material material)[] RendererData { get; }

		public override void OnInit()
		{
			ChunkCheck();
		}

		public void ChunkCheck()
		{
			var pos = Transform.Position;
			Chunk newChunk = world.GetChunkAt(pos.x,pos.z);

			if(chunk==newChunk) {
				return;
			}

			if(chunk!=null) {
				lock(chunk.staticEntities) {
					chunk.staticEntities.Remove(this);
				}
			}

			chunk = newChunk;

			if(chunk!=null) {
				lock(chunk.staticEntities) {
					chunk.staticEntities.Add(this);
				}
			}
		}
	}
}
