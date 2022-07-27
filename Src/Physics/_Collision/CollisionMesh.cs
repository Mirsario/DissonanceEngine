using System;
using BulletSharp;
using Dissonance.Engine.Graphics;

namespace Dissonance.Engine.Physics;

public abstract class CollisionMesh : IDisposable
{
	internal protected CollisionShape CollisionShape { get; protected set; }

	public abstract void SetupFromMesh(Mesh mesh);

	public virtual void Dispose()
	{
		CollisionShape.Dispose();
		GC.SuppressFinalize(this);
	}
}
