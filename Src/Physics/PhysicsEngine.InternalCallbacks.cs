namespace Dissonance.Engine.Physics
{
	partial class PhysicsEngine
	{
		/*
		private static void Callback_ContactAdded(ManifoldPoint cp, CollisionObjectWrapper colObj0, int partId0, int index0, CollisionObjectWrapper colObj1, int partId1, int index1)
		{
			//Bullet seems to use edge normals by default. Code below corrects it so it uses face normals instead.
			//This fixes tons of issues with rigidbodies jumping up when moving between terrain quads, even if the terrain is 100% flat.

			var shape0 = colObj0.CollisionShape;
			var shape1 = colObj1.Parent.CollisionShape; //The need for .Parent here seems like a BulletSharp bug.
			var obj = shape0.ShapeType == BroadphaseNativeType.TriangleShape ? colObj0 : shape1.ShapeType == BroadphaseNativeType.TriangleShape ? colObj1 : null;

			if(obj != null) {
				Matrix4x4 transform = obj.WorldTransform;

				transform.ClearTranslation();

				var shape = (TriangleShape)obj.CollisionShape;

				cp.NormalWorldOnB = (transform * Vector3.Cross(shape.Vertices[1] - shape.Vertices[0], shape.Vertices[2] - shape.Vertices[0])).Normalized;
			}

			//cp.UserPersistentData = colObj1Wrap.CollisionObject.UserObject;
		}
		*/
	}
}
