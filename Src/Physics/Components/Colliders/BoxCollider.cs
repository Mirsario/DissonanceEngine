using BulletSharp;

namespace Dissonance.Engine.Physics
{
	public struct BoxCollider : IComponent
	{
		public static readonly BoxCollider Default = new BoxCollider(Vector3.One);

		internal BoxShape boxShape;
		internal bool needsUpdate;

		private Vector3 size;

		public Vector3 Size {
			get => size;
			set {
				if(value != size) {
					size = value;
					needsUpdate = true;
				}
			}
		}

		public BoxCollider(Vector3 size) : this()
		{
			Size = size;
		}
	}
}
