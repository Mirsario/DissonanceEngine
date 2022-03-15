namespace Dissonance.Engine.Graphics
{
	public struct Light
	{
		public enum LightType
		{
			Point,
			Directional,
		}

		public float Range { get; set; } = 16f;
		public float Intensity { get; set; } = 1f;
		public Vector3 Color { get; set; } = Vector3.One;
		public LightType Type { get; set; } = LightType.Point;

		public Light() { }
	}
}
