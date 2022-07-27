using BulletSharp;
using System;
using System.Collections.Generic;

namespace Dissonance.Engine.Physics;

public sealed partial class PhysicsEngine : EngineModule
{
	protected override void Init()
	{
		// ManifoldPoint.ContactAdded += Callback_ContactAdded;
		// PersistentManifold.ContactProcessed += Callback_ContactProcessed;
		// PersistentManifold.ContactDestroyed += Callback_ContactDestroyed;
	}

	protected override void OnDispose()
	{
		//broadphase?.Dispose();

		/*if (rigidbodies != null) {
			for (int i = 0; i < rigidbodies.Count; i++) {
				rigidbodies[i].Dispose();
			}

			rigidbodies.Clear();
		}*/
	}

	/*public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hit, float range = 100000f, Func<ulong, ulong> mask = null, Func<GameObject, bool?> customFilter = null, bool debug = false)
	{
		direction.Normalize();

		ulong layerMask = ulong.MaxValue;

		if (mask != null) {
			layerMask = mask(layerMask);
		}

		BulletSharp.Math.Vector3 rayEnd = (origin + direction * range);
		BulletSharp.Math.Vector3 origin2 = origin;

		var callback = new RaycastCallback(ref origin2, ref rayEnd, layerMask, customFilter);

		world.RayTest(origin, rayEnd, callback);

		if (!callback.HasHit) {
			hit = new RaycastHit {
				triangleIndex = -1,
			};

			return false;
		}

		hit = new RaycastHit {
			point = callback.HitPointWorld,
			triangleIndex = callback.triangleIndex,
			collider = callback.collider,
			gameObject = callback.collider?.gameObject
		};

		return true;
	}*/
	
	internal static CollisionShape GetSubShape(CollisionShape shape, int subIndex)
	{
		if (shape is CompoundShape compoundShape && compoundShape.NumChildShapes > 0) {
			return compoundShape.GetChildShape(subIndex >= 0 ? subIndex : 0);
		}

		return shape;
	}
}
