using System.Collections.Generic;

namespace Dissonance.Engine.Graphics;

public struct LightingPassData : IRenderComponent
{
	public struct LightData
	{
		public Light.LightType Type;
		public Matrix4x4 Matrix;
		public float Intensity;
		public float? Range;
		public Vector3 Color;
		public Vector3? Position;
		public Vector3? Direction;
	}

	public List<LightData> Lights { get; private set; }

	public void Reset()
	{
		if (Lights == null) {
			Lights = new List<LightData>();
		} else {
			Lights.Clear();
		}
	}
}
