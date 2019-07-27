using GameEngine.Graphics;

namespace GameEngine
{
	public enum LightType
	{
		Point,
		Directional,
		Spot
	}

	public class Light : Component
	{
		public float range = 16f;
		public float intensity = 1f;
		public Vector3 color = Vector3.one;
		public LightType type = LightType.Point;
		
		protected override void OnEnable() => Rendering.lightList.Add(this);
		protected override void OnDisable() => Rendering.lightList.Remove(this);
		protected override void OnDispose() => Rendering.lightList.Remove(this);
	}
}