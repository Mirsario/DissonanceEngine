using Dissonance.Engine.Core.Components;
using Dissonance.Engine.Structures;

namespace Dissonance.Engine.Graphics.Components
{
	public class Light2D : Component
	{
		public Vector3 color = Vector3.One;
		public float range = 16f;
		public float intensity = 1f;
	}
}