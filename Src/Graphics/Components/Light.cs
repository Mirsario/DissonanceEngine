namespace Dissonance.Engine.Graphics
{
	public class Light : Component
	{
		public enum Type
		{
			Point,
			Directional,
			Spot
		}

		public float range = 16f;
		public float intensity = 1f;
		public Vector3 color = Vector3.One;
		public Type type = Type.Point;
	}
}
