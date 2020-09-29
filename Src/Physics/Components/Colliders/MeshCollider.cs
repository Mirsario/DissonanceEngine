namespace Dissonance.Engine.Physics
{
	public class MeshCollider : Collider
	{
		protected CollisionMesh mesh;

		protected override bool OwnsShape => false;

		public CollisionMesh Mesh {
			get => mesh;
			set {
				if(mesh != value) {
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
}
