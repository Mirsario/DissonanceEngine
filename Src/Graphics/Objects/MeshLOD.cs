namespace Dissonance.Engine.Graphics
{
	public class MeshLOD
	{
		public Mesh Mesh { get; set; }
		public Material Material { get; set; }

		public float MaxDistance {
			get => maxDistance;
			set {
				maxDistance = value;
				maxDistanceSqr = value * value;
			}
		}

		internal float maxDistance;
		internal float maxDistanceSqr;

		public MeshLOD(Mesh mesh, Material material)
		{
			Mesh = mesh;
			Material = material;
		}

		public MeshLOD(Mesh mesh, Material material, float maxDistance) : this(mesh, material)
		{
			MaxDistance = maxDistance;
		}
	}
}
