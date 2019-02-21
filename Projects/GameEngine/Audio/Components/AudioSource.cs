using System;
using OpenTK.Audio.OpenAL;

namespace GameEngine
{
	public class AudioSource : Component
	{
		internal uint sourceId;
		
		//TODO: Document these properties, got a little bit confusing after some absence.
		private bool _is2D;
		public bool Is2D {
			get => _is2D;
			set {
				if(value!=_is2D) {
					_is2D = value;
					AL.Source(sourceId,ALSourceb.SourceRelative,_is2D);
					var pos = (OpenTK.Vector3)(_is2D ? Vector3.zero : Transform.Position);
					AL.Source(sourceId,ALSource3f.Position,ref pos);
				}
			}
		}
		internal AudioClip _clip;
		public AudioClip Clip {
			get => _clip;
			set {
				if(_clip!=value) {
					if(IsPlaying) {
						Stop();
					}
					_clip = value;
					AL.Source(sourceId,ALSourcei.Buffer,_clip?.bufferId ?? -1);
				}
			}
		}
		///<summary>Indicates whether or not the source is currently playing.</summary>
		public bool IsPlaying => AL.GetSourceState(sourceId)==ALSourceState.Playing;
		///<summary>Indicates whether the Source is looping. Type: bool Range: [True, False] Default: False.</summary>
		public bool Loop {
			get {
				AL.GetSource(sourceId,ALSourceb.Looping,out bool val);
				return val;
			}
			set => AL.Source(sourceId,ALSourceb.Looping,value);
		}
		///<summary>Source specific reference distance. Type: float Range: [0.0f - float.PositiveInfinity] At 0.0f, no distance attenuation occurs. Type: float Default: 1.0f.</summary>
		public float RefDistance {
			get {
				AL.GetSource(sourceId,ALSourcef.ReferenceDistance,out float val);
				return val;
			}
			set => AL.Source(sourceId,ALSourcef.ReferenceDistance,value);
		}
		///<summary>Indicate distance above which Sources are not attenuated using the inverse clamped distance model. Default: float.PositiveInfinity Type: float Range: [0.0f - float.PositiveInfinity]</summary>
		private float MaxDistance {
			get {
				AL.GetSource(sourceId,ALSourcef.MaxDistance,out float val);
				return val;
			}
			set => AL.Source(sourceId,ALSourcef.MaxDistance,value);
		}
		///<summary>Indicate the gain (volume amplification) applied. Type: float. Range: [0.0f - ? ] A value of 1.0 means un-attenuated/unchanged. Each division by 2 equals an attenuation of -6dB. Each multiplicaton with 2 equals an amplification of +6dB. A value of 0.0f is meaningless with respect to a logarithmic scale; it is interpreted as zero volume - the channel is effectively disabled.</summary>
		public float Volume {
	    //Quite weird? For some reason setting ALSourcef.Gain to values lower than 1.0 is the same as setting it to 1.0.
			get {
				AL.GetSource(sourceId,ALSourcef.MaxGain,out float maxGain);
				if(maxGain<1f) {
					return maxGain;
				}
				AL.GetSource(sourceId,ALSourcef.Gain,out float gain);
				return gain;
			}
			set {
				if(value<1f) {
					AL.Source(sourceId,ALSourcef.Gain,1f);
					AL.Source(sourceId,ALSourcef.MaxGain,Math.Max(0f,value));
				}else{
					AL.Source(sourceId,ALSourcef.Gain,value);
					AL.Source(sourceId,ALSourcef.MaxGain,1f);
				}
			}
		}
		///<summary>The playback position, expressed in seconds.</summary>
		public float PlaybackOffset {
			get {
				AL.GetSource(sourceId,ALSourcef.SecOffset,out float val);
				return val;
			}
			set => AL.Source(sourceId,ALSourcef.SecOffset,value);
		}
		
		protected override void OnInit()
		{
			AL.GenSource(out sourceId);
			//AL.Source(sourceId,ALSourcef.RolloffFactor,1f);
			RefDistance = 1f;
			MaxDistance = float.MaxValue;
			Volume = 1f;
			FixedUpdate();
		}
		protected override void OnDispose()
		{
			AL.DeleteSource(ref sourceId);
		}
		public override void FixedUpdate()
		{
			if(!_is2D) {
				OpenTK.Vector3 pos = Transform.Position;
				AL.Source(sourceId,ALSource3f.Position,ref pos);
			}
		}

		public void Play()
		{
			if(_clip==null) {
				throw new Exception("This AudioSource has no clip set!");
			}
			AL.SourcePlay(sourceId);
		}
		public void Pause()
		{
			if(_clip==null) {
				throw new Exception("This AudioSource has no clip set!");
			}
			AL.SourcePause(sourceId);
		}
		public void Stop()
		{
			if(_clip==null) {
				throw new Exception("This AudioSource has no clip set!");
			}
			AL.SourceStop(sourceId);
		}
	}
}

