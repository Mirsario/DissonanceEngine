using Dissonance.Framework.Audio;
using System;

namespace Dissonance.Engine
{
	[AllowOnlyOneInWorld]
	public class AudioListener : Component
	{
		public override void FixedUpdate()
		{
			Vector3 pos = Transform.Position;

			if(pos.HasNaNs) {
				throw new Exception($"NaNs values detected in {GameObject.GetType().Name}'s Transform.");
			}

			AL.Listener3(ListenerFloat3.Position,pos.x,pos.y,pos.z);

			Vector3 lookAt = -Transform.Forward;
			Vector3 up = Transform.Up;

			AL.Listener(ListenerFloatArray.Orientation,new[] {
				lookAt.x,	lookAt.y,	lookAt.z,
				up.x,		up.y,		up.z
			});
		}
	}
}

