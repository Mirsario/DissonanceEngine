using System;
using Silk.NET.OpenAL;
using static Dissonance.Engine.Audio.OpenALApi;

namespace Dissonance.Engine.Audio
{
	[Callback<EndRenderUpdateCallback>]
	[Autoload(DisablingGameFlags = GameFlags.NoAudio)]
	public sealed class AudioListenerSystem : GameSystem
	{
		private EntitySet entities;

		protected override void Initialize()
		{
			entities = World.GetEntitySet(e => e.Has<AudioListener>());
		}

		protected unsafe override void Execute()
		{
			Entity entity = default;
			bool hasEntity = false;

			foreach (var e in entities.ReadEntities()) {
				if (!hasEntity) {
					entity = e;
					hasEntity = true;
				} else {
					throw new InvalidOperationException($"Only a single {nameof(AudioListener)} is allowed to exist in a world.");
				}
			}

			const int OrientationLength = 6;

			Vector3 position;
			float* orientationPtr = stackalloc float[OrientationLength];

			if (entity.Has<Transform>()) {
				var transform = entity.Get<Transform>();
				var up = transform.Up;
				var lookAt = -transform.Forward;

				position = transform.Position;

				orientationPtr[0] = lookAt.X;
				orientationPtr[1] = lookAt.Y;
				orientationPtr[2] = lookAt.Z;
				orientationPtr[3] = up.X;
				orientationPtr[4] = up.Y;
				orientationPtr[5] = up.Z;
			} else {
				position = Vector3.Zero;

				for (int i = 0; i < OrientationLength; i++) {
					orientationPtr[i] = 0f;
				}
			}

			OpenAL.SetListenerProperty(ListenerVector3.Position, position.X, position.Y, position.Z);
			OpenAL.SetListenerProperty(ListenerFloatArray.Orientation, orientationPtr);

			AudioEngine.CheckALErrors();
		}
	}
}
