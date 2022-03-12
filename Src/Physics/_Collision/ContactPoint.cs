namespace Dissonance.Engine.Physics
{
	public readonly struct ContactPoint
	{
		public readonly Vector3 Point;
		public readonly Vector3 Normal;
		public readonly float Distance;

		public ContactPoint(Vector3 point, Vector3 normal, float distance)
		{
			Point = point;
			Normal = normal;
			Distance = distance;
		}
	}
}
