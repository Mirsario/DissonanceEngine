namespace Dissonance.Engine
{
	public struct Bounds
	{
		public Vector3 min;
		public Vector3 max;

		public Vector3 Center => min + Extents;
		public Vector3 Extents => (max - min) * 0.5f;

		public Bounds(Vector3 min, Vector3 max)
		{
			this.min = min;
			this.max = max;
		}
	}
}

