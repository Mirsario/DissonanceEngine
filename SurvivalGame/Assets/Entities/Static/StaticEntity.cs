using System;
using System.Linq;
using System.Collections.Generic;
using GameEngine;

namespace Game
{
	//Entity that's bound to a chunk.
	public abstract class StaticEntity : Entity
	{
		public Chunk chunk;
		public uint idInChunk;

		public virtual Mesh[] GetCollisionMeshes() => null;

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
			chunk?.staticEntities.Remove(this);
			chunk = newChunk;
			newChunk?.staticEntities.Add(this);
		}
	}
}
