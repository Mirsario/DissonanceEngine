namespace GameEngine
{
	public abstract class PhysicsComponent : Component
	{
		protected override void OnInit()
		{
			if(gameObject.rigidbodyInternal==null) {
				gameObject.rigidbodyInternal = new RigidbodyInternal(GameObject);
			}
		}
		protected override void OnDispose()
		{
			if(gameObject.CountComponents<PhysicsComponent>()==0) {
				gameObject.rigidbodyInternal.Dispose();
			}
		}
	}
}

