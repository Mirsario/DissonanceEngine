using System;

namespace Dissonance.Engine
{
	//TODO: Actually implement animations
	public class AnimationBone : IDisposable
	{
		public string name;
		public AnimationBone parent;
		public Transform transform;
		public Matrix4x4 baseMatrix;

		public AnimationBone(string name)
		{
			this.name = name;
		}
		public void Dispose()
		{
			transform = null;
		}
	}
}
