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
	public partial class Chunk : GameObject, IHasMaterial
	{
		public const int ChunkSize = 64;
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
		public Texture specularTexture;
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
		}
		public override void OnDispose()
		{
			Material?.Dispose();
			mesh?.Dispose();
		}

		public void FinishInit()
		{
			/*collider = AddComponent<MeshCollider>(c => {
				c.Mesh = (ConcaveCollisionMesh)GetMesh();
			});*/
		}

		public static Chunk Create(World world,int x,int y,bool init = true)
		{
			//var w = new World(name,xSize,ySize);
			var chunk = Instantiate<Chunk>(init:false);
			chunk.world = world;
			chunk.position = new Vector2Int(x,y);

			if(init) {
				chunk.Init();
			}

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
