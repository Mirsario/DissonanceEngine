using System;
using System.Runtime.InteropServices;

namespace GameEngine
{
	public struct Bounds
	{
		public Vector3 center;
		public Vector3 extents;

		public Bounds(Vector3 center,Vector3 extents)
		{
			this.center = center;
			this.extents = extents;
		}
	}
}

