using System;
using System.Collections.Generic;
using System.Linq;
using Dissonance.Engine.Graphics;
using Dissonance.Framework.Graphics;
using System.Runtime.CompilerServices;
using Dissonance.Engine.IO;

#pragma warning disable 0649

namespace Dissonance.Engine
{
	public class Mesh : Asset
	{
		public delegate void ArrayCopyDelegate<T>(uint meshIndex,T[] srcArray,uint srcIndex,Vector3[] dstArray,uint dstIndex,uint length);

		public readonly IndexBuffer IndexBuffer;

		private readonly CustomVertexBuffer[] VertexBuffers;

		public string name;
		public Bounds bounds;
		public BufferUsageHint bufferUsage;

		internal uint vertexArrayId;

		protected PrimitiveType currentPrimitiveType = PrimitiveType.Triangles;
		protected PrimitiveType primitiveTypeToSet = PrimitiveType.Triangles;

		public bool IsReady { get; protected set; }
		public PrimitiveType PrimitiveType {
			get => currentPrimitiveType;
			set => primitiveTypeToSet = value;
		}

		//Ref to Index Buffer's array
		public ref uint[] Triangles => ref IndexBuffer.data;
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

			IndexBuffer = new IndexBuffer {
				mesh = this
			};

			VertexBuffers = new CustomVertexBuffer[CustomVertexBuffer.Count];

			for(int i = 0;i<VertexBuffers.Length;i++) {
				var instance = CustomVertexBuffer.CreateInstance(i);

				instance.mesh = this;

				VertexBuffers[i] = instance;
			}
		}

		public virtual void Render()
		{
			GL.BindVertexArray(vertexArrayId);

			GL.DrawElements(currentPrimitiveType,(int)IndexBuffer.DataLength,DrawElementsType.UnsignedInt,0);

			GL.BindVertexArray(0);
		}

		public override void Dispose()
		{
			if(vertexArrayId!=0) {
				GL.DeleteVertexArray(vertexArrayId);

				vertexArrayId = 0;
			}

			IndexBuffer.Dispose();

			for(int i = 0;i<VertexBuffers.Length;i++) {
				VertexBuffers[i].Dispose();
			}
		}

		public void Apply()
		{
			Rendering.CheckGLErrors();

			//Vertex and triangle buffers are the only mandatory ones.

			if(Vertices==null) {
				throw new InvalidOperationException($"{nameof(Mesh)}.{nameof(Apply)}() requires {nameof(VertexBuffer)} to be ready.");
			}
			/*if(Triangles==null) {
				throw new InvalidOperationException($"{nameof(Mesh)}.{nameof(Apply)}() requires {nameof(Triangles)} to be ready.");
			}*/

			//Bind vertex array object (and generate one if needed).

			if(vertexArrayId==0) {
				vertexArrayId = GL.GenVertexArray();
			}

			GL.BindVertexArray(vertexArrayId);

			//Bind and push indices.

			IndexBuffer.Apply();

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
		public ref TData[] VertexData<TBuffer,TData>() where TBuffer : CustomVertexBuffer<TData> where TData : unmanaged
			=> ref ((TBuffer)VertexBuffers[CustomVertexBuffer.IDs<TBuffer>.id]).data;

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
			void CopyVertices(uint meshIndex,Vector3[] srcArray,uint srcIndex,Vector3[] dstArray,uint dstIndex,uint length)
			{
				var matrix = meshesWithMatrices[meshIndex].matrix;

				for(int i = 0;i<length;i++) {
					dstArray[dstIndex+i] = matrix*srcArray[srcIndex+i];
				}
			}

			void CopyNormals(uint meshIndex,Vector3[] srcArray,uint srcIndex,Vector3[] dstArray,uint dstIndex,uint length)
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
			static void DefaultCopyAction<T>(uint meshIndex,T[] srcArray,uint srcIndex,T[] dstArray,uint dstIndex,uint length)
				=> Array.Copy(srcArray,srcIndex,dstArray,dstIndex,length);

			vertexCopyAction ??= DefaultCopyAction;
			normalCopyAction ??= DefaultCopyAction;

			int newVertexCount = meshes.Sum(m => m.Vertices.Length);
			int newTriangleCount = meshes.Sum(m => m.Triangles.Length);

			Mesh newMesh = new Mesh {
				Triangles = new uint[newTriangleCount],
				Vertices = new Vector3[newVertexCount],
				Normals = new Vector3[newVertexCount],
				Uv0 = new Vector2[newVertexCount],
			};

			uint vertex = 0;
			uint triangleIndex = 0;
			uint meshIndex = 0;

			foreach(var mesh in meshes) {
				//Vertices
				uint vertexCount = (uint)mesh.Vertices.Length;

				vertexCopyAction(meshIndex,mesh.Vertices,0,newMesh.Vertices,vertex,vertexCount); //Array.Copy(mesh.vertices,0,newMesh.vertices,vertex,vertexCount);
				normalCopyAction(meshIndex,mesh.Normals,0,newMesh.Normals,vertex,vertexCount); //Array.Copy(mesh.normals,0,newMesh.normals,vertex,vertexCount);
				
				Array.Copy(mesh.Uv0,0,newMesh.Uv0,vertex,vertexCount);

				//Triangles
				uint triangleIndexCount = (uint)mesh.Triangles.Length;

				if(vertex==0) {
					Array.Copy(mesh.Triangles,newMesh.Triangles,triangleIndexCount);
				} else {
					for(int k = 0;k<triangleIndexCount;k++) {
						newMesh.Triangles[triangleIndex+k] = mesh.Triangles[k]+vertex;
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