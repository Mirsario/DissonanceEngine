/*namespace Dissonance.Engine.Physics
{
	public class MeshCollider : Collider
	{
		protected CollisionMesh mesh;
		public CollisionMesh Mesh {
			get => mesh;
			set {
				if(mesh!=value) {
					mesh = value;

					TryUpdateCollider();
				}
			}
		}

		internal override void UpdateCollider()
		{
			collShape = Mesh?.collShape;

			base.UpdateCollider();
		}
	}
}*/
