using Dissonance.Framework.Audio;
using System;

namespace Dissonance.Engine.Audio
{
	public sealed class AudioSource : Component
	{
		[Flags]
		private enum UpdateFlags : byte
		{
			Clip = 1,
			Volume = 2,
			Pitch = 4,
			Loop = 8,
			PlaybackOffset = 16,
			Is2D = 32,
			RefDistance = 64,
			MaxDistance = 128,
		}

		public static float DefaultMaxDistance { get; set; } = 32f;

		private uint sourceId;
		private AudioClip clip;
		private bool is2D;
		private bool loop;
		private float refDistance = 0f;
		private float maxDistance = DefaultMaxDistance;
		private float volume = 1f;
		private float pitch = 1f;
		private float playbackOffset;
		private UpdateFlags updateFlags = UpdateFlags.Volume | UpdateFlags.Pitch | UpdateFlags.RefDistance | UpdateFlags.MaxDistance;

		///<summary>Indicates whether or not the source is currently playing.</summary>
		public bool IsPlaying => (SourceState)AL.GetSource(sourceId, GetSourceInt.SourceState) == SourceState.Playing;

		public AudioClip Clip {
			get => clip;
			set {
				if(Enabled && IsPlaying) {
					Stop();
				}

				clip = value;

				if(Enabled) {
					AL.Source(sourceId, SourceInt.Buffer, (int)(clip?.bufferId ?? 0));

					updateFlags &= ~UpdateFlags.Clip;
				} else {
					updateFlags |= UpdateFlags.Clip;
				}
			}
		}
		public bool Is2D {
			get => is2D;
			set {
				is2D = value;

				if(Enabled) {
					AL.Source(sourceId, SourceBool.SourceRelative, is2D);

					unsafe {
						AL.Source(sourceId, SourceFloatArray.Position, is2D ? Vector3.Zero : Transform.Position);
					}

					updateFlags &= ~UpdateFlags.Is2D;
				} else {
					updateFlags |= UpdateFlags.Is2D;
				}
			}
		}
		public bool Loop {
			get => loop;
			set {
				loop = value;

				if(Enabled) {
					AL.Source(sourceId, SourceBool.Looping, loop);

					updateFlags &= ~UpdateFlags.Loop;
				} else {
					updateFlags |= UpdateFlags.Loop;
				}
			}
		}
		public float RefDistance {
			get => refDistance;
			set {
				refDistance = value;

				if(Enabled) {
					AL.Source(sourceId, SourceFloat.ReferenceDistance, refDistance);

					updateFlags &= ~UpdateFlags.RefDistance;
				} else {
					updateFlags |= UpdateFlags.RefDistance;
				}
			}
		}
		public float MaxDistance {
			get => maxDistance;
			set {
				maxDistance = value;

				if(Enabled) {
					AL.Source(sourceId, SourceFloat.MaxDistance, maxDistance);

					updateFlags &= ~UpdateFlags.MaxDistance;
				} else {
					updateFlags |= UpdateFlags.MaxDistance;
				}
			}
		}
		public float Volume {
			get => volume;
			set {
				volume = value;

				if(Enabled) {
					if(value < 1f) {
						AL.Source(sourceId, SourceFloat.Gain, 1f);
						AL.Source(sourceId, SourceFloat.MaxGain, Math.Max(0f, value));
					} else {
						AL.Source(sourceId, SourceFloat.Gain, value);
						AL.Source(sourceId, SourceFloat.MaxGain, 1f);
					}

					updateFlags &= ~UpdateFlags.Volume;
				} else {
					updateFlags |= UpdateFlags.Volume;
				}
			}
		}
		public float Pitch {
			get => pitch;
			set {
				pitch = value;

				if(Enabled) {
					AL.Source(sourceId, SourceFloat.Pitch, value);

					updateFlags &= ~UpdateFlags.Pitch;
				} else {
					updateFlags |= UpdateFlags.Pitch;
				}
			}
		}
		public float PlaybackOffset {
			get => playbackOffset;
			set {
				playbackOffset = value;

				if(Enabled) {
					AL.Source(sourceId, SourceFloat.SecOffset, value);

					updateFlags &= ~UpdateFlags.PlaybackOffset;
				} else {
					updateFlags |= UpdateFlags.PlaybackOffset;
				}
			}
		}

		protected override void OnInit()
		{
			AL.GenSource(out sourceId);

			FixedUpdate();
		}
		protected override void OnEnable()
		{
			if(updateFlags.HasFlag(UpdateFlags.Clip)) {
				Clip = clip;
			}

			if(updateFlags.HasFlag(UpdateFlags.Is2D)) {
				Is2D = is2D;
			}

			if(updateFlags.HasFlag(UpdateFlags.Loop)) {
				Loop = loop;
			}

			if(updateFlags.HasFlag(UpdateFlags.RefDistance)) {
				RefDistance = refDistance;
			}

			if(updateFlags.HasFlag(UpdateFlags.MaxDistance)) {
				MaxDistance = maxDistance;
			}

			if(updateFlags.HasFlag(UpdateFlags.Volume)) {
				Volume = volume;
			}

			if(updateFlags.HasFlag(UpdateFlags.Pitch)) {
				Pitch = pitch;
			}

			if(updateFlags.HasFlag(UpdateFlags.PlaybackOffset)) {
				PlaybackOffset = playbackOffset;
			}
		}
		protected override void OnDispose()
		{
			AL.DeleteSource(sourceId);
		}
		protected internal override void FixedUpdate()
		{
			if(!is2D) {
				Vector3 pos = Transform.Position;

				AL.Source(sourceId, SourceFloat3.Position, pos.x, pos.y, pos.z);
			}
		}

		public void Play()
		{
			if(clip == null) {
				throw new Exception("This AudioSource has no clip set!");
			}

			AL.SourcePlay(sourceId);
		}
		public void Pause() => AL.SourcePause(sourceId);
		public void Stop() => AL.SourceStop(sourceId);
	}
}

