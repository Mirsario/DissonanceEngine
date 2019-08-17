using GameEngine.Physics;

namespace GameEngine.Extensions.Chains
{
	public static class MeshColliderExtensions
	{
		public static T WithMesh<T>(this T obj,Mesh mesh) where T : MeshCollider
		{
			obj.Mesh = mesh;
			return obj;
		}
		public static T WithConvex<T>(this T obj,bool convex) where T : MeshCollider
		{
			obj.Convex = convex;
			return obj;
		}
	}
}