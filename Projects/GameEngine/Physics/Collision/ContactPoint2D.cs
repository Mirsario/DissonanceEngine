namespace GameEngine.Physics
{
	public struct ContactPoint2D
	{
		public Vector2 point;
		public Vector2 normal;
		public Collider thisCollider;
		public Collider otherCollider;
		public float separation;
	}
}
