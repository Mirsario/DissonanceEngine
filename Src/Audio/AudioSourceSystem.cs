using Dissonance.Framework.Audio;

namespace Dissonance.Engine.Audio
{
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
				if (AL.IsSource(sourceId)) {
					AL.DeleteSource(sourceId);
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
				AL.GenSource(out audioSource.sourceId);
			}

			// Load in clips
			bool clipReady = audioSource.Clip != null && audioSource.Clip.TryGetOrRequestValue(out _);

			// Update buffer.
			uint newBufferId = clipReady ? audioSource.Clip.Value.BufferId : 0;

			if (audioSource.bufferId != newBufferId) {
				AL.Source(audioSource.sourceId, SourceInt.Buffer, (int)newBufferId);

				audioSource.bufferId = newBufferId;
			}

			if (audioSource.PendingAction != AudioSource.PlaybackAction.None) {
				switch (audioSource.PendingAction) {
					case AudioSource.PlaybackAction.Play:
						if (audioSource.bufferId == 0) {
							break;
						}

						AL.SourcePlay(audioSource.sourceId);
						goto default;
					case AudioSource.PlaybackAction.Pause:
						AL.SourcePause(audioSource.sourceId);
						goto default;
					case AudioSource.PlaybackAction.Stop:
						AL.SourceStop(audioSource.sourceId);
						goto default;
					default:
						audioSource.PendingAction = 0;
						break;
				}
			}

			// Update volume.
			AL.Source(audioSource.sourceId, SourceFloat.Gain, audioSource.Volume);

			// Update pitch.
			AL.Source(audioSource.sourceId, SourceFloat.Pitch, audioSource.Pitch);

			// Update 3D position.
			if (!audioSource.Is2D && entity.Has<Transform>()) {
				audioSource.was2D = false;

				var transform = entity.Get<Transform>();
				var position = transform.Position;

				if (position == default) {
					position.X = float.Epsilon;
				}

				AL.Source(audioSource.sourceId, SourceFloat3.Position, position.X, position.Y, position.Z);

				AL.Source(audioSource.sourceId, SourceFloat.ReferenceDistance, audioSource.RefDistance);
				AL.Source(audioSource.sourceId, SourceFloat.MaxDistance, audioSource.MaxDistance);
			} else if (!audioSource.was2D) {
				audioSource.was2D = true;

				AL.Source(audioSource.sourceId, SourceFloat3.Position, 0f, 0f, 0f);
			}

			// Update looping.
			if (audioSource.Loop != audioSource.wasLooped) {
				AL.Source(audioSource.sourceId, SourceBool.Looping, audioSource.Loop);

				audioSource.wasLooped = audioSource.Loop;
			}

			// Update state.
			audioSource.State = (SourceState)AL.GetSource(audioSource.sourceId, GetSourceInt.SourceState);
		}

		[Subsystem]
		partial void CheckALErrors()
		{
			AudioEngine.CheckALErrors();
		}
	}
}
