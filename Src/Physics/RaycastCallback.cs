using System;
using BulletSharp;

namespace Dissonance.Engine.Physics
{
	/*internal class RaycastCallback : ClosestRayResultCallback
	{
		public Func<GameObject, bool?> customFilter;
		public int triangleIndex = -1;
		public ulong layerMask;
		public ICollider collider;

		public RaycastCallback(ref BulletSharp.Math.Vector3 rayFromWorld, ref BulletSharp.Math.Vector3 rayToWorld, ulong layerMask, Func<GameObject, bool?> customFilter) : base(ref rayFromWorld, ref rayToWorld)
		{
			this.layerMask = layerMask;
			this.customFilter = customFilter;
		}

		public override float AddSingleResult(ref LocalRayResult rayResult, bool normalInWorldSpace)
		{
			var rb = rayResult.CollisionObject;
			var shapeInfo = rayResult.LocalShapeInfo;

			if(rb != null && shapeInfo.HasValue) {
				var shapeInfoValue = shapeInfo.Value;
				var collShape = PhysicsEngine.GetSubShape(rb.CollisionShape, shapeInfoValue.ShapePart);

				if(collShape != null) {
					var userObject = collShape.UserObject;

					if(userObject != null && userObject is ICollider coll) {
						collider = coll;
					}

					triangleIndex = shapeInfoValue.TriangleIndex;
				}
			}

			return base.AddSingleResult(ref rayResult, normalInWorldSpace);
		}
		public override bool NeedsCollision(BroadphaseProxy proxy)
		{
			if(proxy.ClientObject is RigidBody bulletBody) {
				var rbInternal = bulletBody.UserObject as RigidbodyInternal;
				ulong objLayerMask = Layers.GetLayerMask(rbInternal.gameObject.Layer);

				if(rbInternal != null) {
					var resultOverride = customFilter?.Invoke(rbInternal.gameObject);

					if(resultOverride != null) {
						return resultOverride.Value;
					}

					if((objLayerMask & layerMask) == 0) {
						return false;
					}
				}
			}

			return base.NeedsCollision(proxy);
		}
	}*/
}
