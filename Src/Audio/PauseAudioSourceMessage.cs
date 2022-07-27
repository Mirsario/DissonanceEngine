namespace Dissonance.Engine.Audio;

public readonly struct PauseAudioSourceMessage
{
	public readonly Entity Entity;

	public PauseAudioSourceMessage(Entity entity)
	{
		Entity = entity;
	}
}
