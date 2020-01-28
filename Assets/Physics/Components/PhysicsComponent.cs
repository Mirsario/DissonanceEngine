/*namespace GameEngine.Physics
{
	public abstract class PhysicsComponent : Component
	{
		protected override void OnPreInit()
		{
			gameObject.rigidbodyInternal ??= new RigidbodyInternal(GameObject);
		}
		protected override void OnDispose()
		{
			if(gameObject.CountComponents<PhysicsComponent>()==0) {
				gameObject.rigidbodyInternal.Dispose();
			}
		}
	}
}*/
