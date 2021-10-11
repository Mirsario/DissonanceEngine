using Dissonance.Framework.Audio;

namespace Dissonance.Engine.Audio
{
	[Reads<AudioSource>]
	[Reads<Transform>]
	[Writes<AudioSource>]
	[Receives<PlayAudioSourceMessage>]
	[Receives<PauseAudioSourceMessage>]
	[Receives<StopAudioSourceMessage>]
	public sealed class AudioSourceSystem : GameSystem
	{
		private EntitySet entities;

		protected internal override void Initialize()
		{
			entities = World.GetEntitySet(e => e.Has<AudioSource>());
		}

		protected internal override void RenderUpdate() => Update();

		protected internal override void FixedUpdate() => Update();

		private void Update()
		{
			foreach (var entity in entities.ReadEntities()) {
				ref var audioSource = ref entity.Get<AudioSource>();

				// Create source if needed.
				if (audioSource.sourceId == 0) {
					AL.GenSource(out audioSource.sourceId);
				}

				// Update buffer.
				uint newBufferId = audioSource.Clip?.BufferId ?? 0;

				if (audioSource.bufferId != newBufferId) {
					AL.Source(audioSource.sourceId, SourceInt.Buffer, (int)newBufferId);

					audioSource.bufferId = newBufferId;
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
			}

			AudioEngine.CheckALErrors();

			ReadMessages();
		}

		private void ReadMessages()
		{
			foreach (var stopMessage in ReadMessages<StopAudioSourceMessage>()) {
				if (stopMessage.Entity.Has<AudioSource>()) {
					var audioSource = stopMessage.Entity.Get<AudioSource>();

					AL.SourceStop(audioSource.sourceId);
				}
			}

			foreach (var pauseMessage in ReadMessages<PauseAudioSourceMessage>()) {
				if (pauseMessage.Entity.Has<AudioSource>()) {
					var audioSource = pauseMessage.Entity.Get<AudioSource>();

					AL.SourcePause(audioSource.sourceId);
				}
			}

			foreach (var playMessage in ReadMessages<PlayAudioSourceMessage>()) {
				if (playMessage.Entity.Has<AudioSource>()) {
					var audioSource = playMessage.Entity.Get<AudioSource>();

					AL.SourcePlay(audioSource.sourceId);
				}
			}

			AudioEngine.CheckALErrors();
		}
	}
}
