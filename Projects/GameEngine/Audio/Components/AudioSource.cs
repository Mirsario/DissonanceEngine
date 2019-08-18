using System;
using OpenTK.Audio.OpenAL;

namespace GameEngine
{
	public class AudioSource : Component
	{
		internal uint sourceId;

		protected AudioClip clip;
		protected bool is2D;
		protected bool loop;
		protected float refDistance = 1f;
		protected float maxDistance = float.MaxValue;
		protected float volume = 1f;
		protected float playbackOffset;
		protected bool updateClip;
		protected bool updateIs2D;
		protected bool updateLoop;
		protected bool updateRefDistance = true;
		protected bool updateMaxDistance = true;
		protected bool updateVolume = true;
		protected bool updatePlaybackOffset;

		///<summary>Indicates whether or not the source is currently playing.</summary>
		public bool IsPlaying => AL.GetSourceState(sourceId)==ALSourceState.Playing;

		public AudioClip Clip {
			get => clip;
			set {
				if(beenEnabledBefore && IsPlaying) {
					Stop();
				}

				clip = value;

				if(beenEnabledBefore) {
					AL.Source(sourceId,ALSourcei.Buffer,clip?.bufferId ?? -1);
					updateClip = false;
				} else {
					updateClip = true;
				}
			}
		}
		public bool Is2D {
			get => is2D;
			set {
				is2D = value;

				if(beenEnabledBefore) {
					AL.Source(sourceId,ALSourceb.SourceRelative,is2D);
					var pos = (OpenTK.Vector3)(is2D ? Vector3.Zero : Transform.Position);
					AL.Source(sourceId,ALSource3f.Position,ref pos);

					updateIs2D = false;
				} else {
					updateIs2D = true;
				}
			}
		}
		public bool Loop {
			get => loop;
			set {
				loop = value;

				if(beenEnabledBefore) {
					AL.Source(sourceId,ALSourceb.Looping,loop);

					updateLoop = false;
				} else {
					updateLoop = true;
				}
			}
		}
		public float RefDistance {
			get => refDistance;
			set {
				refDistance = value;

				if(beenEnabledBefore) {
					AL.Source(sourceId,ALSourcef.ReferenceDistance,refDistance);

					updateRefDistance = false;
				} else {
					updateRefDistance = true;
				}
			}
		}
		public float MaxDistance {
			get => maxDistance;
			set {
				maxDistance = value;

				if(beenEnabledBefore) {
					AL.Source(sourceId,ALSourcef.MaxDistance,maxDistance);

					updateMaxDistance = false;
				} else {
					updateMaxDistance = true;
				}
			}
		}
		public float Volume {
			get => volume;
			set {
				volume = value;

				if(beenEnabledBefore) {
					if(value<1f) {
						AL.Source(sourceId,ALSourcef.Gain,1f);
						AL.Source(sourceId,ALSourcef.MaxGain,Math.Max(0f,value));
					} else {
						AL.Source(sourceId,ALSourcef.Gain,value);
						AL.Source(sourceId,ALSourcef.MaxGain,1f);
					}

					updateVolume = false;
				} else {
					updateVolume = true;
				}
			}
		}
		public float PlaybackOffset {
			get => playbackOffset;
			set {
				playbackOffset = value;

				if(beenEnabledBefore) {
					AL.Source(sourceId,ALSourcef.SecOffset,value);

					updatePlaybackOffset = false;
				} else {
					updatePlaybackOffset = true;
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
			if(updateClip) {
				Clip = clip;
			}
			if(updateIs2D) {
				Is2D = is2D;
			}
			if(updateLoop) {
				Loop = loop;
			}
			if(updateRefDistance) {
				RefDistance = refDistance;
			}
			if(updateMaxDistance) {
				MaxDistance = maxDistance;
			}
			if(updateVolume) {
				Volume = volume;
			}
			if(updatePlaybackOffset) {
				PlaybackOffset = playbackOffset;
			}
		}
		protected override void OnDispose()
		{
			AL.DeleteSource(ref sourceId);
		}
		public override void FixedUpdate()
		{
			if(!is2D) {
				OpenTK.Vector3 pos = Transform.Position;
				AL.Source(sourceId,ALSource3f.Position,ref pos);
			}
		}

		public void Play()
		{
			if(clip==null) {
				throw new Exception("This AudioSource has no clip set!");
			}

			AL.SourcePlay(sourceId);
		}
		public void Pause()
		{
			if(clip==null) {
				throw new Exception("This AudioSource has no clip set!");
			}

			AL.SourcePause(sourceId);
		}
		public void Stop()
		{
			if(clip==null) {
				throw new Exception("This AudioSource has no clip set!");
			}

			AL.SourceStop(sourceId);
		}
	}
}

