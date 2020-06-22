using Dissonance.Engine.Core.Components;
using Dissonance.Engine.Structures;

namespace Dissonance.Engine.Graphics.Components
{
	public class Light2D : Component
	{
		public Vector3 color = Vector3.One;
		public float range = 16f;
		public float intensity = 1f;

		protected override void OnEnable() => Rendering.light2DList.Add(this);
		protected override void OnDisable() => Rendering.light2DList.Remove(this);
		protected override void OnDispose() => Rendering.light2DList.Remove(this);
	}
}