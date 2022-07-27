using System;
using Dissonance.Engine.Graphics;
using Silk.NET.OpenGL;

namespace Dissonance.Engine;

partial class Debug
{
	private const int DefaultBufferSize = 1024;

	private static Mesh bufferMesh;
	private static int verticesSize = DefaultBufferSize;
	private static int vertexIndex;

	public static void DrawLine(Vector3 start, Vector3 end, Vector4 color)
	{
		AllocateSpace(vertexIndex + 2);
		AddLine(bufferMesh, ref vertexIndex, start, end, color);
	}

	public static void DrawBox(Vector3 start, Vector3 end, Vector4 color)
	{
		AllocateSpace(vertexIndex + 24);

		var a = start;
		var b = end;

		// Ceiling
		AddLine(bufferMesh, ref vertexIndex, new Vector3(a.X, a.Y, a.Z), new Vector3(b.X, a.Y, a.Z), color);
		AddLine(bufferMesh, ref vertexIndex, new Vector3(a.X, a.Y, a.Z), new Vector3(a.X, a.Y, b.Z), color);
		AddLine(bufferMesh, ref vertexIndex, new Vector3(b.X, a.Y, a.Z), new Vector3(b.X, a.Y, b.Z), color);
		AddLine(bufferMesh, ref vertexIndex, new Vector3(a.X, a.Y, b.Z), new Vector3(b.X, a.Y, b.Z), color);
		// Walls
		AddLine(bufferMesh, ref vertexIndex, new Vector3(a.X, a.Y, a.Z), new Vector3(a.X, b.Y, a.Z), color);
		AddLine(bufferMesh, ref vertexIndex, new Vector3(b.X, a.Y, a.Z), new Vector3(b.X, b.Y, a.Z), color);
		AddLine(bufferMesh, ref vertexIndex, new Vector3(a.X, a.Y, b.Z), new Vector3(a.X, b.Y, b.Z), color);
		AddLine(bufferMesh, ref vertexIndex, new Vector3(b.X, a.Y, b.Z), new Vector3(b.X, b.Y, b.Z), color);
		// Floor
		AddLine(bufferMesh, ref vertexIndex, new Vector3(a.X, b.Y, a.Z), new Vector3(b.X, b.Y, a.Z), color);
		AddLine(bufferMesh, ref vertexIndex, new Vector3(a.X, b.Y, a.Z), new Vector3(a.X, b.Y, b.Z), color);
		AddLine(bufferMesh, ref vertexIndex, new Vector3(b.X, b.Y, a.Z), new Vector3(b.X, b.Y, b.Z), color);
		AddLine(bufferMesh, ref vertexIndex, new Vector3(a.X, b.Y, b.Z), new Vector3(b.X, b.Y, b.Z), color);
	}

	internal static void ResetRendering()
	{
		bufferMesh ??= new Mesh {
			PrimitiveType = PrimitiveType.Lines
		};

		verticesSize = DefaultBufferSize;
		vertexIndex = 0;

		Array.Resize(ref bufferMesh.Vertices, verticesSize);
		Array.Resize(ref bufferMesh.Colors, verticesSize);
	}

	internal static void FlushRendering()
	{
		bufferMesh.Apply();
		bufferMesh.Render(vertexIndex);
	}

	private static void AllocateSpace(int requiredLength)
	{
		if (verticesSize >= requiredLength) {
			return;
		}

		do {
			verticesSize *= 2;
		}
		while (verticesSize < requiredLength);

		Array.Resize(ref bufferMesh.Vertices, verticesSize);
		Array.Resize(ref bufferMesh.Colors, verticesSize);
	}

	private static void AddLine(Mesh mesh, ref int index, Vector3 start, Vector3 end, Vector4 color)
	{
		mesh.Vertices[index] = start;
		mesh.Colors[index] = color;

		index++;

		mesh.Vertices[index] = end;
		mesh.Colors[index] = color;

		index++;
	}
}

