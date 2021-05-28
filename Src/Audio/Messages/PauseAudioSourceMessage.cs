namespace Dissonance.Engine.Audio
{
	public readonly struct PauseAudioSourceMessage : IMessage
	{
		public readonly Entity Entity;

		public PauseAudioSourceMessage(Entity entity)
		{
			Entity = entity;
		}
	}
}
