using Dissonance.Framework.Audio;
using System;

namespace Dissonance.Engine.Audio
{
	public struct AudioSource : IComponent
	{
		[Flags]
		internal enum UpdateFlags : byte
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

		internal uint sourceId;
		internal UpdateFlags updateFlags;

		private AudioClip clip;
		private bool is2D;
		private bool loop;
		private float refDistance;
		private float maxDistance; // = DefaultMaxDistance;
		private float volume; // = 1f;
		private float pitch; // = 1f;
		private float playbackOffset;

		///<summary>Indicates whether or not the source is currently playing.</summary>
		public bool IsPlaying => (SourceState)AL.GetSource(sourceId, GetSourceInt.SourceState) == SourceState.Playing;

		public AudioClip Clip {
			get => clip;
			set {
				if(clip != value) {
					clip = value;
					updateFlags |= UpdateFlags.Clip;
				}
			}
		}
		public bool Is2D {
			get => is2D;
			set {
				if(is2D != value) {
					is2D = value;
					updateFlags |= UpdateFlags.Is2D;
				}
			}
		}
		public bool Loop {
			get => loop;
			set {
				if(loop != value) {
					loop = value;
					updateFlags |= UpdateFlags.Loop;
				}
			}
		}
		public float RefDistance {
			get => refDistance;
			set {
				if(refDistance != value) {
					refDistance = value;
					updateFlags |= UpdateFlags.RefDistance;
				}
			}
		}
		public float MaxDistance {
			get => maxDistance;
			set {
				if(maxDistance != value) {
					maxDistance = value;
					updateFlags |= UpdateFlags.MaxDistance;
				}
			}
		}
		public float Volume {
			get => volume;
			set {
				if(volume != value) {
					volume = value;
					updateFlags |= UpdateFlags.Volume;
				}
			}
		}
		public float Pitch {
			get => pitch;
			set {
				if(pitch != value) {
					pitch = value;
					updateFlags |= UpdateFlags.Pitch;
				}
			}
		}
		public float PlaybackOffset {
			get => playbackOffset;
			set {
				if(playbackOffset != value) {
					playbackOffset = value;
					updateFlags |= UpdateFlags.PlaybackOffset;
				}
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

