//#define NO_CHUNK_LODS

using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using GameEngine;
using GameEngine.Graphics;
using Debug = GameEngine.Debug;

namespace SurvivalGame
{
	public class Chunk : GameObject,IHasMaterial
	{
		public const int ChunkSize = 16;
		public const float ChunkSizeDiv = 1f/ChunkSize;
		public const float ChunkWorldSize = ChunkSize*TileSize;
		public const float ChunkWorldSizeDiv = 1f/(ChunkSize*TileSize);
		public const float TileSize = 2f;
		public const float TileSizeDiv = 1f/TileSize;
		public const float TileSizeHalf = TileSize*0.5f;

		public World world; //THE WOOOOOOOOORLD that this chunks belongs to
		public Vector2Int position; //Position of this chunk in World.chunks array
		public Vector2Int positionInTiles; //Position of this chunk in World.chunks array,but multiplied by chunkSize.
		public int posId; //Position as if World.chunks array was 1-dimensional
		public Tile[,] tiles; //Only used by multiplayer clients.
		public Mesh mesh;
		public bool updateMesh;
		public bool updateCollisionMesh;
		public bool updateHeights;
		public List<StaticEntity> staticEntities;

		public MeshCollider collider;
		public RenderTexture rtTexture;
		private Texture idTex;
		private Vector4[] idToUV;

		public Vector2Int TilePoint => position*ChunkSize;
		public Vector3 WorldPoint => new Vector3(position.x,0f,position.y)*ChunkSize*TileSize;
		
		private Material material;
		public Material Material {
			get {
				if(material==null) {
					material = Resources.Get<Material>("Terrain.material").Clone();
					GenerateTexture();
				}
				return material;
			}
		}
		private float minHeight;
		public float MinHeight {
			get {
				if(updateHeights) {
					UpdateHeights();
				}
				return minHeight;
			}
		}
		private float maxHeight;
		public float MaxHeight {
			get {
				if(updateHeights) {
					UpdateHeights();
				}
				return maxHeight;
			}
		}

		public Tile this[int x,int y] {
			get => world[positionInTiles.x+x,positionInTiles.y+y];
			set => world[positionInTiles.x+x,positionInTiles.y+y] = value;
		}

		public override void OnInit()
		{
			updateMesh = true;
			updateCollisionMesh = true;
			staticEntities = new List<StaticEntity>();

			layer = Layers.GetLayerIndex("World");
			positionInTiles = position*ChunkSize;
			Transform.Position = new Vector3(position.x*ChunkSize*TileSize,0f,position.y*ChunkSize*TileSize);
			posId = (position.y*(world.xSize/ChunkSize))+position.x;

			if(!Netplay.isHost) {
				tiles = new Tile[ChunkSize,ChunkSize];
				for(int X=0;X<ChunkSize;X++) {
					for(int Y=0;Y<ChunkSize;Y++) {
						tiles[X,Y] = new Tile();
					}
				}
			}

			collider = AddComponent<MeshCollider>();
			collider.Convex = false;
			collider.Mesh = GetMesh();
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
			Material?.Dispose();
			mesh?.Dispose();
		}

		public void Save(BinaryWriter writer)
		{
			for(int y=0;y<ChunkSize;y++) {
				for(int x=0;x<ChunkSize;x++) {
					tiles[x,y].Save(writer);
				}
			}
		}
		public void Load(BinaryReader reader)
		{
			for(int y=0;y<ChunkSize;y++) {
				for(int x=0;x<ChunkSize;x++) {
					tiles[x,y].Load(reader);
				}
			}
		}
		public Mesh GetMesh()
		{
			if(mesh==null) {
				GenerateMesh(0,true,Netplay.isClient);
			}

			return mesh;
		}

		private void UpdateHeights()
		{
			Tile tile;
			float minHeight = 0f;
			float maxHeight = 0f;
			for(int y=0;y<=ChunkSize;y++) {
				for(int x=0;x<=ChunkSize;x++) {
					tile = this[x,y];
					if(x==0 && y==0) {
						minHeight = maxHeight = tile.height;
					}else{
						minHeight = Mathf.Min(minHeight,tile.height);
						maxHeight = Mathf.Max(maxHeight,tile.height);
					}
				}
			}
			this.minHeight = minHeight;
			this.maxHeight = maxHeight;
			updateHeights = false;
		}
		private Mesh GenerateMesh(int lodLevel,bool setAsCollisionMesh,bool generateGrass)
		{
			bool noGraphics = !Netplay.isClient;

			collider.Mesh?.Dispose();
			var meshInfo = new MeshInfo(hasUVs:true);
			var grassMeshes = (!generateGrass || noGraphics) ? null : new Dictionary<string,MeshInfo>();

			int div = lodLevel==0 ? 1 : (int)Math.Pow(2,lodLevel);
			int size = ChunkSize/div;

			Tile tile;
			var vertice2D = new int[size+1,size+1];
			for(int y=0;y<=size;y++) {
				for(int x=0;x<=size;x++) {
					tile = this[x*div,y*div];
					vertice2D[x,y] = meshInfo.vertices.Count;
					meshInfo.vertices.Add(new Vector3(x*TileSize*div,tile.height,y*TileSize*div));
					if(!noGraphics) {
						meshInfo.uvs.Add(new Vector2((y/(float)size),1f-(x/(float)size)));
					}
				}
			}

			for(int y=0;y<=size;y++) {
				for(int x=0;x<=size;x++) {
					int X = x*div;
					int Y = y*div;

					//NormalCalculation
					var vLeft = x==0	? new Vector3((X-1)*TileSize,world[positionInTiles.x+X-1,positionInTiles.y+Y].height,Y*TileSize) : meshInfo.vertices[vertice2D[x-1,y]];
					var vRight = x==size	? new Vector3((X+1)*TileSize,world[positionInTiles.x+X+1,positionInTiles.y+Y].height,Y*TileSize) : meshInfo.vertices[vertice2D[x+1,y]];
					var vHorizontal = (vRight-vLeft).Normalized;

					var vUp = y==0 ? new Vector3(X*TileSize,world[positionInTiles.x+X,positionInTiles.y+Y-1].height,(Y-1)*TileSize) : meshInfo.vertices[vertice2D[x,y-1]];
					var vDown = y==size	? new Vector3(X*TileSize,world[positionInTiles.x+X,positionInTiles.y+Y+1].height,(Y+1)*TileSize) : meshInfo.vertices[vertice2D[x,y+1]];
					var vVertical = (vDown-vUp).Normalized;

					var normal = Vector3.Cross(vVertical,vHorizontal).Normalized;
					meshInfo.normals.Add(normal);

					if(!noGraphics && generateGrass && x<size && y<size) {
						tile = this[x*div,y*div];
						var tileType = TileType.byId[tile.type];
						var localPos = new Vector3(x*TileSize,tile.height,y*TileSize);
						if(tileType.grassMaterial!=null) {
							if(!grassMeshes.TryGetValue(tileType.grassMaterial,out var grassMesh)) {
								grassMeshes[tileType.grassMaterial] = grassMesh = new MeshInfo();
							}
							tileType.ModifyGrassMesh(this,tile,new Vector2Int(x,y),localPos,normal,grassMesh);
						}
					}
				}
			}

			for(int y=0;y<size;y++) {
				for(int x=0;x<size;x++) {
					//int tilePosX = position.x+x;
					//int tilePosY = position.y+y;
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

			/*if(!noGraphics && generateGrass) {
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
			}*/

			return mesh;
		}
		private void GenerateTexture()
		{
			//Preparation
			const int IdTexSize = ChunkSize+2;

			var idPixels = new Pixel[IdTexSize,IdTexSize];
			var tileUVs = new List<(ushort type,byte variant,Vector4 uv)>();
			
			for(int y=-1;y<=ChunkSize;y++) {
				for(int x=-1;x<=ChunkSize;x++) {
					var tileWorld = positionInTiles+new Vector2Int(x,y);
					world.RepeatTilePos(ref tileWorld.x,ref tileWorld.y);

					Tile tile = world[tileWorld.x,tileWorld.y];
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

			idTex ??= new Texture(IdTexSize,IdTexSize,FilterMode.Point); //it's important for this texture to have point filtering
			idTex.SetPixels(idPixels);

			//RTDraw
			GLDraw.DrawDelayed(() => {
				var res = new Vector2Int(512,512);

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

		public static Chunk Create(World world,int x,int y)
		{
			//var w = new World(name,xSize,ySize);
			var chunk = Instantiate<Chunk>(init:false);
			chunk.world = world;
			chunk.position = new Vector2Int(x,y);
			chunk.Init();
			return chunk;
		}

		PhysicMaterial IHasMaterial.GetMaterial(Vector3? atPoint)
		{
			if(!atPoint.HasValue) {
				return PhysicMaterial.GetMaterial<StonePhysicMaterial>();
			}
			var atP = atPoint.Value;
			var tempVec = new Vector2(atP.x,atP.z)/TileSize;
			var tilePoint = new Vector2Int(Mathf.FloorToInt(tempVec.x),Mathf.FloorToInt(tempVec.y));
			var tile = world[tilePoint.x,tilePoint.y];
			var type = TileType.byId[tile.type];
			return type.GetMaterial(atPoint);
		}
	}
}
