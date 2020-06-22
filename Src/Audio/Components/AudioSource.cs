using Dissonance.Engine.Core.Components;
using Dissonance.Engine.Structures;
using Dissonance.Framework.Audio;
using System;

namespace Dissonance.Engine.Audio.Components
{
	public class AudioSource : Component
	{
		public static float defaultMaxDistance = 32f;

		internal uint sourceId;

		protected AudioClip clip;
		protected bool is2D;
		protected bool loop;
		protected float refDistance = 0f;
		protected float maxDistance = defaultMaxDistance;
		protected float volume = 1f;
		protected float pitch = 1f;
		protected float playbackOffset;
		protected bool updateClip;
		protected bool updateIs2D;
		protected bool updateLoop;
		protected bool updateRefDistance = true;
		protected bool updateMaxDistance = true;
		protected bool updateVolume = true;
		protected bool updatePitch = true;
		protected bool updatePlaybackOffset;

		///<summary>Indicates whether or not the source is currently playing.</summary>
		public bool IsPlaying => (SourceState)AL.GetSource(sourceId,GetSourceInt.SourceState)==SourceState.Playing;

		public AudioClip Clip {
			get => clip;
			set {
				if(beenEnabledBefore && IsPlaying) {
					Stop();
				}

				clip = value;

				if(beenEnabledBefore) {
					AL.Source(sourceId,SourceInt.Buffer,(int)(clip?.bufferId ?? 0));

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
					AL.Source(sourceId,SourceBool.SourceRelative,is2D);

					unsafe {
						AL.Source(sourceId,SourceFloatArray.Position,is2D ? Vector3.Zero : Transform.Position);
					}

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
					AL.Source(sourceId,SourceBool.Looping,loop);

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
					AL.Source(sourceId,SourceFloat.ReferenceDistance,refDistance);

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
					AL.Source(sourceId,SourceFloat.MaxDistance,maxDistance);

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
						AL.Source(sourceId,SourceFloat.Gain,1f);
						AL.Source(sourceId,SourceFloat.MaxGain,Math.Max(0f,value));
					} else {
						AL.Source(sourceId,SourceFloat.Gain,value);
						AL.Source(sourceId,SourceFloat.MaxGain,1f);
					}

					updateVolume = false;
				} else {
					updateVolume = true;
				}
			}
		}
		public float Pitch {
			get => pitch;
			set {
				pitch = value;

				if(beenEnabledBefore) {
					AL.Source(sourceId,SourceFloat.Pitch,value);

					updatePitch = false;
				} else {
					updatePitch = true;
				}
			}
		}
		public float PlaybackOffset {
			get => playbackOffset;
			set {
				playbackOffset = value;

				if(beenEnabledBefore) {
					AL.Source(sourceId,SourceFloat.SecOffset,value);

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

			if(updatePitch) {
				Pitch = pitch;
			}

			if(updatePlaybackOffset) {
				PlaybackOffset = playbackOffset;
			}
		}
		protected override void OnDispose()
		{
			AL.DeleteSource(sourceId);
		}
		public override void FixedUpdate()
		{
			if(!is2D) {
				Vector3 pos = Transform.Position;

				AL.Source(sourceId,SourceFloat3.Position,pos.x,pos.y,pos.z);
			}
		}

		public void Play()
		{
			if(clip==null) {
				throw new Exception("This AudioSource has no clip set!");
			}

			AL.SourcePlay(sourceId);
		}
		public void Pause() => AL.SourcePause(sourceId);
		public void Stop() => AL.SourceStop(sourceId);
	}
}

