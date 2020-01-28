using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace GameEngine
{
	public class ObjManager : AssetManager<Mesh>
	{
		public override string[] Extensions => new [] { ".obj" };
		
		public override Mesh Import(Stream stream,string fileName)
		{
			string text;
			using(var reader = new StreamReader(stream)) {
				text = reader.ReadToEnd();
			}
			float scale = 1f;
			var meshInfo = CreateOBJInfo(text);
			PopulateOBJInfo(ref meshInfo,text,scale);
	
			var newVerts = new Vector3[meshInfo.faceData.Length];
			var newUVs = new Vector2[meshInfo.faceData.Length];
			var newNormals = new Vector3[meshInfo.faceData.Length];
			int i = 0;
			/*The following foreach loops through the facedata and assigns the appropriate vertex,uv,or normal
			for the appropriate Unity mesh array.*/
			foreach(var v in meshInfo.faceData) {
				newVerts[i] = meshInfo.vertices[(int)v.x-1];
				if(v.y>=1) {
					newUVs[i] = meshInfo.uv[(int)v.y-1];
				}
				if(v.z>=1) {
					newNormals[i] = meshInfo.normals[(int)v.z-1];
				}
				i++;
			}

			var mesh = new Mesh {
				name = Path.GetFileName(fileName),
				vertices = newVerts,
				uv = newUVs,
				normals = newNormals,
				triangles = meshInfo.triangles //TODO: Does .obj support submeshes?
			};

			mesh.Apply();
	
			//mesh.RecalculateBounds();
			//mesh.Optimize();
	
			return mesh;
		}
		internal static MeshInfo CreateOBJInfo(string objText)
		{
			int triangles = 0;
			int vertices = 0;
			int vt = 0;
			int vn = 0;
			int face = 0;
			var meshInfo = new MeshInfo();
			var reader = new StringReader(objText);
			string thisLine = reader.ReadLine();
			char[] splitIdentifier=	{ ' ' };
			string[] brokenString;
			while(thisLine!=null) {
				if(!thisLine.StartsWith("f ") && !thisLine.StartsWith("v ") && !thisLine.StartsWith("vt ") && !thisLine.StartsWith("vn ")) {
					thisLine = reader.ReadLine();
					if(thisLine!=null) {
						thisLine = thisLine.Replace("  "," ");
					}
				}else{
					thisLine = thisLine.Trim();						//Trim the current line
					brokenString = thisLine.Split(splitIdentifier,50);	//Split the line into an array,separating the original line by blank spaces
					switch(brokenString[0]) {
						case "v":
							vertices++;
							break;
						case "vt":
							vt++;
							break;
						case "vn":
							vn++;
							break;
						case "f":
							face = face+brokenString.Length-1;
							triangles += 3*(brokenString.Length-2);/*brokenString.Length is 3 or greater since a face must have at least
																					3 vertices.  For each additional vertice,there is an additional
																					triangle in the mesh(hence this formula).*/
							break;
					}
					thisLine = reader.ReadLine();
					if(thisLine!=null) {
						thisLine = thisLine.Replace("  "," ");
					}
				}
			}
			reader.Close();
			meshInfo.triangles = new int[triangles];
			meshInfo.vertices = new Vector3[vertices];
			meshInfo.uv = new Vector2[vt];
			meshInfo.normals = new Vector3[vn];
			meshInfo.faceData = new Vector3[face];
			return meshInfo;
		}
		internal static void PopulateOBJInfo(ref MeshInfo meshInfo,string objText,float sizeFactor)
		{
			while(objText.Contains("\t")) {
				objText = objText.Replace("\t"," ");
			}
			while(objText.Contains("  ")) {
				objText = objText.Replace("  "," ");
			}
			
			var reader = new StringReader(objText);
			string thisLine = reader.ReadLine();
			
			var ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
			ci.NumberFormat.CurrencyDecimalSeparator = ".";
			
			char[] splitIdentifier=	{' '};
			char[] splitIdentifier2={'/'};
			string[] brokenBrokenString;
			string[] brokenString;
			int f = 0;
			int f2 = 0;
			int v = 0;
			int vn = 0;
			int vt = 0;
			int vt1 = 0;
			int vt2 = 0;
			while(thisLine!=null) {
				if(!thisLine.StartsWith("f ")		&& !thisLine.StartsWith("v ")	&& !thisLine.StartsWith("vt ")
				&& !thisLine.StartsWith("vn ")		&& !thisLine.StartsWith("g ")	&& !thisLine.StartsWith("usemtl ")
				&& !thisLine.StartsWith("mtllib ")	&& !thisLine.StartsWith("vt1 ")	&& !thisLine.StartsWith("vt2 ")
				&& !thisLine.StartsWith("vc ")		&& !thisLine.StartsWith("usemap ")) {
					thisLine = reader.ReadLine();
					if(thisLine!=null) {
						thisLine = thisLine.Replace("  "," ");
					}
				}else{
					thisLine = thisLine.Trim();
					brokenString = thisLine.Split(splitIdentifier,50);
					switch(brokenString[0]) {
						case "g":
							
						break;
						case "usemtl":
							
						break;
						case "usemap":
							
						break;
						case "mtllib":
							
						break;
						case "v":
							meshInfo.vertices[v] = new Vector3(
								float.Parse(brokenString[1],CultureInfo.InvariantCulture),
								float.Parse(brokenString[2],CultureInfo.InvariantCulture),
								float.Parse(brokenString[3],CultureInfo.InvariantCulture)
								)*sizeFactor;
							v++;
						break;
						case "vt":
							meshInfo.uv[vt] = new Vector2(float.Parse(brokenString[1],CultureInfo.InvariantCulture),float.Parse(brokenString[2],CultureInfo.InvariantCulture));
							vt++;
						break;
						case "vt1":
							meshInfo.uv[vt1] = new Vector2(float.Parse(brokenString[1],CultureInfo.InvariantCulture),float.Parse(brokenString[2],CultureInfo.InvariantCulture));
							vt1++;
						break;
						case "vt2":
							meshInfo.uv[vt2] = new Vector2(float.Parse(brokenString[1],CultureInfo.InvariantCulture),float.Parse(brokenString[2],CultureInfo.InvariantCulture));
							vt2++;
						break;
						case "vn":
							meshInfo.normals[vn] = new Vector3(float.Parse(brokenString[1],CultureInfo.InvariantCulture),float.Parse(brokenString[2],CultureInfo.InvariantCulture),float.Parse(brokenString[3],CultureInfo.InvariantCulture));
							vn++;
						break;
						case "vc":
							
						break;
						case "f":
							int j = 1;
							var intArray = new List<int>();
							while(j<brokenString.Length && (""+brokenString[j]).Length>0) {
								var temp = new Vector3();
								brokenBrokenString = brokenString[j].Split(splitIdentifier2,3);	//Separate the face into individual components(vert,uv,normal)
								temp.x = Convert.ToInt32(brokenBrokenString[0]);
								if(brokenBrokenString.Length==2) {	//Some .obj files skip UV and normal
									temp.y = Convert.ToInt32(brokenBrokenString[1]);
								}
								if(brokenBrokenString.Length==3) {	//Some .obj files skip UV and normal
									if(brokenBrokenString[1]!="") {	//Some .obj files skip the uv and not the normal
										temp.y = Convert.ToInt32(brokenBrokenString[1]);
									}
									temp.z = Convert.ToInt32(brokenBrokenString[2]);
								}
								j++;
								meshInfo.faceData[f2] = temp;
								intArray.Add(f2);
								f2++;
							}
							j = 1;
							while(j+2<brokenString.Length) {	//Create triangles out of the face data.  There will generally be more than 1 triangle per face.
								meshInfo.triangles[f] = intArray[0];
								f++;
								meshInfo.triangles[f] = intArray[j];
								f++;
								meshInfo.triangles[f] = intArray[j+1];
								f++;
								j++;
							}
						break;
					}
					thisLine = reader.ReadLine();
					if(thisLine!=null) {
						thisLine = thisLine.Replace("  "," ");//Some .obj files insert double spaces,this removes them.
					}
				}
			}
			reader.Close();
		}
		internal struct MeshInfo
		{
			#pragma warning disable 649
			public Vector3[] vertices;
			public Vector3[] normals;
			public Vector2[] uv;
			public Vector2[] uv1;
			public Vector2[] uv2;
			public int[] triangles;
			public int[] faceVerts;
			public int[] faceUVs;
			public Vector3[] faceData;
			public string name;
			public string fileName;
			#pragma warning restore 649
		}
	}
}
