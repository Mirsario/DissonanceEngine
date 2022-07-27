using Dissonance.Engine.IO;

namespace Dissonance.Engine.Graphics;

public class MeshLOD
{
	public Asset<Mesh> Mesh { get; set; }
	public Asset<Material> Material { get; set; }

	public float MaxDistance {
		get => maxDistance;
		set {
			maxDistance = value;
			maxDistanceSqr = value * value;
		}
	}

	internal float maxDistance;
	internal float maxDistanceSqr;

	public MeshLOD(Asset<Mesh> mesh, Asset<Material> material)
	{
		Mesh = mesh;
		Material = material;
	}

	public MeshLOD(Asset<Mesh> mesh, Asset<Material> material, float maxDistance) : this(mesh, material)
	{
		MaxDistance = maxDistance;
	}
}
