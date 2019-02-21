using System.IO;
using System.Linq;
using System.Collections.Generic;
using GameEngine;
using GameEngine.Graphics;

namespace AbyssCrusaders
{
	public class Chunk : ISaveable
	{
		public const int ChunkSize = 64;
		
		public int x;
		public int y;
		public World world;
		public Tile[,] tiles;
		public bool updateTexture;
		public bool wasVisible;

		public RenderTexture tileTexture;
		public RenderTexture wallTexture;
		public SpriteObject tileSprite;
		public SpriteObject wallSprite;
		
		public Chunk(World world,int x,int y)
		{
			this.x = x;
			this.y = y;
			this.world = world;

			tiles = new Tile[ChunkSize,ChunkSize];
			updateTexture = true;
		}
		
		public void UpdateIsVisible(bool isVisible)
		{
			if(updateTexture || (isVisible!=wasVisible)) {
				UpdateSprites(isVisible);
			}
			wasVisible = isVisible;
		}

		protected void UpdateSprites(bool isVisible)
		{
			//To be rewritten.
			//Currently copies pixels on CPU instead of doing drawing to a Render Target

			bool hasTiles = false;
			bool hasWalls = false;

			const int SizeInPixels = ChunkSize*Main.UnitSizeInPixels;
			const float SizeInPixelsDiv = 1f/SizeInPixels;

			if(isVisible) {
				int chunkTileX = x*ChunkSize;
				int chunkTileY = y*ChunkSize;

				var tileDrawLists = new Dictionary<ushort,List<Vector2Int>>();
				var wallDrawLists = new Dictionary<ushort,List<Vector2Int>>();
				
				void ReadyLayerTexture(string layerName,ref RenderTexture texture)
				{
					if(texture==null) {
						texture = new RenderTexture($"Chunk_{x}_{y} {layerName}",SizeInPixels,SizeInPixels,FilterMode.Point,TextureWrapMode.Clamp,true);
					}
				}
				bool TryGetTileTexture(TilePreset preset,ushort type,out Texture texture)
				{
					texture = preset.Texture;
					return texture!=null;
				}

				//Loop through tiles and write down what we need to render.
				for(int y=-1;y<=ChunkSize;y++) {
					for(int x=-1;x<=ChunkSize;x++) {
						bool corner = x==-1 || y==-1 || x==ChunkSize || y==ChunkSize;

						ref Tile tile = ref (corner ? ref world[chunkTileX+x,chunkTileY+y] : ref tiles[x,y]);

						if((corner || tile.type==0) && tile.wall==0) {
							continue;
						}

						//Foreground
						if(!corner && tile.type!=0) {
							if(!tileDrawLists.TryGetValue(tile.type,out var list)) {
								tileDrawLists[tile.type] = list = new List<Vector2Int>();
							}
							list.Add(new Vector2Int(x,y));
							hasTiles = true;
						}

						//Background
						if(tile.wall!=0) {
							if(!wallDrawLists.TryGetValue(tile.wall,out var list)) {
								wallDrawLists[tile.wall] = list = new List<Vector2Int>();
							}
							list.Add(new Vector2Int(x,y));
							hasWalls = true;
						}
					}
				}

				//Render Foreground
				if(hasTiles) {
					GLDraw.DrawDelayed(() => {
						ReadyLayerTexture("Tiles",ref tileTexture);

						using(var framebuffer = new Framebuffer("TempFramebuffer")) {
							framebuffer.AttachRenderTexture(tileTexture);
							GLDraw.SetRenderTarget(framebuffer);

							GLDraw.SetShader(Resources.Find<Shader>("BasicTexture"));

							GLDraw.ClearColor(default);
							GLDraw.Clear(ClearMask.ColorBufferBit);
							GLDraw.Viewport(0,0,SizeInPixels,SizeInPixels);
							
							foreach(var pair in tileDrawLists) {
								ushort type = pair.Key;
								var list = pair.Value;
								
								var tilePreset = TilePreset.byId[type];
								var framesets = tilePreset?.frameset?.tileFramesets;
								if(framesets==null || !TryGetTileTexture(tilePreset,type,out var tex)) {
									continue;
								}
								
								GLDraw.SetTextures(new Dictionary<string,Texture> {
									{ "mainTex",tex },
								});
								Vector2 textureSizeDiv = Vector2.One/(Vector2)tex.Size;

								GLDraw.Begin(PrimitiveType.Quads);

								for(int i = 0;i<list.Count;i++) {
									Vector2Int pos = list[i];
									int x = pos.x;
									int y = pos.y;

									ref Tile tile = ref tiles[x,y];

									var frame = tilePreset.frameset.tileFramesets[tile.style][tile.tileFrame]*tilePreset.frameset.tileTextureFrameSize;
									RectFloat src = new RectFloat(
										frame.x*textureSizeDiv.x,
										frame.y*textureSizeDiv.y,
										tilePreset.frameset.tileTextureSize*textureSizeDiv.x,
										tilePreset.frameset.tileTextureSize*textureSizeDiv.y
									);
									const float TileSizeFactor = 1f/ChunkSize*2f;
									RectFloat dest = new RectFloat(
										x*TileSizeFactor-1f,
										y*TileSizeFactor-1f,
										TileSizeFactor,
										TileSizeFactor
									);

									GLDraw.TexCoord2(src.x,		src.Bottom);	GLDraw.Vertex2(dest.x,		dest.Bottom);
									GLDraw.TexCoord2(src.x,		src.y);			GLDraw.Vertex2(dest.x,		dest.y);
									GLDraw.TexCoord2(src.Right,	src.y);			GLDraw.Vertex2(dest.Right,	dest.y);
									GLDraw.TexCoord2(src.Right,	src.Bottom);	GLDraw.Vertex2(dest.Right,	dest.Bottom);
								}

								GLDraw.End();
							}

							GLDraw.SetShader(null);
							GLDraw.SetRenderTarget(null);
						}
					});
				}

				//Render Background
				if(hasWalls) {
					GLDraw.DrawDelayed(() => {
						ReadyLayerTexture("Walls",ref wallTexture);

						using(var framebuffer = new Framebuffer("TempFramebuffer")) {
							framebuffer.AttachRenderTexture(wallTexture);
							GLDraw.SetRenderTarget(framebuffer);

							GLDraw.SetShader(Resources.Find<Shader>("BasicTexture"));

							GLDraw.ClearColor(default);
							GLDraw.Clear(ClearMask.ColorBufferBit);
							GLDraw.Viewport(0,0,SizeInPixels,SizeInPixels);
							GLDraw.Enable(GraphicsFeature.Blend);
							GLDraw.BlendFunc(BlendFunc.SrcAlpha,BlendFunc.OneMinusSrcAlpha);
							
							var sortedKeys = wallDrawLists.Keys.OrderByDescending(key => key);
							foreach(ushort type in sortedKeys) {
								var list = wallDrawLists[type];

								var preset = TilePreset.byId[type];
								var framesets = preset?.frameset?.wallFrameset;
								if(framesets==null || !TryGetTileTexture(preset,type,out var tex)) {
									continue;
								}

								GLDraw.SetTextures(new Dictionary<string,Texture> {
									{ "mainTex",tex },
								});
								Vector2 textureSizeDiv = Vector2.One/(Vector2)tex.Size;

								GLDraw.Begin(PrimitiveType.Quads);

								for(int i = 0;i<list.Count;i++) {
									var pos = list[i];
									int x = pos.x;
									int y = pos.y;
									ref Tile tile = ref ((x==-1 || y==-1 || x==ChunkSize || y==ChunkSize) ? ref world[chunkTileX+x,chunkTileY+y] : ref tiles[x,y]);

									var drawSteps = preset.frameset.wallFrameset[tile.wallFrame];
									for(int j = 0;j<drawSteps.Length;j++) {
										var step = drawSteps[j];
										RectFloat src = new RectFloat(
											step.srcX*preset.frameset.tileTextureFrameSize*textureSizeDiv.x,
											step.srcY*preset.frameset.tileTextureFrameSize*textureSizeDiv.y,
											step.srcWidth*preset.frameset.tileTextureSize*textureSizeDiv.x,
											step.srcHeight*preset.frameset.tileTextureSize*textureSizeDiv.y
										);
										const float TileSizeFactor = 1f/ChunkSize*2f;
										RectFloat dest = new RectFloat(
											(x+step.destOffsetX)*TileSizeFactor-1f,
											(y+step.destOffsetY)*TileSizeFactor-1f,
											step.srcWidth*TileSizeFactor,
											step.srcHeight*TileSizeFactor
										);

										GLDraw.TexCoord2(src.x,		src.Bottom);	GLDraw.Vertex2(dest.x,		dest.Bottom);
										GLDraw.TexCoord2(src.x,		src.y);			GLDraw.Vertex2(dest.x,		dest.y);
										GLDraw.TexCoord2(src.Right,	src.y);			GLDraw.Vertex2(dest.Right,	dest.y);
										GLDraw.TexCoord2(src.Right,	src.Bottom);	GLDraw.Vertex2(dest.Right,	dest.Bottom);
										
										/*pixels.CopyPixels(
											source,
											wallPixels,
											destination,
											(pxlA,pxlB) => {
												float lerpTime = pxlB.a/255f;
												return new Pixel(
													(byte)Mathf.Clamp(Mathf.Lerp(pxlA.r,pxlB.r,lerpTime),0,255),
													(byte)Mathf.Clamp(Mathf.Lerp(pxlA.g,pxlB.g,lerpTime),0,255),
													(byte)Mathf.Clamp(Mathf.Lerp(pxlA.b,pxlB.b,lerpTime),0,255),
													(byte)Mathf.Clamp(pxlA.a+pxlB.a,0,255)
												);
											}
										);*/
									}
								}

								GLDraw.End();
							}

							GLDraw.Disable(GraphicsFeature.Blend);
							GLDraw.SetShader(null);
							GLDraw.SetRenderTarget(null);
						}
					});
					

					
				}
			}

			void UpdateSprite(bool hasLayer,ref SpriteObject obj,ref RenderTexture texture,float depth)
			{
				if(hasLayer) {
					if(obj==null) {
						obj = GameObject2D.Instantiate<SpriteObject>(
							position:new Vector2((x+0.5f)*ChunkSize,(y+0.5f)*ChunkSize),
							depth:depth,
							scale:new Vector2(ChunkSize,ChunkSize)
						);
						obj.sprite.Material = new Material("Level",Resources.Find<Shader>("Game/Sprite"));
						obj.sprite.Material.SetTexture("mainTex",texture);
					}
				}else{
					if(obj!=null) {
						obj.sprite.Material.Dispose();
						obj.Dispose();
						obj = null;
					}
					if(texture!=null) {
						texture.Dispose();
						texture = null;
					}
				}
			}

			GLDraw.DrawDelayed(() => {
				UpdateSprite(hasTiles,ref tileSprite,ref tileTexture,10f);
				UpdateSprite(hasWalls,ref wallSprite,ref wallTexture,-10f);
			});

			/*bool visible = tileSprite!=null && wallSprite!=null;
			if(visible!=wasVisible) {
				if(!wasVisible) {
					world.visibleChunks.Add(this);
				}else{
					world.visibleChunks.Remove(this);
				}
				wasVisible = visible;
			}*/

			updateTexture = false;
		}

		public void Save(BinaryWriter writer)
		{
			for(int y = 0;y<ChunkSize;y++) {
				for(int x = 0;x<ChunkSize;x++) {
					tiles[x,y].Save(writer);
				}
			}
		}
		public void Load(BinaryReader reader)
		{
			for(int y = 0;y<ChunkSize;y++) {
				for(int x = 0;x<ChunkSize;x++) {
					tiles[x,y].Load(reader);
				}
			}
		}
	}
}
