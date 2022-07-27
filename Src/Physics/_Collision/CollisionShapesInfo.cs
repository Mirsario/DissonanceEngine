using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using BulletSharp;

namespace Dissonance.Engine.Physics;

public struct CollisionShapesInfo
{
	internal List<CollisionShape> collisionShapes;

	public ReadOnlySpan<CollisionShape> CollisionShapes => CollectionsMarshal.AsSpan(collisionShapes);
}
