namespace GameEngine.Extensions.Chains
{
	public static class MeshRendererExtensions
	{
		public static T WithMesh<T>(this T obj,Mesh mesh) where T : MeshRenderer
		{
			obj.Mesh = mesh;
			return obj;
		}
		public static T WithLODMesh<T>(this T obj,MeshLOD lodMesh) where T : MeshRenderer
		{
			obj.LODMesh = lodMesh;
			return obj;
		}
		public static T WithLODMeshes<T>(this T obj,MeshLOD[] lodMeshes) where T : MeshRenderer
		{
			obj.LODMeshes = lodMeshes;
			return obj;
		}
	}
}