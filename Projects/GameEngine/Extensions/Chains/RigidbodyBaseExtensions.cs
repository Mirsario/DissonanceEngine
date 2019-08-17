namespace GameEngine.Extensions.Chains
{
	public static class RigidbodyBaseExtensions
	{
		public static T WithMass<T>(this T obj,float mass) where T : RigidbodyBase
		{
			obj.Mass = mass;
			return obj;
		}
		public static T WithFriction<T>(this T obj,float friction) where T : RigidbodyBase
		{
			obj.Friction = friction;
			return obj;
		}
		public static T WithDrag<T>(this T obj,float drag) where T : RigidbodyBase
		{
			obj.Drag = drag;
			return obj;
		}
		public static T WithAngularDrag<T>(this T obj,float angularDrag) where T : RigidbodyBase
		{
			obj.AngularDrag = angularDrag;
			return obj;
		}
		public static T WithUseGravity<T>(this T obj,bool useGravity) where T : RigidbodyBase
		{
			obj.UseGravity = useGravity;
			return obj;
		}
		public static T WithActive<T>(this T obj,bool active) where T : RigidbodyBase
		{
			obj.Active = active;
			return obj;
		}
		public static T WithIsKinematic<T>(this T obj,bool isKinematic) where T : RigidbodyBase
		{
			obj.IsKinematic = isKinematic;
			return obj;
		}
	}
}