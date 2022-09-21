using System;
using System.Runtime.InteropServices;
using BulletSharp;
using Dissonance.Engine.Graphics;

namespace Dissonance.Engine.Physics;

// Concave shapes should only be used for static meshes or kinematic rigidbodies
public class ConcaveCollisionMesh : CollisionMesh
{
	private IntPtr verticesHandle;
	private IntPtr indicesHandle;

	~ConcaveCollisionMesh()
	{
		Dispose();
	}

	public override void Dispose()
	{
		if (indicesHandle != default) {
			Marshal.FreeHGlobal(verticesHandle);

			verticesHandle = default;
		}

		if (indicesHandle != default) {
			Marshal.FreeHGlobal(indicesHandle);

			indicesHandle = default;
		}

		base.Dispose();
	}

	public override unsafe void SetupFromMesh(Mesh mesh)
	{
		TriangleIndexVertexArray meshArray;

		fixed (Vector3* verticesPtr = mesh.Vertices) {
			fixed (uint* indicesPtr = mesh.Indices) {
				int numVertices = mesh.Vertices.Length;
				int numIndices = mesh.Indices.Length;
				int numTriangles = numIndices / 3;

				int vertexSizeInBytes = 3 * sizeof(float);
				int triangleSizeInBytes = 3 * sizeof(float);
				int verticesSizeInBytes = numVertices * vertexSizeInBytes;
				int indicesSizeInBytes = numTriangles * triangleSizeInBytes;

				verticesHandle = Marshal.AllocHGlobal(verticesSizeInBytes);
				indicesHandle = Marshal.AllocHGlobal(verticesSizeInBytes);

				Buffer.MemoryCopy(verticesPtr, (void*)verticesHandle, verticesSizeInBytes, verticesSizeInBytes);
				Buffer.MemoryCopy(indicesPtr, (void*)indicesHandle, indicesSizeInBytes, indicesSizeInBytes);

				meshArray = new TriangleIndexVertexArray(numTriangles, (IntPtr)indicesHandle, triangleSizeInBytes, numVertices, (IntPtr)verticesHandle, vertexSizeInBytes);
			}
		}

		var bvhTriangleMeshShape = new BvhTriangleMeshShape(meshArray, useQuantizedAabbCompression: true, buildBvh: true);

		CollisionShape = bvhTriangleMeshShape;
	}

	public static explicit operator ConcaveCollisionMesh(Mesh mesh)
	{
		var collisionMesh = new ConcaveCollisionMesh();

		collisionMesh.SetupFromMesh(mesh);

		return collisionMesh;
	}
}
