namespace Dissonance.Engine.Audio
{
	public readonly struct StopAudioSourceMessage : IMessage
	{
		public readonly Entity Entity;

		public StopAudioSourceMessage(Entity entity)
		{
			Entity = entity;
		}
	}
}
