namespace Dissonance.Engine.Audio
{
	public readonly struct PlayAudioSourceMessage
	{
		public readonly Entity Entity;

		public PlayAudioSourceMessage(Entity entity)
		{
			Entity = entity;
		}
	}
}
