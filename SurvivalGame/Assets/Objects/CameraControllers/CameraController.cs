using GameEngine;

namespace SurvivalGame
{
	public abstract class CameraController : GameObject
	{
		public const float minLockedPitch = -89.99f;
		public const float maxLockedPitch = 89.99f;
		
		public Entity entity;
		public Camera camera;

		public abstract Vector3 Rotation { get; set; }
		public abstract Vector3 Direction { get; set; }
	}
}