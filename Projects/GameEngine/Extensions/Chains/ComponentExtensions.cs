namespace GameEngine.Extensions.Chains
{
	public static class ComponentExtensions
	{
		public static T Enable<T>(this T component) where T : Component
		{
			component.Enabled = true;
			return component;
		}
		public static T Disable<T>(this T component) where T : Component
		{
			component.Enabled = false;
			return component;
		}
	}
}