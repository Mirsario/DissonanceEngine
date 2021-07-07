namespace Dissonance.Engine.Graphics
{
	public struct Light2D
	{
		public float Range { get; set; }
		public float Intensity { get; set; }
		public Vector3 Color { get; set; }

		public Light2D(Vector3 color, float range = 16f, float intensity = 1f)
		{
			Color = color;
			Range = range;
			Intensity = intensity;
		}
	}
}
