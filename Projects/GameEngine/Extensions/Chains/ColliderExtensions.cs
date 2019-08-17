namespace GameEngine.Extensions.Chains
{
	public static class ColliderExtensions
	{
		public static T WithOffset<T>(this T obj,Vector3 offset) where T : Collider
		{
			obj.Offset = offset;
			return obj;
		}
	}
}