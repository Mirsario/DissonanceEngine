using Silk.NET.OpenAL;
using static Dissonance.Engine.Audio.OpenALApi;

namespace Dissonance.Engine.Audio
{
	[Autoload(DisablingGameFlags = GameFlags.NoAudio)]
	[Callback<EndFixedUpdateCallback>]
	[Callback<EndRenderUpdateCallback>]
	[ExecuteAfter<EntitySpawningSystem>]
	public sealed partial class AudioSourceSystem : GameSystem
	{
		// Dispose sources

		[MessageSubsystem]
		partial void DisposeSources(in ComponentRemovedMessage<AudioSource> message)
		{
			uint sourceId = message.Value.sourceId;

			if (sourceId > 0) {
				if (OpenAL.IsSource(sourceId)) {
					OpenAL.DeleteSource(sourceId);
				}
			}
		}

		// Update sources' pending actions

		[MessageSubsystem]
		partial void StopAudioSources(in StopAudioSourceMessage message, [FromEntity] ref AudioSource audioSource)
		{
			audioSource.PendingAction = AudioSource.PlaybackAction.Stop;
		}

		[MessageSubsystem]
		partial void PauseAudioSources(in PauseAudioSourceMessage message, [FromEntity] ref AudioSource audioSource)
		{
			audioSource.PendingAction = AudioSource.PlaybackAction.Pause;
		}

		[MessageSubsystem]
		partial void PlayAudioSources(in PlayAudioSourceMessage message, [FromEntity] ref AudioSource audioSource)
		{
			audioSource.PendingAction = AudioSource.PlaybackAction.Play;
		}

		// Update audio sources

		[EntitySubsystem]
		partial void UpdateAudioSources(Entity entity, ref AudioSource audioSource)
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

			if (audioSource.PendingAction != AudioSource.PlaybackAction.None) {
				switch (audioSource.PendingAction) {
					case AudioSource.PlaybackAction.Play:
						if (audioSource.bufferId == 0) {
							break;
						}

						OpenAL.SourcePlay(audioSource.sourceId);
						goto default;
					case AudioSource.PlaybackAction.Pause:
						OpenAL.SourcePause(audioSource.sourceId);
						goto default;
					case AudioSource.PlaybackAction.Stop:
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

		[Subsystem]
		partial void CheckALErrors()
		{
			AudioEngine.CheckALErrors();
		}
	}
}
