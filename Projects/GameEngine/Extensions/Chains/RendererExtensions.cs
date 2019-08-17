using GameEngine.Graphics;

namespace GameEngine.Extensions.Chains
{
	public static class RendererExtensions
	{
		public static T WithMaterial<T>(this T obj,Material material) where T : Renderer
		{
			obj.Material = material;
			return obj;
		}
		public static T WithMaterials<T>(this T obj,MaterialCollection materials) where T : Renderer
		{
			obj.Materials = materials;
			return obj;
		}
	}
}