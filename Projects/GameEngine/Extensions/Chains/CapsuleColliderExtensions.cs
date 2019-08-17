namespace GameEngine.Extensions.Chains
{
	public static class CapsuleColliderExtensions
	{
		public static T WithRadius<T>(this T obj,float radius) where T : CapsuleCollider
		{
			obj.Radius = radius;
			return obj;
		}
		public static T WithHeight<T>(this T obj,float height) where T : CapsuleCollider
		{
			obj.Height = height;
			return obj;
		}
	}
}