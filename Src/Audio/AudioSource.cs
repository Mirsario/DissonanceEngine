using Dissonance.Engine.IO;
using Silk.NET.OpenAL;

namespace Dissonance.Engine.Audio
{
	public struct AudioSource
	{
		internal enum PlaybackAction
		{
			None,
			Play,
			Pause,
			Stop
		}

		internal uint sourceId = 0;
		internal uint bufferId = 0;
		internal bool wasLooped = false;
		internal bool was2D = false;
		internal PlaybackAction PendingAction = PlaybackAction.None;

		public Asset<AudioClip> Clip { get; set; } = null;
		public float Volume { get; set; } = 1f;
		public float Pitch { get; set; } = 1f;
		public bool Loop { get; set; } = false;
		public bool Is2D { get; set; } = false;
		public float RefDistance { get; set; } = 0f;
		public float MaxDistance { get; set; } = 32f;
		public float PlaybackOffset { get; set; } = 0f;
		public SourceState State { get; internal set; } = 0;

		public AudioSource(Asset<AudioClip> clip) : this()
		{
			Clip = clip;
		}
	}
}

