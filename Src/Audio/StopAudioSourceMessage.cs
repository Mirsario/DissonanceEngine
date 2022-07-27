namespace Dissonance.Engine.Audio;

public readonly struct StopAudioSourceMessage
{
	public readonly Entity Entity;

	public StopAudioSourceMessage(Entity entity)
	{
		Entity = entity;
	}
}
