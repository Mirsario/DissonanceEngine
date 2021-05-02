namespace Dissonance.Engine
{
	public class ComponentManager : EngineModule
	{
		internal static ComponentManager Instance => Game.Instance.GetModule<ComponentManager>(true);

		protected override void Init()
		{
			
		}
	}
}
