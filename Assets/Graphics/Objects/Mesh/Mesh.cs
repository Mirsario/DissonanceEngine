using System;
using System.Collections.Generic;
using System.Linq;
using GameEngine.Graphics;
using Dissonance.Framework.OpenGL;
using System.Runtime.CompilerServices;

#pragma warning disable 0649

namespace GameEngine
{
	public class Mesh : Asset<Mesh>
	{
		public delegate void ArrayCopyDelegate<T>(int meshIndex,T[] srcArray,int srcIndex,Vector3[] dstArray,int dstIndex,int length);

		private readonly CustomVertexBuffer[] VertexBuffers;

		public string name;
		public int[] triangles;
		public Bounds bounds;
		public BufferUsageHint bufferUsage;

		internal int indexBufferLength;
		internal uint indexBufferId;
		internal uint vertexArrayId;

		public bool IsReady { get; protected set; }

		//Vertex Buffer shortcuts
		public VertexBuffer VertexBuffer => GetBuffer<VertexBuffer>();
		public NormalBuffer NormalBuffer => GetBuffer<NormalBuffer>();
		public TangentBuffer TangentBuffer => GetBuffer<TangentBuffer>();
		public ColorBuffer ColorBuffer => GetBuffer<ColorBuffer>();
		public Uv0Buffer Uv0Buffer => GetBuffer<Uv0Buffer>();
		public BoneWeightsBuffer BoneWeightsBuffer => GetBuffer<BoneWeightsBuffer>();
		//Refs to Vertex Buffers' arrays
		public ref Vector3[] Vertices => ref VertexData<VertexBuffer,Vector3>();
		public ref Vector3[] Normals => ref VertexData<NormalBuffer,Vector3>();
		public ref Vector4[] Tangents => ref VertexData<TangentBuffer,Vector4>();
		public ref Vector4[] Colors => ref VertexData<ColorBuffer,Vector4>();
		public ref Vector2[] Uv0 => ref VertexData<Uv0Buffer,Vector2>();
		public ref BoneWeights[] BoneWeights => ref VertexData<BoneWeightsBuffer,BoneWeights>();

		public Mesh()
		{
			bufferUsage = BufferUsageHint.StaticDraw;

			VertexBuffers = new CustomVertexBuffer[CustomVertexBuffer.Count];

			for(int i = 0;i<VertexBuffers.Length;i++) {
				var instance = CustomVertexBuffer.CreateInstance(i);

				instance.mesh = this;

				VertexBuffers[i] = instance;
			}
		}

		public virtual void DrawMesh()
		{
			GL.BindVertexArray(vertexArrayId);

			GL.DrawElements(PrimitiveType.Triangles,indexBufferLength,DrawElementsType.UnsignedInt,0);

			GL.BindVertexArray(0);
		}

		public void Apply()
		{
			Rendering.CheckGLErrors();

			//Vertex and triangle buffers are the only mandatory ones.

			if(Vertices==null) {
				throw new InvalidOperationException($"{nameof(Mesh)}.{nameof(Apply)}() requires {nameof(VertexBuffer)} to be ready.");
			}
			if(triangles==null) {
				throw new InvalidOperationException($"{nameof(Mesh)}.{nameof(Apply)}() requires {nameof(triangles)} to be ready.");
			}

			//Bind vertex array object (and generate one if needed).

			if(vertexArrayId==0) {
				vertexArrayId = GL.GenVertexArray();
			}

			GL.BindVertexArray(vertexArrayId);

			//Bind and push triangles.

			if(indexBufferId==0) {
				indexBufferId = GL.GenBuffer();
			}

			GL.BindBuffer(BufferTarget.ElementArrayBuffer,indexBufferId);

			indexBufferLength = triangles.Length;

			GL.BufferData(BufferTarget.ElementArrayBuffer,indexBufferLength*sizeof(uint),triangles,bufferUsage);

			//Bind and push vertex buffers.

			for(int i = 0;i<VertexBuffers.Length;i++) {
				var buffer = VertexBuffers[i];

				buffer.Apply();

				Rendering.CheckGLErrors();
			}

			//Calculate bounds.

			bounds = VertexBuffer.CalculateBounds();

			Rendering.CheckGLErrors();

			GL.BindVertexArray(0);
			GL.BindBuffer(BufferTarget.ArrayBuffer,0);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer,0);

			IsReady = true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T GetBuffer<T>() where T : CustomVertexBuffer => (T)VertexBuffers[CustomVertexBuffer.GetId<T>()];

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref TData[] VertexData<TBuffer, TData>() where TBuffer : CustomVertexBuffer<TData> where TData : unmanaged => ref GetBuffer<TBuffer>().data;

		public static Mesh CombineMeshes(params Mesh[] meshes) => CombineMeshesInternal(meshes);
		public static Mesh CombineMeshes(params (Mesh mesh,Vector3 offset)[] meshesWithOffsets)
		{
			return CombineMeshesInternal(meshesWithOffsets.Select(t => t.mesh),(meshIndex,srcVertices,srcIndex,dstVertices,dstIndex,length) => {
				var offset = meshesWithOffsets[meshIndex].offset;

				for(int i = 0;i<length;i++) {
					dstVertices[dstIndex+i] = srcVertices[srcIndex+i]+offset;
				}
			});
		}
		public static Mesh CombineMeshes(params (Mesh mesh,Matrix4x4 matrix)[] meshesWithMatrices)
		{
			void CopyVertices(int meshIndex,Vector3[] srcArray,int srcIndex,Vector3[] dstArray,int dstIndex,int length)
			{
				var matrix = meshesWithMatrices[meshIndex].matrix;

				for(int i = 0;i<length;i++) {
					dstArray[dstIndex+i] = matrix*srcArray[srcIndex+i];
				}
			}

			void CopyNormals(int meshIndex,Vector3[] srcArray,int srcIndex,Vector3[] dstArray,int dstIndex,int length)
			{
				var matrix = meshesWithMatrices[meshIndex].matrix;

				matrix.ClearScale();
				matrix.ClearTranslation();

				for(int i = 0;i<length;i++) {
					dstArray[dstIndex+i] = matrix*srcArray[srcIndex+i];
				}
			}

			return CombineMeshesInternal(meshesWithMatrices.Select(t => t.mesh),CopyVertices,CopyNormals);
		}

		internal static Mesh CombineMeshesInternal(IEnumerable<Mesh> meshes,ArrayCopyDelegate<Vector3> vertexCopyAction = null,ArrayCopyDelegate<Vector3> normalCopyAction = null)
		{
			static void DefaultCopyAction<T>(int meshIndex,T[] srcArray,int srcIndex,T[] dstArray,int dstIndex,int length)
				=> Array.Copy(srcArray,srcIndex,dstArray,dstIndex,length);

			vertexCopyAction ??= DefaultCopyAction;
			normalCopyAction ??= DefaultCopyAction;

			int newVertexCount = meshes.Sum(m => m.Vertices.Length);
			int newTriangleCount = meshes.Sum(m => m.triangles.Length);

			Mesh newMesh = new Mesh {
				triangles = new int[newTriangleCount],
				Vertices = new Vector3[newVertexCount],
				Normals = new Vector3[newVertexCount],
				Uv0 = new Vector2[newVertexCount],
			};

			int vertex = 0;
			int triangleIndex = 0;
			int meshIndex = 0;

			foreach(var mesh in meshes) {
				//Vertices
				int vertexCount = mesh.Vertices.Length;

				vertexCopyAction(meshIndex,mesh.Vertices,0,newMesh.Vertices,vertex,vertexCount); //Array.Copy(mesh.vertices,0,newMesh.vertices,vertex,vertexCount);
				normalCopyAction(meshIndex,mesh.Normals,0,newMesh.Normals,vertex,vertexCount); //Array.Copy(mesh.normals,0,newMesh.normals,vertex,vertexCount);
				Array.Copy(mesh.Uv0,0,newMesh.Uv0,vertex,vertexCount);

				//Triangles
				int triangleIndexCount = mesh.triangles.Length;

				if(vertex==0) {
					Array.Copy(mesh.triangles,newMesh.triangles,triangleIndexCount);
				} else {
					for(int k = 0;k<triangleIndexCount;k++) {
						newMesh.triangles[triangleIndex+k] = mesh.triangles[k]+vertex;
					}
				}

				vertex += vertexCount;
				triangleIndex += triangleIndexCount;

				meshIndex++;
			}

			newMesh.Apply();

			return newMesh;
		}
	}
}