using System;
using System.Collections.Generic;
using AbyssCrusaders.Core.Generation;
using GameEngine;

namespace AbyssCrusaders.Core
{
	public abstract partial class World : GameObject2D
	{
		public string displayName;
		public string localPath;
		public int width;
		public int height;
		public Chunk[,] chunks;
		public List<Chunk> visibleChunks;
		public Vector2Int spawnPoint;

		public int ChunkWidth => width/Chunk.ChunkSize;
		public int ChunkHeight => height/Chunk.ChunkSize;

		public ref Tile this[int x,int y] {
			get {
				x = x>=0 ? (x<width ? x : x%width) : (int.MaxValue+x+1)%width;
				y = y>=0 ? (y<height ? y : y%height) : (int.MaxValue+y+1)%height;

				int chunkX = x/Chunk.ChunkSize;
				int chunkY = y/Chunk.ChunkSize;

				var chunk = chunks[chunkX,chunkY];

				if(chunk==null) {
					chunks[chunkX,chunkY] = chunk = new Chunk(this,new Vector2Int(chunkX,chunkY));
				}

				return ref chunk.tiles[x-chunkX*Chunk.ChunkSize,y-chunkY*Chunk.ChunkSize];
			}
		}
		
		public override void OnInit()
		{
			int chunkWidth = Math.Max(1,width/Chunk.ChunkSize);
			int chunkHeight = Math.Max(1,height/Chunk.ChunkSize);
			width = chunkWidth*Chunk.ChunkSize;
			height = chunkHeight*Chunk.ChunkSize;
			chunks = new Chunk[chunkWidth,chunkHeight];
			visibleChunks = new List<Chunk>();
		}
		public override void RenderUpdate()
		{
			RectFloat rect = Main.camera.rect;
			int x1 = Mathf.Clamp((int)Math.Floor(rect.x/Chunk.ChunkSize),0,ChunkWidth-1);
			int y1 = Mathf.Clamp((int)Math.Floor(rect.y/Chunk.ChunkSize),0,ChunkHeight-1);
			int x2 = Mathf.Clamp((int)Math.Ceiling((rect.x+rect.width)/Chunk.ChunkSize),0,ChunkWidth-1);
			int y2 = Mathf.Clamp((int)Math.Ceiling((rect.y+rect.height)/Chunk.ChunkSize),0,ChunkHeight-1);
			
			bool resetFrames = Input.GetKeyDown(Keys.L);
			var noLongerVisibleChunks = new List<Chunk>();
			noLongerVisibleChunks.AddRange(visibleChunks);
			var newVisibleChunks = new List<Chunk>();

			for(int y = y1;y<=y2;y++) {
				for(int x = x1;x<=x2;x++) {
					var chunk = chunks[x,y] ?? (chunks[x,y] = new Chunk(this,new Vector2Int(x,y)));

					if(resetFrames) {
						int xEnd = (x+1)*Chunk.ChunkSize;
						int yEnd = (y+1)*Chunk.ChunkSize;
						for(int yy = y*Chunk.ChunkSize;yy<yEnd;yy++) {
							for(int xx = x*Chunk.ChunkSize;xx<xEnd;xx++) {
								TileFrame(xx,yy);
							}
						}
					}

					chunk.UpdateIsVisible(true);
					newVisibleChunks.Add(chunk);
					noLongerVisibleChunks.Remove(chunk);
				}
			}

			visibleChunks = newVisibleChunks;
			for(int i = 0;i<noLongerVisibleChunks.Count;i++) {
				var chunk = noLongerVisibleChunks[i];
				chunk.UpdateIsVisible(false);
			}
			noLongerVisibleChunks.Clear();
		}

		public abstract void ModifyGenTasks(List<GenPass> list);

		public virtual void Generate(int seed)
		{
			Debug.Log("Resetting Tile Array");

			for(int y = 0;y<height;y++) {
				for(int x = 0;x<width;x++) {
					this[x,y] = default;
				}
			}

			var list = new List<GenPass>();

			ModifyGenTasks(list);

			for(int i = 0;i<list.Count;i++) {
				var task = list[i];

				Debug.Log($"Executing generation task {task.GetType().Name}...");

				task.Run(seed,i,this);

				Debug.Log("Done...");
			}
		}
		
		public static T Create<T>(int width,int height,int? seed = null) where T : World
		{
			var world = Instantiate<T>(init:false);
			world.width = width;
			world.height = height;
			world.Init();
			world.Generate(seed ?? (int)DateTime.Now.Ticks);
			return world;
		}
	}
}
