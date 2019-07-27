using GameEngine;

namespace SurvivalGame
{
	public abstract class CameraController : GameObject
	{
		public const float MinLockedPitch = -89.99f;
		public const float MaxLockedPitch = 89.99f;
		
		public Entity entity;
		public Camera camera;

		public abstract Vector3 Rotation { get; set; }
		public abstract Vector3 Direction { get; set; }
	}
}