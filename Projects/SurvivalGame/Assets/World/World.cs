//#define LOOP_WORLD

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameEngine;

namespace SurvivalGame
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
		
		public string worldName;
		public string worldDisplayName;
		public string path;
		public int xSize;
		public int ySize;
		public int xSizeInChunks;
		public int ySizeInChunks;
		public float xSizeInUnits;
		public float ySizeInUnits;
		public float waterLevel;
		public Tile[,] tiles; //Multiplayer non-host clients store tile arrays in Chunks instead.
		public Chunk[,] chunks;
		public Dictionary<Vector2Int,ChunkRenderer> chunkRenderers;

		public bool IsReady { protected set; get; }

		public Vector2Int Size => new Vector2Int(xSize,ySize);
		public Vector2Int SizeInChunks => new Vector2Int(xSizeInChunks,ySizeInChunks);
		public Vector2 SizeInUnits => new Vector2(xSizeInUnits,ySizeInUnits);

		public Tile this[int x,int y] {
			get {
				RepeatTilePos(ref x,ref y);

				if(Netplay.isHost) {
					return tiles[x,y];
				}

				var chunkPoint = new Vector2Int(x/Chunk.ChunkSize,y/Chunk.ChunkSize);
				var relativePos = new Vector2Int(x-(chunkPoint.x*Chunk.ChunkSize),y-(chunkPoint.y*Chunk.ChunkSize));
				var chunk = chunks[x/Chunk.ChunkSize,y/Chunk.ChunkSize];
				return chunk?.tiles[relativePos.x,relativePos.y];
			}
			set {
				RepeatTilePos(ref x,ref y);

				var chunkPoint = new Vector2Int(x/Chunk.ChunkSize,y/Chunk.ChunkSize);
				Chunk chunk = Netplay.isHost ? null : (chunks[chunkPoint.x,chunkPoint.y] ?? (chunks[chunkPoint.x,chunkPoint.y] = Chunk.Create(this,chunkPoint.x,chunkPoint.y)));

				if(Netplay.isHost) {
					tiles[x,y]?.Dispose();
					tiles[x,y] = value;
				}else{
					int X = x-(chunkPoint.x*Chunk.ChunkSize);
					int Y = y-(chunkPoint.y*Chunk.ChunkSize);
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

		public override void OnInit()
		{
			Debug.Log("Constructor called!");
			
			if(xSize<=0 || ySize<=0 || xSize%Chunk.ChunkSize!=0 || ySize%Chunk.ChunkSize!=0) {
				throw new ArgumentException($"World size must be multiple of chunks' size ({Chunk.ChunkSize})");
			}

			Name = "World_"+worldName;
			xSizeInChunks = xSize/Chunk.ChunkSize;
			ySizeInChunks = ySize/Chunk.ChunkSize;
			xSizeInUnits = xSize*Chunk.TileSize;
			ySizeInUnits = ySize*Chunk.TileSize;

			chunks = new Chunk[xSizeInChunks,ySizeInChunks];
			layer = Layers.GetLayerIndex("World");

			if(Netplay.isHost) {
				tiles = new Tile[xSize,ySize];
			}
			
			if(!TileType.initialized) {
				TileType.Initialize();
			}
			
			Debug.Log("Init called!");
			chunks = new Chunk[xSize/Chunk.ChunkSize,ySize/Chunk.ChunkSize];
			layer = Layers.GetLayerIndex("World");

			if(Netplay.isClient) {
				chunkRenderers = new Dictionary<Vector2Int,ChunkRenderer>();
			}
		}
		public override void FixedUpdate()
		{
			if(!Netplay.isClient || Main.camera==null) {
				return;
			}
			
			Vector2Int cameraPos = (Vector2Int)(Main.camera.Transform.Position.XZ/Chunk.ChunkWorldSize);

			const int Range = 16;

#if LOOP_WORLD
			int xStart = cameraPos.x-Range;
			int yStart = cameraPos.y-Range;
			int xEnd = cameraPos.x+Range;
			int yEnd = cameraPos.y+Range;
#else
			int xStart = Mathf.Clamp(cameraPos.x-Range,0,xSizeInChunks-1);
			int yStart = Mathf.Clamp(cameraPos.y-Range,0,ySizeInChunks-1);
			int xEnd = Mathf.Clamp(cameraPos.x+Range,0,xSizeInChunks-1);
			int yEnd = Mathf.Clamp(cameraPos.y+Range,0,ySizeInChunks-1);
#endif

			for(int y = yStart;y<=yEnd;y++) {
				for(int x = xStart;x<=xEnd;x++) {
					Vector2Int vecPos = new Vector2Int(x,y);

					if(chunkRenderers.ContainsKey(vecPos)) {
						continue;
					}

					Vector2Int chunkPos = vecPos;
#if LOOP_WORLD
					RepeatChunkPos(ref chunkPos.x,ref chunkPos.y);
#endif

					chunkRenderers[vecPos] = ChunkRenderer.Create(chunks[chunkPos.x,chunkPos.y],vecPos.x,vecPos.y);
				}
			}
		}

		public void Generate()
		{
			var noise = new PerlinNoiseFloat(Rand.Next(10000),frequency:xSize/64f);
			float divX = 1f/xSize;
			float divY = 1f/ySize;

			ushort grass = TileType.byName["Grass"].type;
			ushort grassFlowers = TileType.byName["GrassFlowers"].type;
			ushort dirt = TileType.byName["Dirt"].type;
			ushort stone = TileType.byName["Stone"].type;
			ushort sand = TileType.byName["Sand"].type;

			waterLevel = 32f;

			float beachLevel = waterLevel+1.5f;

			for(int y=0;y<ySize;y++) {
				for(int x=0;x<xSize;x++) {
					var tile = new Tile {
						height = noise.GetValue(x*divX,0f,y*divY)*60f
					};

					tile.type = tile.height<=beachLevel ? sand : (Rand.Next(3)==0 ? grassFlowers : grass);

					this[x,y] = tile;
				}
			}

			/*for(int i=0;i<5;i++) {
				var posA = new Vector2Int(Rand.Next(xSize),Rand.Next(ySize));
				var posB = new Vector2Int(Rand.Next(xSize),Rand.Next(ySize));
				LineLoop(posA,posB,pos => this[pos.x,pos.y].type = dirt);
				LineLoop(posA+new Vector2Int(1,0),posB+new Vector2Int(1,0),pos => this[pos.x,pos.y].type = dirt);
			}*/

			var genRoster = new(int maxRand,Action<Tile,int,int,Vector3> action)[] {
				(25,(t,x,y,spawnPos) => {
					if(spawnPos.y<=beachLevel) {
						return;
					}
					Entity.Instantiate<Spruce>(this,position:spawnPos,rotation:Quaternion.FromEuler(0f,Rand.Range(0f,360f),0f));
					t.type = dirt;
				}),
				(60,(t,x,y,spawnPos) => {
					Entity.Instantiate<Boulder>(this,position:spawnPos,rotation:Quaternion.FromEuler(0f,Rand.Range(0f,360f),0f));
					for(int i=0;i<20;i++) {
						this[x+Rand.Range(-2,2),y+Rand.Range(-2,2)].type = dirt;
					}
				}),
				(300,(t,x,y,spawnPos) => {
					if(spawnPos.y<=beachLevel) {
						return;
					}
					Entity.Instantiate<BerryBush>(this,position:spawnPos);
					for(int i=0;i<20;i++) {
						this[x+Rand.Range(-2,2),y+Rand.Range(-2,2)].type = grassFlowers;
					}
					t.type = dirt;
				}),
				(600,(t,x,y,spawnPos) => {
					if(spawnPos.y<=beachLevel) {
						return;
					}

					Entity.Instantiate<Campfire>(this,position:spawnPos);
					t.type = dirt;
					for(int i = 0;i<3;i++) {
						Entity.Instantiate<StoneHatchet>(this,position:spawnPos+new Vector3(Rand.Range(-2f,2f),Rand.Range(5f,7f),Rand.Range(-2f,2f)));
					}
				}),
			};

			for(int y=0;y<ySize;y++) {
				for(int x=0;x<xSize;x++) {
					Tile tile = this[x,y];
					if(tile.type==dirt) {
						continue;
					}

					if(tile.height>beachLevel) {
						float maxHeightDiff = Mathf.Max(Mathf.Abs(this[x-1,y].height-this[x+1,y].height),Mathf.Abs(this[x,y-1].height-this[x,y+1].height));
						if(maxHeightDiff>=3.2f) {
							tile.type = maxHeightDiff>=4.35f ? stone : dirt;
							continue;
						}
					}

					(int maxRand, var action) = genRoster[Rand.Next(genRoster.Length)];
					if(Rand.Next(maxRand)==0) {
						var spawnPos = new Vector3(x*Chunk.TileSize+Chunk.TileSizeHalf,0f,y*Chunk.TileSize+Chunk.TileSizeHalf);
						spawnPos.y = HeightAt(spawnPos,false);
						action(tile,x,y,spawnPos);
					}
				}
			}

			//Random stone pikes
			for(int i = 0;i<1000;i++) {
				int x = Rand.Range(1,xSize-1);
				int y = Rand.Range(1,ySize-1);
				Tile tile = tiles[x,y];
				if(tile.type==dirt || tile.type==stone) {
					tile.height += Rand.Range(-1f,4.75f);
					tiles[x-1,y].type = tiles[x,y-1].type = tiles[x-1,y-1].type = tile.type = stone;
				}
			}

			var playerPos = new Vector3(xSizeInUnits*0.5f,0f,ySizeInUnits*0.5f);
			playerPos.y = HeightAt(playerPos,false);

			Main.LocalEntity = Entity.Instantiate<Human>(this,position:playerPos); //Instantiate<Human>(null,new Vector3(xSizeInUnits*0.5f,56f,ySizeInUnits*0.5f));

			Entity.Instantiate<StoneHatchet>(this,position:new Vector3(xSizeInUnits*0.5f-1f,45f,ySizeInUnits*0.5f));
			Instantiate<AtmosphereSystem>();
			Instantiate<Sun>();
			Instantiate<Skybox>(); //TODO: Implement a skybox inside the engine
			Entity.Instantiate<Water>(this,position:new Vector3(xSizeInUnits*0.5f,32f,ySizeInUnits*0.5f));

			IsReady = true;
		}
		public void InitChunks()
		{
			for(int y=0;y<ySizeInChunks;y++) {
				for(int x=0;x<xSizeInChunks;x++) {
					chunks[x,y] = Chunk.Create(this,x,y);
				}
			}
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
				do { x += xSize; } while(x<0);
			}else while(x>=xSize) {
				x -= xSize;
			}

			if(y<0) {
				do { y += ySize; } while(y<0);
			}else while(y>=ySize) {
				y -= ySize;
			}
		}
		public void RepeatChunkPos(ref int x,ref int y)
		{
			//bad
			if(x<0) {
				do { x += xSizeInChunks; } while(x<0);
			}else while(x>=xSizeInChunks) {
				x -= xSizeInChunks;
			}

			if(y<0) {
				do { y += ySizeInChunks; } while(y<0);
			}else while(y>=ySizeInChunks) {
				y -= ySizeInChunks;
			}
		}
		public float HeightAt(Vector3 position,bool tileSpace) => HeightAt(position.x,position.z,tileSpace);
		public float HeightAt(Vector2 position,bool tileSpace) => HeightAt(position.x,position.y,tileSpace);
		public float HeightAt(float x,float y,bool tileSpace)
		{
			if(!tileSpace) {
				x /= Chunk.TileSize;
				y /= Chunk.TileSize;
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

		public Chunk GetChunkAt(float x,float y)
		{
			int X = Mathf.FloorToInt(Mathf.Repeat(x*Chunk.ChunkWorldSizeDiv,xSizeInChunks));
			int Y = Mathf.FloorToInt(Mathf.Repeat(y*Chunk.ChunkWorldSizeDiv,ySizeInChunks));
			return chunks[X,Y];
		}
		public float GetWaterLevelAt(Vector3 position) => GetWaterLevelAt(position.x,position.z);
		public float GetWaterLevelAt(float x,float z) => waterLevel+Mathf.Lerp(-2f,2f,Mathf.Sin(Time.GameTime+(x+z)*0.1f)*0.5f+0.5f);

		public static World Instantiate(string name,int xSize,int ySize,string path = null)
		{
			var world = Instantiate<World>(init: false);
			world.worldDisplayName = name;
			world.worldName = new string(name.Select(c => char.IsWhiteSpace(c) ? '_' : c).ToArray());
			world.xSize = xSize;
			world.ySize = ySize;
			world.path = path ?? Main.savePath+world.worldName+".wld";
			world.Init();
			return world;
		}
		public static World NewWorld(string name,int xSize,int ySize,string path = null)
		{
			var world = Instantiate(name,xSize,ySize,path);

			world.Generate();
			world.InitChunks();
			Main.MainMenu = false;
			return world;
		}
		public static void SaveWorld(World w,string path)
		{
			//WIP
			using var stream = File.OpenWrite(path);
			using var writer = new BinaryWriter(stream);

			writer.Write(fileHeader);
			writer.Write(w.worldName);
			writer.Write(w.worldDisplayName);
			writer.Write(w.xSize);
			writer.Write(w.ySize);

			//Make a (chunkId > dataPos) map here

			for(int y = 0;y<w.ySizeInChunks;y++) {
				for(int x = 0;x<w.xSizeInChunks;x++) {
					w.chunks[x,y].Save(writer);
				}
			}
		}
		public static World LoadWorld(string path)
		{
			//WIP
			using var stream = File.OpenRead(path);
			using var reader = new BinaryReader(stream);

			if(!ReadInfoHeader(reader,out var info)) {
				throw new IOException("World is corrupt.");
			}

			var w = Instantiate(info.displayName,info.xSize,info.ySize,path);

			//Make a (chunkId > dataIOPosition) map here?

			for(int y = 0;y<w.ySizeInChunks;y++) {
				for(int x = 0;x<w.xSizeInChunks;x++) {
					var chunk = w.chunks[x,y] = Chunk.Create(w,x,y);
					chunk.Load(reader);
				}
			}

			w.IsReady = true;
			Main.MainMenu = false;

			return null;
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
				if(info.xSize%Chunk.ChunkSize!=0 || info.ySize%Chunk.ChunkSize!=0) {
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

