﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
	public static class PrimitiveMeshes
	{
		//Need field access internally for full speed
		internal static Mesh quad;
		public static Mesh Quad => quad;
		internal static Mesh cube;
		public static Mesh Cube => cube;
		internal static Mesh invertedCube;
		public static Mesh InvertedCube => invertedCube;
		internal static Mesh sphere;
		public static Mesh Sphere => sphere;
		internal static Mesh icoSphere;
		public static Mesh IcoSphere => icoSphere;

		internal static void GenerateDefaultMeshes()
		{
			quad = GenerateQuad();
			cube = GenerateCube();
			invertedCube = GenerateCube(inverted:true);
			sphere = GenerateSphere();
			icoSphere = GenerateIcoSphere();
		}
		
		public static Mesh GenerateQuad(float size = 1f,bool addUVs = true,bool addNormals = true,bool addTangents = true,bool apply = true)
		{
			float half = size*0.5f;
			var newMesh = new Mesh {
				#region Vertices
				vertices = new[] {
					new Vector3(-half,-half,0f),	new Vector3( half,-half,0f),
					new Vector3(-half, half,0f),	new Vector3( half, half,0f),
				},
				#endregion
				#region UVs
				uv = addUVs ? new[] {
					new Vector2(0f,0f),new Vector2(1f,0f),
					new Vector2(0f,1f),new Vector2(1f,1f),
				} : null,
				#endregion
				#region Triangles
				triangles = new[] {
					2,1,0,
					2,3,1,
				}
				#endregion
			};
			if(addNormals) {
				newMesh.RecalculateNormals();
			}
			if(addTangents) {
				newMesh.RecalculateTangents();
			}
			if(apply) {
				newMesh.Apply();
			}
			return newMesh;
		}
		public static Mesh GenerateCube(float size = 1f,bool inverted = false,bool addUVs = true,bool addNormals = true,bool addTangents = true,bool apply = true)
		{
			float half = size*0.5f;
			var newMesh = new Mesh {
				#region Vertices
				vertices = new[] {
					new Vector3(-half, half, half),	new Vector3( half, half, half),	new Vector3(-half,-half, half),	new Vector3( half,-half, half),
					new Vector3( half, half,-half),	new Vector3(-half, half,-half),	new Vector3( half,-half,-half),	new Vector3(-half,-half,-half),
					new Vector3(-half, half, half),	new Vector3( half, half, half),	new Vector3(-half, half,-half),	new Vector3( half, half,-half),
					new Vector3(-half,-half, half),	new Vector3( half,-half, half),	new Vector3(-half,-half,-half),	new Vector3( half,-half,-half),
					new Vector3(-half, half,-half),	new Vector3(-half, half, half),	new Vector3(-half,-half,-half),	new Vector3(-half,-half, half),
					new Vector3( half, half,-half),	new Vector3( half, half, half),	new Vector3( half,-half,-half),	new Vector3( half,-half, half)
				},
				#endregion
				#region UVs
				uv = addUVs ? new[] {
					new Vector2(0f,0f),new Vector2(1f,0f),new Vector2(0f,1f),new Vector2(1f,1f),
					new Vector2(0f,0f),new Vector2(1f,0f),new Vector2(0f,1f),new Vector2(1f,1f),
					new Vector2(1f,0f),new Vector2(0f,0f),new Vector2(1f,1f),new Vector2(0f,1f),
					new Vector2(0f,0f),new Vector2(1f,0f),new Vector2(0f,1f),new Vector2(1f,1f),
					new Vector2(0f,0f),new Vector2(1f,0f),new Vector2(0f,1f),new Vector2(1f,1f),
					new Vector2(1f,0f),new Vector2(0f,0f),new Vector2(1f,1f),new Vector2(0f,1f)
				} : null,
				#endregion
				#region Triangles
				triangles = inverted ? new[] {
					1,2,0, //Inverted
					3,2,1,
					5,6,4,
					7,6,5,
					9,8,10,
					11,9,10,
					13,14,12,
					15,14,13,
					17,18,16,
					19,18,17,
					21,20,22,
					23,21,22
				} : new[] {
					2,1,0, //Normal
					2,3,1,
					6,5,4,
					6,7,5,
					8,9,10,
					9,11,10,
					14,13,12,
					14,15,13,
					18,17,16,
					18,19,17,
					20,21,22,
					21,23,22
				}
				#endregion
			};
			if(addNormals) {
				newMesh.RecalculateNormals();
			}
			if(addTangents) {
				newMesh.RecalculateTangents();
			}
			if(apply) {
				newMesh.Apply();
			}
			return newMesh;
		}
		public static Mesh GenerateSphere(int xRes = 16,int yRes = 16,float radius = 1f,bool inverted = false,bool addUVs = true,bool addNormals = true,bool apply = true)
		{
			//TODO: There's plenty of unneeded vertex repeating, which is only needed on UV seams. Fixing this could increase performance.
			float xResMultiplier = 1f/xRes;
			float yResMultiplier = 1f/yRes;
			float xOffset = Mathf.TwoPI*xResMultiplier;
			float yOffset = Mathf.PI*yResMultiplier;
			int verticeAmount = xRes*yRes*4;
			var newMesh = new Mesh {
				vertices = new Vector3[verticeAmount],
				triangles = new int[xRes*yRes*6]
			};
			if(addNormals) {
				newMesh.normals = new Vector3[verticeAmount];
			}
			if(addUVs) {
				newMesh.uv = new Vector2[verticeAmount];
			}
			
			void SphereVertex(int x,int y,int index)
			{
				float hAngle = x*xOffset;
				float vAngle = y*yOffset;
				var normal = new Vector3(Mathf.Sin(hAngle)*Mathf.Sin(vAngle),Mathf.Cos(vAngle),Mathf.Cos(hAngle)*Mathf.Sin(vAngle));
				newMesh.vertices[index] = normal*radius;
				if(addNormals) {
					newMesh.normals[index] = normal;
				}
				if(addUVs) {
					newMesh.uv[index] = new Vector2(x*xResMultiplier,y*yResMultiplier);
				}
			}

			int vertexIndex = 0;
			int triangleIndex = 0;
			for(int y=0;y<yRes;y++) {
				for(int x=0;x<xRes;x++) {
					SphereVertex(x,y,vertexIndex);
					SphereVertex(x,y+1,vertexIndex+1);
					SphereVertex(x+1,y,vertexIndex+2);
					SphereVertex(x+1,y+1,vertexIndex+3);
					newMesh.triangles[triangleIndex] = vertexIndex;
					newMesh.triangles[triangleIndex+1] = vertexIndex+1;
					newMesh.triangles[triangleIndex+2] = vertexIndex+3;
					newMesh.triangles[triangleIndex+3] = vertexIndex+2;
					newMesh.triangles[triangleIndex+4] = vertexIndex+3;
					newMesh.triangles[triangleIndex+5] = vertexIndex+1;
					vertexIndex += 4;
					triangleIndex += 6;
				}
			}
			if(apply) {
				newMesh.Apply();
			}
			return newMesh;
		}
		public static Mesh GenerateIcoSphere(float size = 1f,bool inverted = false,bool addNormals = true,bool addTangents = true,bool apply = true)
		{
			//TODO: Reimplement with an algorithm that accepts a resolution integer
			//TODO: Add UVs and make tangents work
			const float N = 0f;
			//float X = 0.525731112119133606f;
			//float Z = 0.850650808352039932f;
			float X = 0.61803398874989484830630790701031f*size;
			float Z = 1f*size;

			var newMesh = new Mesh {
				#region Vertices
				vertices = new[] {
					new Vector3(-X,N,Z),new Vector3( X, N, Z),new Vector3(-X, N,-Z),new Vector3( X, N,-Z),
					new Vector3( N,Z,X),new Vector3( N, Z,-X),new Vector3( N,-Z, X),new Vector3( N,-Z,-X),
					new Vector3( Z,X,N),new Vector3(-Z, X, N),new Vector3( Z,-X, N),new Vector3(-Z,-X, N)
				},
				#endregion
				#region Triangles
				triangles = inverted ? new[] {
					//Inverted
					4,0,1,	9,0,4,	5,9,4,	5,4,8,
					8,4,1,	10,8,1,	3,8,10,	3,5,8,
					2,5,3,	7,2,3,	10,7,3,	6,7,10,
					11,7,6,	0,11,6,	1,0,6,	1,6,10,
					0,9,11,	11,9,2,	2,9,5,	2,7,11
				} : new[] {
					//Normal
					0,4,1,	0,9,4,	9,5,4,	4,5,8,
					4,8,1,	8,10,1,	8,3,10,	5,3,8,
					5,2,3,	2,7,3,	7,10,3,	7,6,10,
					7,11,6,	11,0,6,	0,1,6,	6,1,10,
					9,0,11,	9,11,2,	9,2,5,	7,2,11
				}
				#endregion
			};
			if(addNormals) {
				newMesh.RecalculateNormals();
			}
			/*if(addTangents) {
				newMesh.RecalculateTangents();
			}*/
			if(apply) {
				newMesh.Apply();
			}
			return newMesh;
		}
	}
}
