using GameEngine.Graphics;

namespace GameEngine
{
	public class MeshLOD
	{
		public Mesh mesh;
		public Material material;

		public float MaxDistance {
			get => maxDistance;
			set {
				maxDistance = value;
				maxDistanceSqr = value*value;
			}
		}

		internal float maxDistance;
		internal float maxDistanceSqr;
		
		public MeshLOD(Mesh mesh,Material material)
		{
			this.mesh = mesh;
			this.material = material;
		}
		public MeshLOD(Mesh mesh,Material material,float maxDistance) : this(mesh,material)
		{
			MaxDistance = maxDistance;
		}
	}
}