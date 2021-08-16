namespace Dissonance.Engine.Physics
{
	public struct Rigidbody
	{
		public static readonly Rigidbody Default = new() {
			Mass = 1f
		};

		internal bool updateShapes;
		internal bool updateMass;
		internal bool ownsCollisionShape;
		internal bool? wasKinematic;
		internal float lastMass;

		public bool IsKinematic { get; set; }
		public float Mass { get; set; }
		public Vector3 Velocity { get; set; }
		public Vector3 AngularVelocity { get; set; }
		public Vector3 AngularFactor { get; set; }
	}
}
