using System;
using Dissonance.Framework.Audio;

namespace Dissonance.Engine.Audio
{
	[Reads(typeof(AudioListener), typeof(Transform))]
	public sealed class AudioListenerSystem : GameSystem
	{
		private static readonly float[] OrientationArray = new float[6];

		private EntitySet entities;

		public override void Initialize()
		{
			entities = World.GetEntitySet(e => e.Has<AudioListener>());
		}

		public override void RenderUpdate() => Update();

		public override void FixedUpdate() => Update();

		private void Update()
		{
			var entitySpan = entities.ReadEntities();

			if(entitySpan.Length == 0) {
				return;
			}

			if(entitySpan.Length > 1) {
				throw new InvalidOperationException($"Only a single {nameof(AudioListener)} is allowed to exist in a world.");
			}

			var entity = entitySpan[0];

			Vector3 position;

			if(entity.Has<Transform>()) {
				var transform = entity.Get<Transform>();
				var up = transform.Up;
				var lookAt = -transform.Forward;

				position = transform.Position;

				OrientationArray[0] = lookAt.x;
				OrientationArray[1] = lookAt.y;
				OrientationArray[2] = lookAt.z;
				OrientationArray[3] = up.x;
				OrientationArray[4] = up.y;
				OrientationArray[5] = up.z;
			} else {
				position = Vector3.Zero;

				for(int i = 0; i < OrientationArray.Length; i++) {
					OrientationArray[i] = 0f;
				}
			}

			AL.Listener3(ListenerFloat3.Position, position.x, position.y, position.z);
			AL.Listener(ListenerFloatArray.Orientation, OrientationArray);

			AudioEngine.CheckALErrors();
		}
	}
}
