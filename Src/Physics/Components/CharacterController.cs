/*using BulletSharp;

namespace Dissonance.Engine.Physics
{
	public class CharacterController : Component
	{
		public KinematicCharacterController charController;
		internal CapsuleShape capsule;

		protected override void OnEnable()
		{
			capsule = new CapsuleShape(0.5f,2f);

			var obj = new PairCachingGhostObject();

			charController = new KinematicCharacterController(obj,capsule,0.05f);
		}

		public void Move(Vector3 offset)
		{
			charController.SetJumpSpeed(1f);
			charController.SetUpAxis(1);
			charController.SetWalkDirection(offset);
			charController.SetVelocityForTimeInterval(offset,Time.fixedDeltaTime);
			charController.PlayerStep(Physics.world,Time.fixedDeltaTime);
		}
	}
}*/
