//#define LOOP_WORLD

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameEngine;
using GameEngine.Physics;
using ImmersionFramework;

namespace SurvivalGame
{
	public abstract partial class World : GameObject, IWorld
	{
		public static readonly char[] FileHeader = { 'I','n','c','a','r','n','a','t','e',' ','W','o','r','l','d',' '};
		
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

		private bool forceChunkLoad = true;

		public bool IsReady { protected set; get; }

		public Vector2Int Size => new Vector2Int(xSize,ySize);
		public Vector2Int SizeInChunks => new Vector2Int(xSizeInChunks,ySizeInChunks);
		public Vector2 SizeInUnits => new Vector2(xSizeInUnits,ySizeInUnits);

		public ref Tile this[int x,int y] {
			get {
				RepeatTilePos(ref x,ref y);

				if(Netplay.isHost) {
					return ref tiles[x,y];
				}

				var chunkPoint = new Vector2Int(x/Chunk.ChunkSize,y/Chunk.ChunkSize);
				var relativePos = new Vector2Int(x-(chunkPoint.x*Chunk.ChunkSize),y-(chunkPoint.y*Chunk.ChunkSize));
				var chunk = chunks[x/Chunk.ChunkSize,y/Chunk.ChunkSize];
				return ref chunk.tiles[relativePos.x,relativePos.y];
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

			//temp
			InitChunks();
		}
		public override void FixedUpdate()
		{
			if(!Netplay.isClient || (!forceChunkLoad && Time.FixedUpdateCount%60!=0)) {
				return;
			}

			if(!Player.TryGetLocalPlayer(0,out var localPlayer) || !localPlayer.TryGetEntity(out var localPlayerEntity)) {
				return;
			}

			Vector2Int cameraPos = (Vector2Int)(localPlayerEntity.Transform.Position.XZ/Chunk.ChunkWorldSize);
			//Vector2Int cameraPos = (Vector2Int)(Main.camera.Transform.Position.XZ/Chunk.ChunkWorldSize);

			const int Range = 24;

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

			var toDispose = new List<Vector2Int>(chunkRenderers.Keys);

			for(int y = yStart;y<=yEnd;y++) {
				for(int x = xStart;x<=xEnd;x++) {
					Vector2Int vecPos = new Vector2Int(x,y);

					if(chunkRenderers.ContainsKey(vecPos)) {
						toDispose.Remove(vecPos);
						continue;
					}

					Vector2Int chunkPos = vecPos;
#if LOOP_WORLD
					RepeatChunkPos(ref chunkPos.x,ref chunkPos.y);
#endif

					chunkRenderers[vecPos] = ChunkRenderer.Create(chunks[chunkPos.x,chunkPos.y],vecPos.x,vecPos.y);
				}
			}

			if(toDispose.Count>0) {
				foreach(var point in toDispose) {
					var renderer = chunkRenderers[point];
					renderer.Dispose();
					chunkRenderers.Remove(point);
				}

				Debug.Log($"Disposed {toDispose.Count} chunk renderers");
			}

			forceChunkLoad = false;
		}

		public void InitChunks()
		{
			for(int y = 0;y<ySizeInChunks;y++) {
				for(int x = 0;x<xSizeInChunks;x++) {
					chunks[x,y] = Chunk.Create(this,x,y);
				}
			}
		}
		public void FinishChunkInit()
		{
			for(int y = 0;y<ySizeInChunks;y++) {
				for(int x = 0;x<xSizeInChunks;x++) {
					chunks[x,y].FinishInit();
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

		public static T Instantiate<T>(string name,int xSize,int ySize,string path = null) where T : World
		{
			var world = Instantiate<T>(init: false);
			world.worldDisplayName = name;
			world.worldName = new string(name.Select(c => char.IsWhiteSpace(c) ? '_' : c).ToArray());
			world.xSize = xSize;
			world.ySize = ySize;
			world.path = path ?? Main.savePath+world.worldName+".wld";
			world.Init();
			return world;
		}
		public static T NewWorld<T>(string name,int xSize,int ySize,string path = null,int? seed = null) where T : World
		{
			var world = Instantiate<T>(name,xSize,ySize,path);

			world.Generate(seed ?? ((int)DateTime.Now.Ticks));
			world.FinishChunkInit();

			Main.MainMenu = false;

			return world;
		}
	}
}

