namespace Dissonance.Engine.Audio
{
	public struct AudioSource : IComponent
	{
		public static readonly AudioSource Default = new(null);

		internal uint sourceId;
		internal uint bufferId;

		public AudioClip Clip { get; set; }
		public float Volume { get; set; }
		public float Pitch { get; set; }
		public bool Loop { get; set; }
		public bool Is2D { get; set; }
		public float RefDistance { get; set; }
		public float MaxDistance { get; set; }
		public float PlaybackOffset { get; set; }

		public AudioSource(AudioClip clip, float volume = 1f, float pitch = 1f, bool loop = false, bool is2D = false, float refDistance = 1f, float maxDistance = 32f, float playbackOffset = 0f)
		{
			Clip = clip;
			Volume = volume;
			Pitch = pitch;
			Loop = loop;
			Is2D = is2D;
			RefDistance = refDistance;
			MaxDistance = maxDistance;
			PlaybackOffset = playbackOffset;

			sourceId = 0;
			bufferId = 0;
		}
	}
}

