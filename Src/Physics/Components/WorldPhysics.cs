using BulletSharp;

namespace Dissonance.Engine.Physics
{
	public struct WorldPhysics : IComponent
	{
		public static readonly WorldPhysics Default = new() {
			Gravity = new Vector3(0f, -9.81f, 0f)
		};

		public Vector3 Gravity { get; set; }

		internal DiscreteDynamicsWorld PhysicsWorld { get; set; }
	}
}
