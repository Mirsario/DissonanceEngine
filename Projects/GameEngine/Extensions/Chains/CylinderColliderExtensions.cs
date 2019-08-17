namespace GameEngine.Extensions.Chains
{
	public static class CylinderColliderExtensions
	{
		public static T WithSize<T>(this T obj,Vector3 size) where T : CylinderCollider
		{
			obj.Size = size;
			return obj;
		}
	}
}