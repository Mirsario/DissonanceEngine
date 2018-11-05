using System;
using System.Linq;
using System.Collections.Generic;
using GameEngine;

namespace Game
{
	//Entity that's bound to a chunk.
	public abstract class StaticEntity : Entity
	{
		public uint idInChunk;

		public virtual Mesh[] GetCollisionMeshes()
		{
			return null;
		}
	}
}
