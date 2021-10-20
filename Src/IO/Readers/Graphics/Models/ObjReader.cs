using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Dissonance.Engine.Graphics;

namespace Dissonance.Engine.IO
{
	public class ObjReader : IAssetReader<Mesh>
	{
		internal struct MeshInfo
		{
			public uint[] triangles;
			public Vector3[] faceData;
			public Vector3[] vertices;
			public Vector3[] normals;
			public Vector2[] uv;
		}

		public string[] Extensions { get; } = { ".obj" };

		public async ValueTask<Mesh> ReadFromStream(Stream stream, string assetPath, MainThreadCreationContext switchToMainThread)
		{
			string text;

			using (var reader = new StreamReader(stream)) {
				text = reader.ReadToEnd();
			}

			float scale = 1f;
			var meshInfo = CreateOBJInfo(text);

			PopulateOBJInfo(ref meshInfo, text, scale);

			var newVerts = new Vector3[meshInfo.faceData.Length];
			var newUVs = new Vector2[meshInfo.faceData.Length];
			var newNormals = new Vector3[meshInfo.faceData.Length];

			for (int i = 0; i < meshInfo.faceData.Length; i++) {
				Vector3 v = meshInfo.faceData[i];

				newVerts[i] = meshInfo.vertices[(int)v.X - 1];

				if (v.Y >= 1) {
					newUVs[i] = meshInfo.uv[(int)v.Y - 1];
				}

				if (v.Z >= 1) {
					newNormals[i] = meshInfo.normals[(int)v.Z - 1];
				}
			}

			await switchToMainThread;

			var mesh = new Mesh {
				Name = Path.GetFileName(assetPath),
				Vertices = newVerts,
				Uv0 = newUVs,
				Normals = newNormals,
				Indices = meshInfo.triangles
			};

			mesh.Apply();

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
			char[] splitIdentifier = { ' ' };
			string[] brokenString;

			while (thisLine != null) {
				if (!thisLine.StartsWith("f ") && !thisLine.StartsWith("v ") && !thisLine.StartsWith("vt ") && !thisLine.StartsWith("vn ")) {
					thisLine = reader.ReadLine();

					if (thisLine != null) {
						thisLine = thisLine.Replace("  ", " ");
					}
				} else {
					thisLine = thisLine.Trim(); // Trim the current line
					brokenString = thisLine.Split(splitIdentifier, 50); // Split the line into an array, separating the original line by blank spaces

					switch (brokenString[0]) {
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
							face = face + brokenString.Length - 1;
							triangles += 3 * (brokenString.Length - 2);
							break;
					}

					thisLine = reader.ReadLine();

					if (thisLine != null) {
						thisLine = thisLine.Replace("  ", " ");
					}
				}
			}

			reader.Close();

			meshInfo.triangles = new uint[triangles];
			meshInfo.vertices = new Vector3[vertices];
			meshInfo.uv = new Vector2[vt];
			meshInfo.normals = new Vector3[vn];
			meshInfo.faceData = new Vector3[face];

			return meshInfo;
		}

		internal static void PopulateOBJInfo(ref MeshInfo meshInfo, string objText, float sizeFactor)
		{
			while (objText.Contains("\t")) {
				objText = objText.Replace("\t", " ");
			}

			while (objText.Contains("  ")) {
				objText = objText.Replace("  ", " ");
			}

			var reader = new StringReader(objText);
			string thisLine = reader.ReadLine();
			var ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();

			ci.NumberFormat.CurrencyDecimalSeparator = ".";

			char[] splitIdentifier = { ' ' };
			char[] splitIdentifier2 = { '/' };

			int f = 0;
			uint f2 = 0;
			int v = 0;
			int vn = 0;
			int vt = 0;
			int vt1 = 0;
			int vt2 = 0;

			string[] brokenBrokenString;
			string[] brokenString;

			while (thisLine != null) {
				if (!thisLine.StartsWith("f ") && !thisLine.StartsWith("v ") && !thisLine.StartsWith("vt ")
				&& !thisLine.StartsWith("vn ") && !thisLine.StartsWith("g ") && !thisLine.StartsWith("usemtl ")
				&& !thisLine.StartsWith("mtllib ") && !thisLine.StartsWith("vt1 ") && !thisLine.StartsWith("vt2 ")
				&& !thisLine.StartsWith("vc ") && !thisLine.StartsWith("usemap ")) {
					thisLine = reader.ReadLine();

					if (thisLine != null) {
						thisLine = thisLine.Replace("  ", " ");
					}

					continue;
				}

				thisLine = thisLine.Trim();
				brokenString = thisLine.Split(splitIdentifier, 50);

				switch (brokenString[0]) {
					case "v":
						meshInfo.vertices[v++] = new Vector3(
							float.Parse(brokenString[1], CultureInfo.InvariantCulture),
							float.Parse(brokenString[2], CultureInfo.InvariantCulture),
							float.Parse(brokenString[3], CultureInfo.InvariantCulture)
						) * sizeFactor;
						break;
					case "vt":
						meshInfo.uv[vt++] = new Vector2(
							float.Parse(brokenString[1], CultureInfo.InvariantCulture),
							float.Parse(brokenString[2], CultureInfo.InvariantCulture)
						);
						break;
					case "vt1":
						meshInfo.uv[vt1++] = new Vector2(
							float.Parse(brokenString[1], CultureInfo.InvariantCulture),
							float.Parse(brokenString[2], CultureInfo.InvariantCulture)
						);
						break;
					case "vt2":
						meshInfo.uv[vt2++] = new Vector2(
							float.Parse(brokenString[1], CultureInfo.InvariantCulture),
							float.Parse(brokenString[2], CultureInfo.InvariantCulture)
						);
						break;
					case "vn":
						meshInfo.normals[vn++] = new Vector3(
							float.Parse(brokenString[1], CultureInfo.InvariantCulture),
							float.Parse(brokenString[2], CultureInfo.InvariantCulture),
							float.Parse(brokenString[3], CultureInfo.InvariantCulture)
						);
						break;
					case "f":
						int j = 1;
						var indexList = new List<uint>();

						while (j < brokenString.Length && ("" + brokenString[j]).Length > 0) {
							var temp = new Vector3();

							brokenBrokenString = brokenString[j].Split(splitIdentifier2, 3); // Separate the face into individual components(vert,uv,normal)

							temp.X = Convert.ToInt32(brokenBrokenString[0]);

							if (brokenBrokenString.Length == 2) {  // Some .obj files skip UV and normal
								temp.Y = Convert.ToInt32(brokenBrokenString[1]);
							}

							if (brokenBrokenString.Length == 3) {  // Some .obj files skip UV and normal
								if (brokenBrokenString[1] != "") { // Some .obj files skip the uv and not the normal
									temp.Y = Convert.ToInt32(brokenBrokenString[1]);
								}

								temp.Z = Convert.ToInt32(brokenBrokenString[2]);
							}

							j++;

							meshInfo.faceData[f2] = temp;

							indexList.Add(f2);

							f2++;
						}

						j = 1;

						// Create triangles out of the face data. There will generally be more than 1 triangle per face.
						while (j + 2 < brokenString.Length) {
							meshInfo.triangles[f++] = indexList[0];
							meshInfo.triangles[f++] = indexList[j];
							meshInfo.triangles[f++] = indexList[++j];
						}

						break;
				}

				thisLine = reader.ReadLine();

				if (thisLine != null) {
					// Some .obj files insert double spaces, this removes them.
					thisLine = thisLine.Replace("  ", " ");
				}
			}

			reader.Close();
		}
	}
}
