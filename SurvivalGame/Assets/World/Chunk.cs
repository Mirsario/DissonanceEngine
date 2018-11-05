#define NO_CHUNK_LODS

using System;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using GameEngine;
using Debug = GameEngine.Debug;

namespace Game
{
	public class Chunk : GameObject,IHasMaterial
	{
		public const int chunkSize = 16;
		public const float tileSize = 2f;
		public const float tileSizeHalf = tileSize*0.5f;

		public Vector2Int TilePoint	=> position*chunkSize;
		public Vector3 WorldPoint => new Vector3(position.x,0f,position.y)*chunkSize*tileSize;

		public readonly World world;				//THEEE WOOOOOOOOORLD!!! ..that this chunks belongs to
		public readonly Vector2Int position;		//Position of this chunk in World.chunks array
		public readonly Vector2Int positionInTiles;	//Position of this chunk in World.chunks array,but multiplied by chunkSize.
		public readonly int posId;					//Position,as if World.chunks array was 1-dimensional
		public readonly Tile[,] tiles;				//Only used by multiplayer clients.
		public Mesh mesh;
		public bool updateMesh;
		public bool updateCollisionMesh;
		public bool updateHeights;

		public MeshRenderer renderer;
		public MeshRenderer[] grassRenderers;
		public MeshCollider collider;
		public RenderTexture rtTexture;
		private Texture idTex;
		private Vector4[] idToUV;

		private float _minHeight;
		public float MinHeight {
			get {
				if(updateHeights) {
					UpdateHeights();
				}
				return _minHeight;
			}
		}
		private float _maxHeight;
		public float MaxHeight {
			get {
				if(updateHeights) {
					UpdateHeights();
				}
				return _maxHeight;
			}
		}

		public Tile this[int x,int y] {
			get => world[positionInTiles.x+x,positionInTiles.y+y];
			set => world[positionInTiles.x+x,positionInTiles.y+y] = value;
		}

		public Chunk(World world,int x,int y) : base("Chunk_"+x+"_"+y)
		{
			collider = AddComponent<MeshCollider>();
			collider.Convex = false;

			if(Netplay.isClient) {
				renderer = AddComponent<MeshRenderer>();
				renderer.Material = Resources.Get<Material>("Terrain.material").Clone();
				//renderer.Material.Shader.polygonMode = PolygonMode.Line;
			}
			updateMesh = true;
			updateCollisionMesh = true;

			layer = Layers.GetLayerIndex("World");
			this.world = world;
			position = new Vector2Int(x,y);
			positionInTiles = position*chunkSize;
			Transform.Position = new Vector3(position.x*chunkSize*tileSize,0f,position.y*chunkSize*tileSize);
			posId = (y*(world.xSize/chunkSize))+x;

			if(!Netplay.isHost) {
				tiles = new Tile[chunkSize,chunkSize];
				for(int X=0;X<chunkSize;X++) {
					for(int Y=0;Y<chunkSize;Y++) {
						tiles[X,Y] = new Tile();
					}
				}
			}
			
			#if NO_CHUNK_LODS
			var mesh = GenerateMesh(0,true,Netplay.isClient);
			if(Netplay.isClient) {
				GenerateTextures();
				renderer.Mesh = mesh;
			}
			#else
			float maxViewDistance = 256f;
			var lods = Netplay.isClient ? new MeshLOD[4] : null;
			for(int i=0;i<lods.Length;i++) {
				var mesh = GenerateMesh(i,i==0,i==0 && Netplay.isClient);
				if(Netplay.isClient) {
					lods[i] = new MeshLOD(mesh,i==lods.Length-1 ? 0f : (maxViewDistance/lods.Length)*(2f+i));
				}
			}
			if(Netplay.isClient) {
				GenerateTextures();
				renderer.LODMeshes = lods;
			}
			#endif
		}
		public override void FixedUpdate()
		{
			if(Main.MainMenu || !world.IsReady) {}
			/*if(updateCollisionMesh) {
				GenerateMesh(true);
				updateCollisionMesh = false;
			}*/
		}
		public override void RenderUpdate()
		{
			if(Main.MainMenu || !Netplay.isClient || !world.IsReady) {}
			/*if(updateMesh) {
				updateMesh = false;
			}*/
		}
		public override void OnDispose()
		{
			renderer.Material?.Dispose();
			renderer.Dispose();
		}

		public void Save(BinaryWriter writer)
		{
			/*for(int y=0;y<chunkSize;y++) {
				for(int x=0;x<chunkSize;x++) {
					tiles[x,y].Save(writer);
				}
			}*/
		}
		public void Load(BinaryReader reader)
		{
			/*for(int y=0;y<chunkSize;y++) {
				for(int x=0;x<chunkSize;x++) {
					tiles[x,y].Load(reader);
				}
			}*/
		}

		private void UpdateHeights()
		{
			Tile tile;
			float minHeight = 0f;
			float maxHeight = 0f;
			for(int y=0;y<=chunkSize;y++) {
				for(int x=0;x<=chunkSize;x++) {
					tile = this[x,y];
					if(x==0 && y==0) {
						minHeight = maxHeight = tile.height;
					}else{
						minHeight = Mathf.Min(minHeight,tile.height);
						maxHeight = Mathf.Max(maxHeight,tile.height);
					}
				}
			}
			_minHeight = minHeight;
			_maxHeight = maxHeight;
			updateHeights = false;
		}
		public Mesh GenerateMesh(int lodLevel,bool setAsCollisionMesh,bool generateGrass)
		{
			bool noGraphics=	!Netplay.isClient;

			if(collider.Mesh!=null) {
				collider.Mesh.Dispose();
			}
			var meshInfo = new MeshInfo(hasUVs:true);
			var grassMeshes = (!generateGrass || noGraphics) ? null : new Dictionary<string,MeshInfo>();

			int div = lodLevel==0 ? 1 : (int)Math.Pow(2,lodLevel);
			int size = chunkSize/div;

			Tile tile;
			var vertice2D = new int[size+1,size+1];
			for(int y=0;y<=size;y++) {
				for(int x=0;x<=size;x++) {
					tile = this[x*div,y*div];
					vertice2D[x,y] = meshInfo.vertices.Count;
					meshInfo.vertices.Add(new Vector3(x*tileSize*div,tile.height,y*tileSize*div));
					if(!noGraphics) {
						meshInfo.uvs.Add(new Vector2((y/(float)size),1f-(x/(float)size)));
					}
				}
			}
			for(int y=0;y<=size;y++) {
				for(int x=0;x<=size;x++) {
					int X = x*div;
					int Y = y*div;

					#region NormalCalculation
					var vLeft = x==0	? new Vector3((X-1)*tileSize,world[positionInTiles.x+X-1,positionInTiles.y+Y].height,Y*tileSize) : meshInfo.vertices[vertice2D[x-1,y]];
					var vRight = x==size	? new Vector3((X+1)*tileSize,world[positionInTiles.x+X+1,positionInTiles.y+Y].height,Y*tileSize) : meshInfo.vertices[vertice2D[x+1,y]];
					var vHorizontal = (vRight-vLeft).Normalized;

					var vUp = y==0 ? new Vector3(X*tileSize,world[positionInTiles.x+X,positionInTiles.y+Y-1].height,(Y-1)*tileSize) : meshInfo.vertices[vertice2D[x,y-1]];
					var vDown = y==size	? new Vector3(X*tileSize,world[positionInTiles.x+X,positionInTiles.y+Y+1].height,(Y+1)*tileSize) : meshInfo.vertices[vertice2D[x,y+1]];
					var vVertical = (vDown-vUp).Normalized;

					var normal = Vector3.Cross(vVertical,vHorizontal).Normalized;
					meshInfo.normals.Add(normal);
					#endregion

					if(!noGraphics && generateGrass && x<size && y<size) {
						tile = this[x*div,y*div];
						var tileType = TileType.byId[tile.type];
						var localPos = new Vector3(x*tileSize,tile.height,y*tileSize);
						if(tileType.grassMaterial!=null) {
							if(!grassMeshes.TryGetValue(tileType.grassMaterial,out var grassMesh)) {
								grassMeshes[tileType.grassMaterial] = grassMesh = new MeshInfo();
							}
							tileType.ModifyGrassMesh(this,tile,new Vector2Int(x,y),localPos,normal,grassMesh);
						}
					}
				}
			}
			for(var y=0;y<size;y++) {
				for(var x=0;x<size;x++) {
					int tilePosX = position.x+x;
					int tilePosY = position.y+y;
					meshInfo.triangles.Add(vertice2D[x,y+1]);
					meshInfo.triangles.Add(vertice2D[x+1,y+1]);
					meshInfo.triangles.Add(vertice2D[x,y]);
					meshInfo.triangles.Add(vertice2D[x+1,y]);
					meshInfo.triangles.Add(vertice2D[x,y]);
					meshInfo.triangles.Add(vertice2D[x+1,y+1]);
				}
			}

			if(meshInfo.vertices.Count==0) {
				mesh = null;
			}else{
				mesh = new Mesh();
				meshInfo.ApplyToMesh(mesh);
				if(!noGraphics) {
					mesh.RecalculateTangents();
				}
				mesh.Apply();
			}
			if(setAsCollisionMesh) {
				collider.Mesh = mesh;
			}
			if(!noGraphics && generateGrass) {
				var materialDict = grassMeshes.Select(p => new KeyValuePair<Material,MeshInfo>(Resources.Find<Material>(p.Key),p.Value)).ToDictionary(p => p.Key,p => p.Value);
				var grassRenderers = this.grassRenderers!=null ? this.grassRenderers.ToList() : new List<MeshRenderer>();
				for(int i=0;i<grassRenderers.Count;i++) {
					var renderer = grassRenderers[i];
					if(materialDict.TryGetValue(renderer.Material,out var grassMeshInfo)) {
						grassMeshInfo.ApplyToMesh(renderer.Mesh);
						renderer.Mesh.Apply();
						materialDict.Remove(renderer.Material);
					}else{
						renderer.Dispose();
						grassRenderers.RemoveAt(i);
						i--;
					}
				}
				foreach(var pair in materialDict) {
					var newRenderer = AddComponent<MeshRenderer>();
					grassRenderers.Add(newRenderer);
					var newMesh = new Mesh();
					pair.Value.ApplyToMesh(newMesh);
					newMesh.Apply();
					newRenderer.LODMesh = new MeshLOD(newMesh,192f);//hardcoded!
					newRenderer.Material = pair.Key;
				}
				materialDict.Clear();
				this.grassRenderers = grassRenderers.ToArray();
			}
			return mesh;
		}
		public void GenerateTextures()
		{
			if(posId==0) {
				Debug.Log("GenerateTextures called");
			}
			#region Preparation
			int idTexSize = chunkSize+2;
			var idPixels = new Pixel[idTexSize,idTexSize];
			List<(ushort type,byte variant,Vector4 uv)> tileUVs = new List<(ushort,byte,Vector4)>();
			
			Tile tile;
			for(int y=-1;y<=chunkSize;y++) {
				for(int x=-1;x<=chunkSize;x++) {
					var tileWorld = positionInTiles+new Vector2Int(x,y);
					world.RepeatTilePos(ref tileWorld.x,ref tileWorld.y);
					tile = world[tileWorld.x,tileWorld.y];
					var type = TileType.byId[tile.type];
					byte variant = type.GetVariant(tileWorld.x,tileWorld.y);
					int index;
					if(!tileUVs.Any(q => q.type==tile.type && q.variant==variant)) {
						index = tileUVs.Count;
						tileUVs.Add((tile.type,variant,type.variantUVs[variant]));
					}else{
						index = tileUVs.FindIndex(q => q.type==tile.type && q.variant==variant);
					}
					idPixels[x+1,y+1] = new Pixel((byte)index,255,255,255);
				}
			}
			idToUV = new Vector4[tileUVs.Count];
			for(int i=0;i<idToUV.Length;i++) {
				idToUV[i] = tileUVs[i].uv;
			}

			if(idTex==null) {
				idTex = new Texture(idTexSize,idTexSize,FilterMode.Point);//it's important for this texture to have point filtering
			}
			idTex.SetPixels(idPixels);
			#endregion

			#region RTDraw
			GLDraw.DrawDelayed((Action)(() => {
				if(this==null) {
					return;
				}
				var res = new Vector2Int(512,512);
				if(rtTexture==null) {
					rtTexture = new RenderTexture(res.x,res.y,FilterMode.Point,TextureWrapMode.Clamp,true);
				}

				GLDraw.SetRenderTarget(rtTexture);
				GLDraw.SetShader(Resources.Find<Shader>("TerrainBatch"));
				GLDraw.SetTextures(new Dictionary<string,Texture> {
					{ "tileMap",idTex },
					{ "tileAtlas",TileType.tileAtlas },
				});
				GLDraw.Uniform4("tileUVs",idToUV);//this is shit

				GLDraw.ClearColor(new Vector4(new Vector3(Rand.Range(0f,1f),Rand.Range(0f,1f),Rand.Range(0f,1f)).Normalized,1f));
				GLDraw.Clear(ClearMask.ColorBufferBit);
				GLDraw.Viewport(0,0,res.x,res.y);
				GLDraw.Begin(PrimitiveType.Quads);
				GLDraw.TexCoord2(0.0f,1.0f);
				GLDraw.Vertex2(	-1.0f,1.0f);
				GLDraw.TexCoord2(0.0f,0.0f);
				GLDraw.Vertex2(	-1.0f,-1.0f);
				GLDraw.TexCoord2(1.0f,0.0f);
				GLDraw.Vertex2(	1.0f,-1.0f);
				GLDraw.TexCoord2(1.0f,1.0f);
				GLDraw.Vertex2(	1.0f,1.0f);
				GLDraw.End();

				GLDraw.SetShader(null);
				GLDraw.SetRenderTarget(null);

				//Debug.Log("Drawing finished!");
				Graphics.CheckGLErrors();
				//Debug.Log("Checked errors!");

				rtTexture.GenerateMipmaps();
				GetComponent<MeshRenderer>().Material.SetTexture("mainTex",rtTexture);
			}));
			#endregion
		}

		PhysicMaterial IHasMaterial.GetMaterial(Vector3? atPoint)
		{
			if(atPoint==null) {
				return PhysicMaterial.GetMaterial<StonePhysicMaterial>();
			}
			var atP = atPoint.Value;
			var tempVec = new Vector2(atP.x,atP.z)/tileSize;
			var tilePoint = new Vector2Int(Mathf.FloorToInt(tempVec.x),Mathf.FloorToInt(tempVec.y));
			var tile = world[tilePoint.x,tilePoint.y];
			var type = TileType.byId[tile.type];
			return type.GetMaterial(atPoint);
		}
	}
}
