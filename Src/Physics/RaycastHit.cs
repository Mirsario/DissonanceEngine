using Dissonance.Engine.Core;
using Dissonance.Engine.Structures;

namespace Dissonance.Engine.Physics
{
	public struct RaycastHit
	{
		public Vector3 point;
		public Vector3 normal;
		public int triangleIndex;
		public Collider collider;
		public GameObject gameObject;
	}
}
