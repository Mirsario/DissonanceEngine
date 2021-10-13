using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Dissonance.Framework.Graphics;

namespace Dissonance.Engine.Graphics
{
	public sealed class Mesh : IDisposable
	{
		public delegate void ArrayCopyDelegate<T>(uint meshIndex, T[] srcArray, uint srcIndex, Vector3[] dstArray, uint dstIndex, uint length);

		public readonly IndexBuffer IndexBuffer;
		// Cached Vertex Buffers
		public readonly VertexBuffer VertexBuffer;
		public readonly NormalBuffer NormalBuffer;
		public readonly TangentBuffer TangentBuffer;
		public readonly ColorBuffer ColorBuffer;
		public readonly Uv0Buffer Uv0Buffer;
		public readonly BoneWeightsBuffer BoneWeightsBuffer;

		private readonly CustomVertexBuffer[] VertexBuffers;

		private uint vertexArrayId;
		
		private PrimitiveType currentPrimitiveType = PrimitiveType.Triangles;
		private PrimitiveType primitiveTypeToSet = PrimitiveType.Triangles;

		public string Name { get; set; }
		public Bounds Bounds { get; set; }
		public BufferUsageHint BufferUsage { get; set; }
		public bool IsReady { get; private set; }

		// Shortcut refs to buffers' arrays
		public ref uint[] Indices => ref IndexBuffer.data;
		public ref Vector3[] Vertices => ref VertexBuffer.data;
		public ref Vector3[] Normals => ref NormalBuffer.data;
		public ref Vector4[] Tangents => ref TangentBuffer.data;
		public ref Vector4[] Colors => ref ColorBuffer.data;
		public ref Vector2[] Uv0 => ref Uv0Buffer.data;
		public ref BoneWeights[] BoneWeights => ref BoneWeightsBuffer.data;

		public PrimitiveType PrimitiveType {
			get => currentPrimitiveType;
			set => primitiveTypeToSet = value;
		}

		public Mesh()
		{
			BufferUsage = BufferUsageHint.StaticDraw;

			IndexBuffer = new IndexBuffer {
				Mesh = this
			};

			VertexBuffers = new CustomVertexBuffer[CustomVertexBuffer.Count];

			for (int i = 0; i < VertexBuffers.Length; i++) {
				var instance = CustomVertexBuffer.CreateInstance(i);

				instance.Mesh = this;

				VertexBuffers[i] = instance;
			}

			VertexBuffer = GetBuffer<VertexBuffer>();
			NormalBuffer = GetBuffer<NormalBuffer>();
			TangentBuffer = GetBuffer<TangentBuffer>();
			ColorBuffer = GetBuffer<ColorBuffer>();
			Uv0Buffer = GetBuffer<Uv0Buffer>();
			BoneWeightsBuffer = GetBuffer<BoneWeightsBuffer>();
		}

		public void Render()
		{
			GL.BindVertexArray(vertexArrayId);

			int indexCount = (int)IndexBuffer.DataLength;

			if (indexCount > 0) {
				GL.DrawElements(currentPrimitiveType, indexCount, DrawElementsType.UnsignedInt, 0);
			} else {
				GL.DrawArrays(currentPrimitiveType, 0, (int)VertexBuffer.DataLength);
			}

			GL.BindVertexArray(0);
		}

		public void Dispose()
		{
			if (vertexArrayId != 0) {
				GL.DeleteVertexArray(vertexArrayId);

				vertexArrayId = 0;
			}

			IndexBuffer.Dispose();

			for (int i = 0; i < VertexBuffers.Length; i++) {
				VertexBuffers[i].Dispose();
			}

			GC.SuppressFinalize(this);
		}

		public void Apply()
		{
			Rendering.CheckGLErrors();

			// Vertex and triangle buffers are the only mandatory ones.

			if (Vertices == null) {
				throw new InvalidOperationException($"{nameof(Mesh)}.{nameof(Apply)}() requires {nameof(VertexBuffer)} to be ready.");
			}
			/*if (Triangles==null) {
				throw new InvalidOperationException($"{nameof(Mesh)}.{nameof(Apply)}() requires {nameof(Triangles)} to be ready.");
			}*/

			// Bind vertex array object (and generate one if needed).

			if (vertexArrayId == 0) {
				vertexArrayId = GL.GenVertexArray();
			}

			GL.BindVertexArray(vertexArrayId);

			// Bind and push indices.

			IndexBuffer.Apply();

			// Bind and push vertex buffers.

			for (int i = 0; i < VertexBuffers.Length; i++) {
				var buffer = VertexBuffers[i];

				buffer.Apply();

				Rendering.CheckGLErrors();
			}

			// Calculate bounds.

			Bounds = VertexBuffer.CalculateBounds();

			Rendering.CheckGLErrors();

			GL.BindVertexArray(0);
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

			Rendering.CheckGLErrors();

			currentPrimitiveType = primitiveTypeToSet;

			IsReady = true;
		}

		public CustomVertexBuffer GetBuffer(int id)
			=> VertexBuffers[id];

		public CustomVertexBuffer GetBuffer(Type type)
			=> VertexBuffers[CustomVertexBuffer.GetId(type)];

		public T GetBuffer<T>() where T : CustomVertexBuffer
			=> (T)VertexBuffers[CustomVertexBuffer.GetId<T>()];

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref TData[] VertexData<TBuffer, TData>() where TBuffer : CustomVertexBuffer<TData> where TData : unmanaged
			=> ref ((TBuffer)VertexBuffers[CustomVertexBuffer.IDs<TBuffer>.id]).data;

		public static Mesh CombineMeshes(params Mesh[] meshes)
			=> CombineMeshesInternal(meshes);

		public static Mesh CombineMeshes(params (Mesh mesh, Vector3 offset)[] meshesWithOffsets)
		{
			return CombineMeshesInternal(meshesWithOffsets.Select(t => t.mesh), (meshIndex, srcVertices, srcIndex, dstVertices, dstIndex, length) => {
				var offset = meshesWithOffsets[meshIndex].offset;

				for (int i = 0; i < length; i++) {
					dstVertices[dstIndex + i] = srcVertices[srcIndex + i] + offset;
				}
			});
		}

		public static Mesh CombineMeshes(params (Mesh mesh, Matrix4x4 matrix)[] meshesWithMatrices)
		{
			void CopyVertices(uint meshIndex, Vector3[] srcArray, uint srcIndex, Vector3[] dstArray, uint dstIndex, uint length)
			{
				var matrix = meshesWithMatrices[meshIndex].matrix;

				for (int i = 0; i < length; i++) {
					dstArray[dstIndex + i] = matrix * srcArray[srcIndex + i];
				}
			}

			void CopyNormals(uint meshIndex, Vector3[] srcArray, uint srcIndex, Vector3[] dstArray, uint dstIndex, uint length)
			{
				var matrix = meshesWithMatrices[meshIndex].matrix;

				matrix.ClearScale();
				matrix.ClearTranslation();

				for (int i = 0; i < length; i++) {
					dstArray[dstIndex + i] = matrix * srcArray[srcIndex + i];
				}
			}

			return CombineMeshesInternal(meshesWithMatrices.Select(t => t.mesh), CopyVertices, CopyNormals);
		}

		internal static Mesh CombineMeshesInternal(IEnumerable<Mesh> meshes, ArrayCopyDelegate<Vector3> vertexCopyAction = null, ArrayCopyDelegate<Vector3> normalCopyAction = null)
		{
			static void DefaultCopyAction<T>(uint meshIndex, T[] srcArray, uint srcIndex, T[] dstArray, uint dstIndex, uint length)
				=> Array.Copy(srcArray, srcIndex, dstArray, dstIndex, length);

			vertexCopyAction ??= DefaultCopyAction;
			normalCopyAction ??= DefaultCopyAction;

			int newVertexCount = meshes.Sum(m => m.Vertices.Length);
			int newTriangleCount = meshes.Sum(m => m.Indices.Length);

			var newMesh = new Mesh {
				Indices = new uint[newTriangleCount],
				Vertices = new Vector3[newVertexCount],
				Normals = new Vector3[newVertexCount],
				Uv0 = new Vector2[newVertexCount],
			};

			uint vertex = 0;
			uint triangleIndex = 0;
			uint meshIndex = 0;

			foreach (var mesh in meshes) {
				// Vertices
				uint vertexCount = (uint)mesh.Vertices.Length;

				vertexCopyAction(meshIndex, mesh.Vertices, 0, newMesh.Vertices, vertex, vertexCount); // Array.Copy(mesh.vertices,0,newMesh.vertices,vertex,vertexCount);
				normalCopyAction(meshIndex, mesh.Normals, 0, newMesh.Normals, vertex, vertexCount); // Array.Copy(mesh.normals,0,newMesh.normals,vertex,vertexCount);

				Array.Copy(mesh.Uv0, 0, newMesh.Uv0, vertex, vertexCount);

				// Indices
				uint indexCount = (uint)mesh.Indices.Length;

				if (vertex == 0) {
					Array.Copy(mesh.Indices, newMesh.Indices, indexCount);
				} else {
					for (int k = 0; k < indexCount; k++) {
						newMesh.Indices[triangleIndex + k] = mesh.Indices[k] + vertex;
					}
				}

				vertex += vertexCount;
				triangleIndex += indexCount;

				meshIndex++;
			}

			newMesh.Apply();

			return newMesh;
		}
	}
}
