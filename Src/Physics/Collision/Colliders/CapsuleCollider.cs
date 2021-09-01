using BulletSharp;

namespace Dissonance.Engine.Physics
{
	public struct CapsuleCollider
	{
		public static readonly CapsuleCollider Default = new(1f, 2f);

		internal CapsuleShape capsuleShape;
		internal bool needsUpdate;

		private float radius;
		private float height;

		public float Radius {
			get => radius;
			set {
				if (value != radius) {
					radius = value;
					needsUpdate = true;
				}
			}
		}

		public float Height {
			get => height;
			set {
				if (value != height) {
					height = value;
					needsUpdate = true;
				}
			}
		}

		public CapsuleCollider(float radius, float height) : this()
		{
			this.radius = radius;
			this.height = height;
			needsUpdate = true;
		}
	}
}
