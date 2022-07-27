using System;
using System.Linq;
using Dissonance.Engine.IO;

namespace Dissonance.Engine.Graphics;

public struct MeshRenderer
{
	private Bounds aabb = default;
	private Matrix4x4 aabbLastMatrix = default;
	private MeshLOD[] lodMeshes;

	public MeshLOD[] LODMeshes {
		get => lodMeshes;
		set {
			if (value == null) {
				lodMeshes = value;

				return;
			}

			bool hadNull = false;

			if (value.Length == 0) {
				throw new Exception("Value cannot be an empty array. Set it to null instead.");
			}

			if (value.Any(l1 => l1 == null ? hadNull = true : value.Count(l2 => l1.maxDistance == l2.maxDistance) > 1)) { //TODO: dumb check
				throw new Exception(hadNull ? "Array cannot contain null values" : "All maxDistance values must be unique.");
			}

			var list = value.OrderBy(q => q.maxDistance).ToList();

			if (list[0].maxDistance == 0f) {
				var val = list[0];
				list.RemoveAt(0);
				list.Add(val);
			}

			lodMeshes = list.ToArray();
		}
	}
	public MeshLOD LODMesh {
		get => lodMeshes?[0];
		set {
			if (lodMeshes != null) {
				lodMeshes[0] = value;
			} else {
				LODMeshes = new[] { value };
			}
		}
	}
	public Asset<Mesh> Mesh {
		get => lodMeshes[0].Mesh;
		set => lodMeshes[0].Mesh = value;
	}
	public Asset<Material> Material {
		get => lodMeshes[0].Material;
		set => lodMeshes[0].Material = value;
	}

	public MeshRenderer()
	{
		lodMeshes = new MeshLOD[1] { new MeshLOD(null, null) };
	}

	public MeshRenderer(Asset<Mesh> mesh, Asset<Material> material)
	{
		lodMeshes = new MeshLOD[1] { new MeshLOD(mesh, material) };
	}

	public Bounds GetAabb(Transform transform)
	{
		var matrix = transform.WorldMatrix;
		var translation = matrix.ExtractTranslation();

		matrix.m30 = matrix.m31 = matrix.m32 = 0f; // Remove translation
		matrix.m03 = matrix.m13 = matrix.m23 = matrix.m33 = 0f; // Remove projection

		if (matrix == aabbLastMatrix) {
			return new Bounds(aabb.Min + translation, aabb.Max + translation);
		}

		var bounds = Mesh.Value.Bounds;
		var min = bounds.Min;
		var max = bounds.Max;

		Vector3[] corners = new Vector3[8] {
			min,
			new Vector3(min.X, min.Y, max.Z),
			new Vector3(min.X, max.Y, min.Z),
			new Vector3(max.X, min.Y, min.Z),
			new Vector3(min.X, max.Y, max.Z),
			new Vector3(max.X, min.Y, max.Z),
			new Vector3(max.X, max.Y, min.Z),
			max,
		};

		var newMin = Vector3.One * float.PositiveInfinity;
		var newMax = Vector3.One * float.NegativeInfinity;

		for (int i = 0; i < corners.Length; i++) {
			Vector3 transformed = matrix * corners[i];

			newMin = Vector3.Min(newMin, transformed);
			newMax = Vector3.Max(newMax, transformed);
		}

		aabb = new Bounds(newMin, newMax);
		aabbLastMatrix = matrix;

		return new Bounds(aabb.Min + translation, aabb.Max + translation);
	}
}
