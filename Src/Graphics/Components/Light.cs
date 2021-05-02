namespace Dissonance.Engine.Graphics
{
	public struct Light : IComponent
	{
		public enum LightType
		{
			Point,
			Directional,
			Spot
		}

		public float Range { get; set; }
		public float Intensity { get; set; }
		public Vector3 Color { get; set; }
		public LightType Type { get; set; }

		public Light(LightType type, Vector3 color, float range = 16f, float intensity = 1f)
		{
			Type = type;
			Color = color;
			Range = range;
			Intensity = intensity;
		}
	}
}
