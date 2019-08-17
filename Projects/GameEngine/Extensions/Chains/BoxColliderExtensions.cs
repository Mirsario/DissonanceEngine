using GameEngine.Physics;

namespace GameEngine.Extensions.Chains
{
	public static class BoxColliderExtensions
	{
		public static T WithSize<T>(this T obj,Vector3 size) where T : BoxCollider
		{
			obj.Size = size;
			return obj;
		}
	}
}