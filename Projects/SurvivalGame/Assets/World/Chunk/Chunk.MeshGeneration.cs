//#define NO_CHUNK_LODS

using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using GameEngine;
using GameEngine.Graphics;
using GameEngine.Physics;

namespace SurvivalGame
{
	public partial class Chunk
	{
		public Mesh GetMesh() => mesh ?? (mesh = GenerateMesh(0,Netplay.isClient));

		private void UpdateHeights()
		{
			Tile tile;
			float minHeight = 0f;
			float maxHeight = 0f;
			for(int y = 0;y<=ChunkSize;y++) {
				for(int x = 0;x<=ChunkSize;x++) {
					tile = this[x,y];

					if(x==0 && y==0) {
						minHeight = maxHeight = tile.height;
					} else {
						minHeight = Mathf.Min(minHeight,tile.height);
						maxHeight = Mathf.Max(maxHeight,tile.height);
					}
				}
			}

			this.minHeight = minHeight;
			this.maxHeight = maxHeight;

			updateHeights = false;
		}
		private Mesh GenerateMesh(int lodLevel,bool generateGrass)
		{
			Debug.Log($"Generating mesh for chunk [{position.x},{position.y}]...");

			bool noGraphics = !Netplay.isClient;

			//var grassMeshes = (!generateGrass || noGraphics) ? null : new Dictionary<string,MeshInfo>();

			int div = lodLevel==0 ? 1 : (int)Math.Pow(2,lodLevel);
			int size = ChunkSize/div;
			int vertexCount = (size+1)*(size+1);
			int triIndexCount = size*size*6;

			mesh = new Mesh() {
				vertices = new Vector3[vertexCount],
				normals = new Vector3[vertexCount],
				uv = noGraphics ? null : new Vector2[vertexCount],
				triangles = new int[triIndexCount]
			};

			Tile tile;
			int vertex = 0;
			var vertice2D = new int[size+1,size+1];
			for(int y = 0;y<=size;y++) {
				for(int x = 0;x<=size;x++) {
					tile = this[x*div,y*div];

					mesh.vertices[vertex] = new Vector3(x*TileSize*div,tile.height,y*TileSize*div);
					if(!noGraphics) {
						mesh.uv[vertex] = new Vector2((y/(float)size),1f-(x/(float)size));
					}

					vertice2D[x,y] = vertex++;
				}
			}

			vertex = 0;

			for(int y = 0;y<=size;y++) {
				for(int x = 0;x<=size;x++) {
					int X = x*div;
					int Y = y*div;

					//NormalCalculation
					var vLeft = x==0 ? new Vector3((X-1)*TileSize,world[positionInTiles.x+X-1,positionInTiles.y+Y].height,Y*TileSize) : mesh.vertices[vertice2D[x-1,y]];
					var vRight = x==size ? new Vector3((X+1)*TileSize,world[positionInTiles.x+X+1,positionInTiles.y+Y].height,Y*TileSize) : mesh.vertices[vertice2D[x+1,y]];
					var vHorizontal = (vRight-vLeft).Normalized;

					var vUp = y==0 ? new Vector3(X*TileSize,world[positionInTiles.x+X,positionInTiles.y+Y-1].height,(Y-1)*TileSize) : mesh.vertices[vertice2D[x,y-1]];
					var vDown = y==size ? new Vector3(X*TileSize,world[positionInTiles.x+X,positionInTiles.y+Y+1].height,(Y+1)*TileSize) : mesh.vertices[vertice2D[x,y+1]];
					var vVertical = (vDown-vUp).Normalized;

					var normal = Vector3.Cross(vVertical,vHorizontal).Normalized;

					mesh.normals[vertex++] = normal;
				}
			}

			int triIndex = 0;

			for(int y = 0;y<size;y++) {
				for(int x = 0;x<size;x++) {
					int topLeft = vertice2D[x,y];
					int topRight = vertice2D[x+1,y];
					int bottomLeft = vertice2D[x,y+1];
					int bottomRight = vertice2D[x+1,y+1];

					mesh.triangles[triIndex++] = bottomLeft;
					mesh.triangles[triIndex++] = bottomRight;
					mesh.triangles[triIndex++] = topLeft;
					mesh.triangles[triIndex++] = topRight;
					mesh.triangles[triIndex++] = topLeft;
					mesh.triangles[triIndex++] = bottomRight;
				}
			}

			if(!noGraphics) {
				mesh.RecalculateTangents();
			}

			mesh.Apply();

			return mesh;
		}
		private void GenerateTexture()
		{
			//Preparation
			const int IdTexSize = ChunkSize+2;

			var idPixels = new Pixel[IdTexSize,IdTexSize];
			//var specularPixels = new Pixel[IdTexSize,IdTexSize];
			var tileUVs = new List<(ushort type, byte variant, Vector4 uv)>();

			for(int y = -1;y<=ChunkSize;y++) {
				for(int x = -1;x<=ChunkSize;x++) {
					var tileWorld = positionInTiles+new Vector2Int(x,y);
					world.RepeatTilePos(ref tileWorld.x,ref tileWorld.y);

					Tile tile = world[tileWorld.x,tileWorld.y];
					var type = TileType.byId[tile.type];
					byte variant = type.GetVariant(tileWorld.x,tileWorld.y);

					int index;
					if(!tileUVs.Any(q => q.type==tile.type && q.variant==variant)) {
						index = tileUVs.Count;
						tileUVs.Add((tile.type, variant, type.variantUVs[variant]));
					} else {
						index = tileUVs.FindIndex(q => q.type==tile.type && q.variant==variant);
					}

					idPixels[x+1,y+1] = new Pixel((byte)index,255,255,255);

					//specularPixels[x+1,y+1] = new Pixel(tile.height<world.waterLevel ? (byte)255 : (byte)0,0,0,0);
				}
			}

			idToUV = new Vector4[tileUVs.Count];
			for(int i = 0;i<idToUV.Length;i++) {
				idToUV[i] = tileUVs[i].uv;
			}

			idTex ??= new Texture(IdTexSize,IdTexSize,FilterMode.Point); //Point Filtering, because we're dealing with bytes
			idTex.SetPixels(idPixels);

			//specularTexture ??= new Texture(IdTexSize,IdTexSize,FilterMode.Bilinear); //Bilinear filtering, for smoothness
			//specularTexture.SetPixels(specularPixels);
			//Material.SetTexture("specularMap",specularTexture);

			//RTDraw
			GLDraw.DrawDelayed(() => {
				var res = new Vector2Int(ChunkSize*32,ChunkSize*32);

				using(var framebuffer = new Framebuffer("TempFramebuffer")) {
					if(rtTexture==null) {
						rtTexture = new RenderTexture($"Chunk_{position.x}_{position.y} RenderTexture",res.x,res.y,FilterMode.Point,TextureWrapMode.Clamp,true);
					}

					framebuffer.AttachRenderTexture(rtTexture);
					GLDraw.SetRenderTarget(framebuffer);

					GLDraw.SetShader(Resources.Find<Shader>("TerrainBatch"));
					GLDraw.SetTextures(new Dictionary<string,Texture> {
						{ "tileMap",idTex },
						{ "tileAtlas",TileType.tileAtlas },
					});
					GLDraw.Uniform1("chunkSize",ChunkSize);
					GLDraw.Uniform4("tileUVs",idToUV); //this is shit

					GLDraw.ClearColor(new Vector4(new Vector3(Rand.Range(0f,1f),Rand.Range(0f,1f),Rand.Range(0f,1f)).Normalized,1f));
					GLDraw.Clear(ClearMask.ColorBufferBit);
					GLDraw.Viewport(0,0,res.x,res.y);

					GLDraw.Begin(PrimitiveType.Quads);

					GLDraw.TexCoord2(1f,0f); GLDraw.Vertex2(-1f, 1f);
					GLDraw.TexCoord2(0f,0f); GLDraw.Vertex2(-1f,-1f);
					GLDraw.TexCoord2(0f,1f); GLDraw.Vertex2( 1f,-1f);
					GLDraw.TexCoord2(1f,1f); GLDraw.Vertex2( 1f, 1f);

					GLDraw.End();

					GLDraw.SetShader(null);
					GLDraw.SetRenderTarget(null);
				}

				Rendering.CheckGLErrors();

				rtTexture.GenerateMipmaps();
				Material.SetTexture("mainTex",rtTexture);
			});
		}
	}
}
