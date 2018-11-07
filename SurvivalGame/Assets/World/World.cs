using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using GameEngine;

namespace Game
{
	public struct WorldInfo
	{
		public string name;
		public string displayName;
		public int xSize;
		public int ySize;
		public string localPath;
	}
	public class World : GameObject
	{
		public static readonly char[] fileHeader = { 'I','n','c','a','r','n','a','t','e',' ','W','o','r','l','d',' '};
		
		public readonly string worldName;
		public readonly string worldDisplayName;
		public readonly string path;
		public readonly int xSize;
		public readonly int ySize;
		public readonly int xSizeInChunks;
		public readonly int ySizeInChunks;
		public readonly float xSizeInUnits;
		public readonly float ySizeInUnits;
		public readonly Tile[,] tiles;	//Multiplayer non-host clients store tile arrays in Chunks instead.
		public Chunk[,] chunks;

		public bool IsReady { protected set; get; }

		public Vector2Int Size => new Vector2Int(xSize,ySize);

		public Tile this[int x,int y] {
			get {
				RepeatTilePos(ref x,ref y);
				if(Netplay.isHost) {
					return tiles[x,y];
				}
				var chunkPoint = new Vector2Int(x/Chunk.chunkSize,y/Chunk.chunkSize);
				var relativePos = new Vector2Int(x-(chunkPoint.x*Chunk.chunkSize),y-(chunkPoint.y*Chunk.chunkSize));
				var chunk = chunks[x/Chunk.chunkSize,y/Chunk.chunkSize];
				if(chunk!=null) {
					return chunk.tiles[relativePos.x,relativePos.y];
				}
				return null;
			}
			set {
				RepeatTilePos(ref x,ref y);
				var chunkPoint = new Vector2Int(x/Chunk.chunkSize,y/Chunk.chunkSize);
				Chunk chunk = Netplay.isHost ? null : (chunks[chunkPoint.x,chunkPoint.y] ?? (chunks[chunkPoint.x,chunkPoint.y] = chunk = new Chunk(this,chunkPoint.x,chunkPoint.y)));
				if(Netplay.isHost) {
					tiles[x,y]?.Dispose();
					tiles[x,y] = value;
				}else{
					int X = x-(chunkPoint.x*Chunk.chunkSize);
					int Y = y-(chunkPoint.y*Chunk.chunkSize);
					chunk.tiles[X,Y]?.Dispose();
					chunk.tiles[X,Y] = value;
				}
				if(chunk!=null) {
					chunk.updateMesh = Netplay.isClient;
					chunk.updateCollisionMesh = true;
					chunk.updateHeights = true;
				}
			}
		}

		public World(string name,int xSize,int ySize,string path = null) : base("")
		{
			Debug.Log("Constructor called!");
			if(xSize<=0 || ySize<=0 || xSize%Chunk.chunkSize!=0 || ySize%Chunk.chunkSize!=0) {
				throw new ArgumentException($"World size must be multiple of chunks' size ({Chunk.chunkSize})");
			}
			worldDisplayName = name;
			worldName = new string(worldDisplayName.Select(c => char.IsWhiteSpace(c) ? '_' : c).ToArray());
			this.Name = "World_"+worldName;
			this.path = path ?? Main.savePath+worldName+".wld";
			this.xSize = xSize;
			this.ySize = ySize;
			xSizeInChunks = xSize/Chunk.chunkSize;
			ySizeInChunks = ySize/Chunk.chunkSize;
			xSizeInUnits = xSize*Chunk.tileSize;
			ySizeInUnits = ySize*Chunk.tileSize;

			chunks = new Chunk[xSizeInChunks,ySizeInChunks];
			layer = Layers.GetLayerIndex("World");

			if(Netplay.isHost) {
				tiles = new Tile[xSize,ySize];
			}
		}
		public override void OnInit()
		{
			if(!TileType.initialized) {
				TileType.Initialize();
			}
			
			Debug.Log("Init called!");
			chunks = new Chunk[xSize/Chunk.chunkSize,ySize/Chunk.chunkSize];
			layer = Layers.GetLayerIndex("World");

			//GenerateWorld();
		}
		public void Generate()
		{
			var noise = new PerlinNoise(0,frequency:xSize/64);
			double divX = 1.0/xSize;
			double divY = 1.0/ySize;

			ushort grass = TileType.byName["Grass"].type;
			ushort grassFlowers = TileType.byName["GrassFlowers"].type;
			ushort dirt = TileType.byName["Dirt"].type;
			ushort stone = TileType.byName["Stone"].type;

			for(int y=0;y<ySize;y++) {
				for(int x=0;x<xSize;x++) {
					ushort type = Rand.Next(3)==0 ? grassFlowers : grass;
					float height = noise.GetValue(x*divX,0.0,y*divY)*60f;
					Tile tile = new Tile {
						type = Rand.Next(3)==0 ? grassFlowers : grass,
						height = height
					};
					this[x,y] = tile;
				}
			}
			/*for(int i=0;i<5;i++) {
				var posA = new Vector2Int(Rand.Next(xSize),Rand.Next(ySize));
				var posB = new Vector2Int(Rand.Next(xSize),Rand.Next(ySize));
				LineLoop(posA,posB,pos => this[pos.x,pos.y].type = dirt);
				LineLoop(posA+new Vector2Int(1,0),posB+new Vector2Int(1,0),pos => this[pos.x,pos.y].type = dirt);
			}*/

			var genRoaster = new(int maxRand,Action<Tile,int,int,Vector3> action)[] {
				(25,(t,x,y,spawnPos) => {
					Instantiate<Spruce>(spawnPos);
					t.type = dirt;
				}),
				(60,(t,x,y,spawnPos) => {
					Instantiate<Boulder>(spawnPos);
					for(int i=0;i<20;i++) {
						this[x+Rand.Range(-2,2),y+Rand.Range(-2,2)].type = dirt;
					}
				}),
				(300,(t,x,y,spawnPos) => {
					Instantiate<BerryBush>(spawnPos);
					for(int i=0;i<20;i++) {
						this[x+Rand.Range(-2,2),y+Rand.Range(-2,2)].type = grassFlowers;
					}
					t.type = dirt;
				}),
				(600,(t,x,y,spawnPos) => {
					Instantiate<Campfire>(spawnPos);
					t.type = dirt;
					for(int i = 0;i<3;i++) {
						Instantiate<StoneHatchet>(spawnPos+new Vector3(Rand.Range(-2f,2f),Rand.Range(5f,7f),Rand.Range(-2f,2f)));
					}
				}),
			};

			for(int y=0;y<ySize;y++) {
				for(int x=0;x<xSize;x++) {
					Tile tile = this[x,y];
					if(tile.type==dirt) {
						continue;
					}
					float maxHeightDiff = Mathf.Max(Mathf.Abs(this[x-1,y].height-this[x+1,y].height),Mathf.Abs(this[x,y-1].height-this[x,y+1].height));
					if(maxHeightDiff>=3.2f) {
						tile.type = maxHeightDiff>=4.35f ? stone : dirt;
						continue;
					}
					(int maxRand, var action) = genRoaster[Rand.Next(genRoaster.Length)];
					if(Rand.Next(maxRand)==0) {
						var spawnPos = new Vector3(x*Chunk.tileSize+Chunk.tileSizeHalf,0f,y*Chunk.tileSize+Chunk.tileSizeHalf);
						spawnPos.y = HeightAt(spawnPos,false);
						action(tile,x,y,spawnPos);
					}
				}
			}
			for(int i = 0;i<1000;i++) {
				int x = Rand.Range(1,xSize-1);
				int y = Rand.Range(1,ySize-1);
				Tile tile = tiles[x,y];
				if(tile.type==dirt || tile.type==stone) {
					tile.height += Rand.Range(-1f,4.75f);
					tiles[x-1,y].type = tiles[x,y-1].type = tiles[x-1,y-1].type = tile.type = stone;
				}
			}
			
			Main.LocalEntity = new Human();//Instantiate<Human>(null,new Vector3(xSizeInUnits*0.5f,56f,ySizeInUnits*0.5f));
			var playerPos = new Vector3(xSizeInUnits*0.5f,0f,ySizeInUnits*0.5f);
			playerPos.y = HeightAt(playerPos,false);
			Main.LocalEntity.Transform.Position = playerPos;
			Instantiate<StoneHatchet>(new Vector3(xSizeInUnits*0.5f-1f,45f,ySizeInUnits*0.5f));
			//Instantiate<CubeObj>(null,new Vector3(xSizeInUnits*0.5f-1f,45f,ySizeInUnits*0.5f));

			//new LightObj().transform.parent = Main.camera.transform;

			var sun = new Sun();
			var skybox = new Skybox();

			IsReady = true;
		}
		public void InitChunks()
		{
			for(int y=0;y<ySizeInChunks;y++) {
				for(int x=0;x<xSizeInChunks;x++) {
					chunks[x,y] = new Chunk(this,x,y);
				}
			}
		}
		
		public static World NewWorld(string name,int xSize,int ySize)
		{
			var w = new World(name,xSize,ySize);
			w.Generate();
			w.InitChunks();
			Main.MainMenu = false;
			return (Main.world = w);
		}
		public static void SaveWorld(World w,string path)
		{
			using(var stream = File.OpenWrite(path)) {
				using(var writer = new BinaryWriter(stream)) {
					writer.Write(fileHeader);
					writer.Write(w.worldName);
					writer.Write(w.worldDisplayName);
					writer.Write(w.xSize);
					writer.Write(w.ySize);

					//Make a (chunkId > dataPos) map here
				
					for(int y=0;y<w.ySizeInChunks;y++) {
						for(int x=0;x<w.xSizeInChunks;x++) {
							w.chunks[x,y].Save(writer);
						}
					}
				}
			}
		}
		public static World LoadWorld(string path)
		{
			World w;
			using(var stream = File.OpenRead(path)) {
				using(var reader = new BinaryReader(stream)) {
					if(!ReadInfoHeader(reader,out var info)) {
						throw new IOException("World is corrupt.");
					}
					w = new World(info.displayName,info.xSize,info.ySize,path);

					//Make a (chunkId > dataIOPosition) map here?

					for(int y=0;y<w.ySizeInChunks;y++) {
						for(int x=0;x<w.xSizeInChunks;x++) {
							var chunk = w.chunks[x,y] = new Chunk(w,x,y);
							chunk.Load(reader);
						}
					}
				}
			}
			w.IsReady = true;
			Main.MainMenu = false;
			return null;
		}
		public void LineLoop(Vector2Int pointA,Vector2Int pointB,Action<Vector2Int> action)
		{
			//rewrite copypasta
			var pos = pointA;
			var size = pointB-pointA;
			int x1 = 0, y1 = 0, x2 = 0, y2 = 0;
			if(size.x<0) {
				x1 = -1;
			}else if(size.x>0) {
				x1 = 1;
			}
			if(size.y<0) {
				y1 = -1;
			}else if(size.y>0) {
				y1 = 1;
			}
			if(size.x<0) {
				x2 = -1;
			}else if(size.x>0) {
				x2 = 1;
			}
			int longest = Math.Abs(size.x);
			int shortest = Math.Abs(size.y);
			if(!(longest>shortest)) {
				longest = Math.Abs(size.y);
				shortest = Math.Abs(size.x);
				if(size.y<0) {
					y2 = -1;
				}else if(size.y>0) {
					y2 = 1;
				}
				x2 = 0;
			}
			int numerator = longest >> 1;
			for(int i = 0;i<=longest;i++) {
				action(pos);
				numerator += shortest;
				if(numerator>=longest) {
					numerator -= longest;
					pos.x += x1;
					pos.y += y1;
				} else {
					pos.x += x2;
					pos.y += y2;
				}
			}
		}
		public void RepeatTilePos(ref int x,ref int y)
		{
			//bad
			if(x<0) {
				do {
					x += xSize;
				}
				while(x<0);
			}else if(x>=xSize) {
				do {
					x -= xSize;
				}
				while(x>=xSize);
			}
			if(y<0) {
				do {
					y += ySize;
				}
				while(y<0);
			}else if(y>=ySize) {
				do {
					y -= ySize;
				}
				while(y>=ySize);
			}
		}
		public float HeightAt(Vector3 position,bool tileSpace) => HeightAt(position.x,position.z,tileSpace);
		public float HeightAt(Vector2 position,bool tileSpace) => HeightAt(position.x,position.y,tileSpace);
		public float HeightAt(float x,float y,bool tileSpace)
		{
			if(!tileSpace) {
				x /= Chunk.tileSize;
				y /= Chunk.tileSize;
			}
			int X1 = Mathf.FloorToInt(x);
			int Y1 = Mathf.FloorToInt(y);
			int X2 = X1+1;
			int Y2 = Y1+1;
			x -= X1;
			y -= Y1;
			return Mathf.Lerp(
				Mathf.Lerp(this[X1,Y1].height,this[X2,Y1].height,x),
				Mathf.Lerp(this[X1,Y2].height,this[X2,Y2].height,x),
				y
			);
		}

		public static bool ReadInfoHeader(BinaryReader reader,out WorldInfo info)
		{
			info = default;
			try {
				if(string.Compare(new string(reader.ReadChars(16)),new string(fileHeader))!=0) {
					return false;
				}
				info.name = reader.ReadString();
				info.displayName = reader.ReadString();
				info.xSize = reader.ReadInt32();
				info.ySize = reader.ReadInt32();
				if(info.xSize%Chunk.chunkSize!=0 || info.ySize%Chunk.chunkSize!=0) {
					return false;
				}
				return true;
			}
			catch {
				return false;
			}
		}

		/*public Chunk GetChunkFromTile(int x,int y,out Vector2Int chunkRelativeTilePos)
		{
			RepeatTilePos(ref x,ref y);
			var chunkPoint = new Vector2Int(x/Chunk.chunkSize,y/Chunk.chunkSize);
			chunkRelativeTilePos = new Vector2Int(x-(chunkPoint.x*Chunk.chunkSize),y-(chunkPoint.y*Chunk.chunkSize));
			return chunks[x/Chunk.chunkSize,y/Chunk.chunkSize];
		}
		public Chunk GetChunkFromTile(int x,int y)
		{
			RepeatTilePos(ref x,ref y);
			return chunks[x/Chunk.chunkSize,y/Chunk.chunkSize];
		}*/
	}
}

