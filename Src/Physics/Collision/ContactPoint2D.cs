namespace Dissonance.Engine.Physics
{
	public struct ContactPoint2D
	{
		public Vector2 point;
		public Vector2 normal;
		public ICollider thisCollider;
		public ICollider otherCollider;
		public float separation;
	}
}
