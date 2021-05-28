using Dissonance.Framework.Audio;

namespace Dissonance.Engine.Audio
{
	[Reads(typeof(AudioSource), typeof(Transform))]
	[Writes(typeof(AudioSource))]
	[Receives(typeof(PlayAudioSourceMessage), typeof(PauseAudioSourceMessage), typeof(StopAudioSourceMessage))]
	public sealed class AudioSourceSystem : GameSystem
	{
		private EntitySet entities;

		public override void Initialize()
		{
			entities = World.GetEntitySet(e => e.Has<AudioSource>());
		}

		public override void RenderUpdate() => Update();

		public override void FixedUpdate() => Update();

		private void Update()
		{
			foreach(var entity in entities.ReadEntities()) {
				ref var audioSource = ref entity.Get<AudioSource>();

				//Create source if needed.
				if(audioSource.sourceId == 0) {
					AL.GenSource(out audioSource.sourceId);
				}

				//Update buffer.
				uint newBufferId = audioSource.Clip?.bufferId ?? 0;

				if(audioSource.bufferId != newBufferId) {
					AL.Source(audioSource.sourceId, SourceInt.Buffer, (int)newBufferId);

					audioSource.bufferId = newBufferId;
				}
			}

			AudioEngine.CheckALErrors();

			ReadMessages();
		}

		private void ReadMessages()
		{
			foreach(var stopMessage in ReadMessages<StopAudioSourceMessage>()) {
				if(stopMessage.Entity.Has<AudioSource>()) {
					var audioSource = stopMessage.Entity.Get<AudioSource>();

					AL.SourceStop(audioSource.sourceId);
				}
			}

			foreach(var pauseMessage in ReadMessages<PauseAudioSourceMessage>()) {
				if(pauseMessage.Entity.Has<AudioSource>()) {
					var audioSource = pauseMessage.Entity.Get<AudioSource>();

					AL.SourcePause(audioSource.sourceId);
				}
			}

			foreach(var playMessage in ReadMessages<PlayAudioSourceMessage>()) {
				if(playMessage.Entity.Has<AudioSource>()) {
					var audioSource = playMessage.Entity.Get<AudioSource>();

					AL.SourcePlay(audioSource.sourceId);
				}
			}

			AudioEngine.CheckALErrors();
		}
	}
}
