using Silk.NET.OpenAL;
using static Dissonance.Engine.Audio.OpenALApi;
using static Dissonance.Engine.Audio.AudioSource;

namespace Dissonance.Engine.Audio;

internal static partial class AudioSourceImplementation
{
	// Dispose sources

	[MessageSystem, CalledIn<AudioUpdate>, Tags("AudioMessageProcessing")]
	[Autoload(DisablingGameFlags = GameFlags.NoAudio)]
	static partial void DisposeAudioSources(in ComponentRemovedMessage<AudioSource> message)
	{
		uint sourceId = message.Value.sourceId;

		if (sourceId > 0) {
			if (OpenAL.IsSource(sourceId)) {
				OpenAL.DeleteSource(sourceId);
			}
		}
	}

	// Update sources' pending actions

	[MessageSystem, CalledIn<AudioUpdate>, Tags("AudioMessageProcessing")]
	[Autoload(DisablingGameFlags = GameFlags.NoAudio)]
	static partial void StopAudioSources(in StopAudioSourceMessage _, [FromEntity] ref AudioSource audioSource)
	{
		if (audioSource.PendingAction <= PlaybackAction.None || audioSource.PendingAction > PlaybackAction.Stop) {
			audioSource.PendingAction = PlaybackAction.Stop;
		}
	}

	[MessageSystem, CalledIn<AudioUpdate>, Tags("AudioMessageProcessing")]
	[Autoload(DisablingGameFlags = GameFlags.NoAudio)]
	static partial void PauseAudioSources(in PauseAudioSourceMessage _, [FromEntity] ref AudioSource audioSource)
	{
		if (audioSource.PendingAction <= PlaybackAction.None || audioSource.PendingAction > PlaybackAction.Pause) {
			audioSource.PendingAction = PlaybackAction.Pause;
		}
	}

	[MessageSystem, CalledIn<AudioUpdate>, Tags("AudioMessageProcessing")]
	[Autoload(DisablingGameFlags = GameFlags.NoAudio)]
	static partial void PlayAudioSources(in PlayAudioSourceMessage _, [FromEntity] ref AudioSource audioSource)
	{
		audioSource.PendingAction = PlaybackAction.Play;
	}

	// Update audio sources

	[EntitySystem, CalledIn<AudioUpdate>, RequiresTags("AudioMessageProcessing")]
	[Autoload(DisablingGameFlags = GameFlags.NoAudio)]
	static partial void UpdateAudioSources(Entity entity, ref AudioSource audioSource)
	{
		// Create source if needed.
		if (audioSource.sourceId == 0) {
			audioSource.sourceId = OpenAL.GenSource();
		}

		// Load in clips
		bool clipReady = audioSource.Clip != null && audioSource.Clip.TryGetOrRequestValue(out _);

		// Update buffer.
		uint newBufferId = clipReady ? audioSource.Clip.Value.BufferId : 0;

		if (audioSource.bufferId != newBufferId) {
			OpenAL.SetSourceProperty(audioSource.sourceId, SourceInteger.Buffer, (int)newBufferId);

			audioSource.bufferId = newBufferId;
		}

		if (audioSource.PendingAction != PlaybackAction.None) {
			switch (audioSource.PendingAction) {
				case PlaybackAction.Play:
					if (audioSource.bufferId == 0) {
						break;
					}

					OpenAL.SourcePlay(audioSource.sourceId);
					goto default;
				case PlaybackAction.Pause:
					OpenAL.SourcePause(audioSource.sourceId);
					goto default;
				case PlaybackAction.Stop:
					OpenAL.SourceStop(audioSource.sourceId);
					goto default;
				default:
					audioSource.PendingAction = 0;
					break;
			}
		}

		// Update volume.
		OpenAL.SetSourceProperty(audioSource.sourceId, SourceFloat.Gain, audioSource.Volume);

		// Update pitch.
		OpenAL.SetSourceProperty(audioSource.sourceId, SourceFloat.Pitch, audioSource.Pitch);

		// Update 3D position.
		if (!audioSource.Is2D && entity.Has<Transform>()) {
			audioSource.was2D = false;

			var transform = entity.Get<Transform>();
			var position = transform.Position;

			if (position == default) {
				position.X = float.Epsilon;
			}

			OpenAL.SetSourceProperty(audioSource.sourceId, SourceVector3.Position, position.X, position.Y, position.Z);

			OpenAL.SetSourceProperty(audioSource.sourceId, SourceFloat.ReferenceDistance, audioSource.RefDistance);
			OpenAL.SetSourceProperty(audioSource.sourceId, SourceFloat.MaxDistance, audioSource.MaxDistance);
		} else if (!audioSource.was2D) {
			audioSource.was2D = true;

			OpenAL.SetSourceProperty(audioSource.sourceId, SourceVector3.Position, 0f, 0f, 0f);
		}

		// Update looping.
		if (audioSource.Loop != audioSource.wasLooped) {
			OpenAL.SetSourceProperty(audioSource.sourceId, SourceBoolean.Looping, audioSource.Loop);

			audioSource.wasLooped = audioSource.Loop;
		}

		// Update state.
		OpenAL.GetSourceProperty(audioSource.sourceId, GetSourceInteger.SourceState, out int sourceStateInt);

		audioSource.State = (SourceState)sourceStateInt;
	}
}
