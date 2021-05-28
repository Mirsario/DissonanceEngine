using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dissonance.Engine.Physics
{
	public sealed class RigidbodySystem : GameSystem
	{
		private EntitySet entities;

		public override void Initialize()
		{
			entities = World.GetEntitySet(e => e.Has<Rigidbody>() && e.Has<Transform>());
		}

		public override void FixedUpdate()
		{
			foreach(var entity in entities.ReadEntities()) {
				ref var rigidbody = ref entity.Get<Rigidbody>();

				
			}
		}
	}
}
