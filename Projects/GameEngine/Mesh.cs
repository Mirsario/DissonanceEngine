using System;
using OpenTK.Graphics.OpenGL;

#pragma warning disable 0649

namespace GameEngine
{
	public struct BoneWeight
	{
		public int boneIndex0;
		public int boneIndex1;
		public int boneIndex2;
		public int boneIndex3;
		public float weight0;
		public float weight1;
		public float weight2;
		public float weight3;
	}
	public class SubMesh
	{
		public int[] triangles;
	}

	public class Mesh : Asset<Mesh>
	{
		//TODO: Implement OnDispose

		internal bool IsReady => vertexBufferId!=-1 && indexBufferId!=-1 && vertexCount>0 && indexLength>0;

		public string name;
		internal int vertexBufferId = -1;
		internal int indexBufferId = -1;
		internal int vertexSize;
		public int vertexCount;
		public int indexLength;
		
		public int[] triangles;
		public Vector3[] vertices;
		public Vector3[] normals;
		public Vector2[] uv;
		public Vector4[] colors;
		public Vector4[] tangents;
		public BoneWeight[] boneWeights;
		public AnimationSkeleton skeleton;
		
		public Vector3 boundsCenter;
		public Vector3 boundsExtent;
		
		public void Apply()
		{
			//Debug.StartStopwatch("meshApply");
			if(vertices==null || vertices.Length==0) {
				throw new ArgumentException("Mesh's vertice array cannot be null or empty");
			}
			if(triangles==null || triangles.Length==0) {
				throw new ArgumentException("Mesh's triangle array cannot be null or empty");
			}
			if(normals!=null && normals.Length!=vertices.Length) {
				throw new ArgumentException("Mesh's normals array is supposed to be the same length as vertices or be null");
			}
			if(tangents!=null && tangents.Length!=vertices.Length) {
				throw new ArgumentException("Mesh's tangents array is supposed to be the same length as vertices or be null");
			}
			if(colors!=null && colors.Length!=vertices.Length) {
				throw new ArgumentException("Mesh's colors array is supposed to be the same length as vertices or be null");
			}
			if(uv!=null && uv.Length!=vertices.Length) {
				throw new ArgumentException("Mesh's uv array is supposed to be the same length as vertices or be null");
			}

			float xMin = 0f; float xMax = 0f;
			float yMin = 0f; float yMax = 0f;
			float zMin = 0f; float zMax = 0f;
			vertexCount = vertices.Length;
			indexLength = triangles.Length;
			vertexSize = 3+(uv!=null ? 2 : 0)+(normals!=null ? 3 : 0)+(colors!=null ? 4 : 0)+(tangents!=null ? 4 : 0)+(boneWeights!=null ? 8 : 0);
			var vertexData = new float[vertexSize*vertices.Length];
			vertexSize *= sizeof(float);

			int j = 0;
			for(int i = 0;i<vertices.Length;i++) {
				//Bounding box stuff
				if(vertices[i].x>xMax) {
					xMax = vertices[i].x;
				}else if(vertices[i].x<xMin) {
					xMin = vertices[i].x;
				}
				if(vertices[i].y>yMax) {
					yMax = vertices[i].y;
				}else if(vertices[i].y<yMin) {
					yMin = vertices[i].y;
				}
				if(vertices[i].z>zMax) {
					zMax = vertices[i].z;
				}else if(vertices[i].z<zMin) {
					zMin = vertices[i].z;
				}

				//Map all data to 1D array
				vertexData[j] = vertices[i].x;
				vertexData[j+1] = vertices[i].y;
				vertexData[j+2] = vertices[i].z;
				j += 3;

				if(normals!=null) {
					vertexData[j] = normals[i].x;
					vertexData[j+1] = normals[i].y;
					vertexData[j+2] = normals[i].z;
					j += 3;
				}

				if(tangents!=null) {
					vertexData[j] = tangents[i].x;
					vertexData[j+1] = tangents[i].y;
					vertexData[j+2] = tangents[i].z;
					vertexData[j+3] = tangents[i].w;
					j += 4;
				}

				if(colors!=null) {
					vertexData[j] = colors[i].x;
					vertexData[j+1] = colors[i].y;
					vertexData[j+2] = colors[i].z;
					vertexData[j+3] = colors[i].w;
					j += 4;
				}

				if(boneWeights!=null) {
					vertexData[j] = boneWeights[i].boneIndex0;
					vertexData[j+1] = boneWeights[i].boneIndex1;
					vertexData[j+2] = boneWeights[i].boneIndex2;
					vertexData[j+3] = boneWeights[i].boneIndex3;
					vertexData[j+4] = boneWeights[i].weight0;
					vertexData[j+5] = boneWeights[i].weight1;
					vertexData[j+6] = boneWeights[i].weight2;
					vertexData[j+7] = boneWeights[i].weight3;
					j += 8;
				}

				if(uv!=null) {
					vertexData[j] = uv[i].x;
					vertexData[j+1] = 1f-uv[i].y;
					j += 2;
				}
			}

			boundsCenter = new Vector3((xMin+xMax)/2f,(yMin+yMax)/2f,(zMin+zMax)/2f);
			boundsExtent = new Vector3(
				Mathf.Max(Mathf.Abs(xMin),Mathf.Abs(xMax))-boundsCenter.x,
				Mathf.Max(Mathf.Abs(yMin),Mathf.Abs(yMax))-boundsCenter.y,
				Mathf.Max(Mathf.Abs(zMin),Mathf.Abs(zMax))-boundsCenter.z
			);

			if(vertexBufferId==-1) {
				vertexBufferId = GL.GenBuffer();
			}

			GL.BindBuffer(BufferTarget.ArrayBuffer,vertexBufferId);
			GL.BufferData(BufferTarget.ArrayBuffer,(IntPtr)(sizeof(float)*vertexData.Length),vertexData,BufferUsageHint.StaticDraw);
			GL.BindBuffer(BufferTarget.ArrayBuffer,0);
			
			if(indexBufferId==-1) {
				indexBufferId = GL.GenBuffer();
			}

			GL.BindBuffer(BufferTarget.ElementArrayBuffer,indexBufferId);
			GL.BufferData(BufferTarget.ElementArrayBuffer,(IntPtr)(sizeof(int)*triangles.Length),triangles,BufferUsageHint.StaticDraw);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer,0);
		}
		public void RecalculateNormals()
		{
			var newNormals = new Vector3[vertices.Length];
			for(int i=0;i<triangles.Length/3;i++) {
				var firstVec = vertices[triangles[i*3+1]]-vertices[triangles[i*3]];
				var secondVec = vertices[triangles[i*3]]-vertices[triangles[i*3+2]];
				var normal = Vector3.Cross(firstVec,secondVec).Normalized;
				newNormals[triangles[i*3]] -= normal;
				newNormals[triangles[i*3+1]] -= normal;
				newNormals[triangles[i*3+2]] -= normal;
			}

			var zero = Vector3.Zero;
			for(int i=0;i<vertices.Length;i++) {
				if(newNormals[i]!=zero) {
					newNormals[i].Normalize();
				}
			}
			
			normals = newNormals;
		}
		public void RecalculateTangents()
		{
			if(uv==null) {
				throw new Exception("RecalculateTangents() requires a working UV array");
			}
			if(normals==null) {
				throw new Exception("RecalculateTangents() requires a working array of normals, call RecalculateNormals() first");
			}

			int verticeCount = vertices.Length;
			int triangleCount = triangles.Length/3;

			tangents = new Vector4[verticeCount];
			var tan1 = new Vector3[verticeCount];
			var tan2 = new Vector3[verticeCount];

			int tri = 0;

			for(int i=0;i<triangleCount;i++)  {
				int i1 = triangles[tri];
				int i2 = triangles[tri+1];
				int i3 = triangles[tri+2];

				var v1 = vertices[i1];
				var v2 = vertices[i2];
				var v3 = vertices[i3];

				var w1 = uv[i1];
				var w2 = uv[i2];
				var w3 = uv[i3];

				float x1 = v2.x-v1.x;
				float x2 = v3.x-v1.x;
				float y1 = v2.y-v1.y;
				float y2 = v3.y-v1.y;
				float z1 = v2.z-v1.z;
				float z2 = v3.z-v1.z;

				float s1 = w2.x-w1.x;
				float s2 = w3.x-w1.x;
				float t1 = w2.y-w1.y;
				float t2 = w3.y-w1.y;

				float r = 1f/(s1*t2-s2*t1);
				var sdir = new Vector3((t2*x1-t1*x2)*r,(t2*y1-t1*y2)*r,(t2*z1-t1*z2)*r);
				var tdir = new Vector3((s1*x2-s2*x1)*r,(s1*y2-s2*y1)*r,(s1*z2-s2*z1)*r);

				tan1[i1] += sdir;
				tan1[i2] += sdir;
				tan1[i3] += sdir;

				tan2[i1] += tdir;
				tan2[i2] += tdir;
				tan2[i3] += tdir;

				tri += 3;
			}

			for(int i=0;i<verticeCount;i++)  {
				var n = normals[i];
				var t = tan1[i];

				//Gram-Schmidt orthogonalize
				float w = Vector3.Dot(Vector3.Cross(n,t),tan2[i])<0f ?-1f : 1f;
				tangents[i] = new Vector4((t-n*Vector3.Dot(n,t)).Normalized,w);
			}
		}
	}
	
}