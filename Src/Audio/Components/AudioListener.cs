namespace Dissonance.Engine.Audio
{
	[AllowOnlyOneInWorld]
	public struct AudioListener : IComponent
	{
		public static readonly AudioListener Default = new AudioListener();
	}
}

