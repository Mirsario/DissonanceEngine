namespace Dissonance.Engine.Audio
{
	public struct AudioSource : IComponent
	{
		internal uint sourceId;

		public AudioClip Clip { get; set; }
		public float Volume { get; set; }
		public float Pitch { get; set; }
		public bool Loop { get; set; }
		public bool Is2D { get; set; }
		public float RefDistance { get; set; }
		public float MaxDistance { get; set; }
		public float PlaybackOffset { get; set; }
	}
}

