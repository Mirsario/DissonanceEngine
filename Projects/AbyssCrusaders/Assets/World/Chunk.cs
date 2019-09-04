using System;
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

		private static (Vector2Int pos,float distance)[] lightChecks;
		
		public Vector2Int position;
		public Vector2Int positionInTiles;
		public World world;
		public Tile[,] tiles;
		public bool updateTexture;
		public bool wasVisible;

		public RenderTexture tileTexture;
		public RenderTexture wallTexture;
		public RenderTexture lightTexture;
		public SpriteObject tileSprite;
		public SpriteObject wallSprite;
		
		public Chunk(World world,Vector2Int pos)
		{
			position = pos;
			positionInTiles = position*ChunkSize;
			this.world = world;

			tiles = new Tile[ChunkSize,ChunkSize];
			updateTexture = true;

			if(lightChecks==null) {
				const int MaxDistance = 3;
				const float MaxDistanceDiv = 1f/MaxDistance;

				var finalChecks = new List<(Vector2Int,float)>();
				
				var pointsToCheck = new List<(Vector2Int point,int distance)> {
					(default,0)
				};
				var points = new HashSet<Vector2Int> {
					default
				};

				Vector2 vecCenter = default;
				int i = 0;

				while(i<pointsToCheck.Count) {
					(var point,int distance) = pointsToCheck[i];

					finalChecks.Add((point,1f-Math.Min(1f,Vector2.Distance(vecCenter,new Vector2(point.x+0.5f,point.y+0.5f))*MaxDistanceDiv)));

					if(distance<MaxDistance) {
						void TryAdd(int xx,int yy)
						{
							var ivec = new Vector2Int(xx,yy);
							if(points.Add(ivec)) {
								pointsToCheck.Add((ivec,distance+1));
							}
						}

						TryAdd(point.x-1,point.y);
						TryAdd(point.x,	 point.y-1);
						TryAdd(point.x+1,point.y);
						TryAdd(point.x,	 point.y+1);
					}

					i++;
				}

				lightChecks = finalChecks.ToArray();
			}
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

			if(isVisible) {
				int chunkTileX = position.x*ChunkSize;
				int chunkTileY = position.y*ChunkSize;

				var tileDrawLists = new Dictionary<ushort,List<Vector2Int>>();
				var wallDrawLists = new Dictionary<ushort,List<Vector2Int>>();

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
				
				static bool TryGetTileTexture(TilePreset preset,ushort type,out Texture texture) => (texture = preset.Texture)!=null;

				void ReadyLayerTexture(string layerName,ref RenderTexture texture,int xSize,int ySize,FilterMode filterMode = FilterMode.Point)
				{
					if(texture==null) {
						texture = new RenderTexture($"Chunk_{position.x}_{position.y}_{layerName}",xSize,ySize,filterMode,TextureWrapMode.Clamp);
					}
				}

				static void Draw(string layerName,ref RenderTexture texture,int xSize,int ySize,Shader shader,Action action)
				{
					using var framebuffer = Framebuffer.Create("TempFramebuffer");

					framebuffer.AttachRenderTexture(texture);
					GLDraw.SetRenderTarget(framebuffer);
					GLDraw.SetShader(shader);

					GLDraw.ClearColor(default);
					GLDraw.Clear(ClearMask.ColorBufferBit);
					GLDraw.Viewport(0,0,xSize,ySize);
					GLDraw.Enable(GraphicsFeature.Blend);
					GLDraw.BlendFunc(BlendingFactor.SrcAlpha,BlendingFactor.OneMinusSrcAlpha);

					action();

					GLDraw.SetShader(null);
					GLDraw.SetRenderTarget(null);
					GLDraw.Disable(GraphicsFeature.Blend);
				}

				#region Render Foreground
				if(hasTiles) {
					GLDraw.DrawDelayed(() => {
						ReadyLayerTexture("Tiles",ref tileTexture,SizeInPixels,SizeInPixels);
						Draw("Tiles",ref tileTexture,SizeInPixels,SizeInPixels,Resources.Find<Shader>("BasicTexture"),() => {
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
									Vector2Int tilePos = list[i];
									int x = tilePos.x;
									int y = tilePos.y;

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
						});
					});
				}
				#endregion

				#region Render Background
				if(hasWalls) {
					GLDraw.DrawDelayed(() => {
						ReadyLayerTexture("Walls",ref wallTexture,SizeInPixels,SizeInPixels);
						Draw("Walls",ref wallTexture,SizeInPixels,SizeInPixels,Resources.Find<Shader>("BasicTexture"),() => {
							foreach(ushort type in wallDrawLists.Keys.OrderByDescending(key => key)) {
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
									var tilePos = list[i];
									int x = tilePos.x;
									int y = tilePos.y;
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
									}
								}

								GLDraw.End();
							}
						});
					});
				}
				#endregion

				#region Render Lighting
				GLDraw.DrawDelayed(() => {
					const int TexSize = ChunkSize+2;

					var shader = Resources.Find<Shader>("BasicVertexColor");
					
					ReadyLayerTexture("Light",ref lightTexture,TexSize,TexSize,FilterMode.Bilinear);

					Draw("Light",ref lightTexture,TexSize,TexSize,shader,() => {
						const float PixelSize = 1f/TexSize;

						GLDraw.Begin(PrimitiveType.Points);

						for(int y = -1;y<=ChunkSize;y++) {
							for(int x = -1;x<=ChunkSize;x++) {
								Tile tile = (x<0 || y<0 || x>=ChunkSize || y>=ChunkSize) ? world[positionInTiles.x+x,positionInTiles.y+y] : tiles[x,y];

								float lightLevel;
								if(tile.type==0) {
									lightLevel = 1f;
								}else{
									lightLevel = 0f;

									Vector2Int basePoint = LocalPointToWorld(x,y);

									for(int i = 0;i<lightChecks.Length;i++) {
										(var relativePos,float pointLightLevel) = lightChecks[i];

										int xx = x+relativePos.x;
										int yy = y+relativePos.y;

										var thisTile = (xx<0 || yy<0 || xx>=ChunkSize || yy>=ChunkSize) ? world[positionInTiles.x+xx,positionInTiles.y+yy] : tiles[xx,yy];

										if(thisTile.type==0) {
											lightLevel = pointLightLevel;
											break;
										}
									}
								}

								//GLDraw.Uniform4("color",new Vector4(lightLevel,0f,0f,0f));

								GLDraw.VertexAttrib4(AttributeId.Color,new Vector4(lightLevel,lightLevel,lightLevel,1f));
								GLDraw.Vertex2(-1f+x*PixelSize*2f,-1f+y*PixelSize*2f);
							}
						}

						GLDraw.End();
					});
				});
				#endregion
			}

			void UpdateSprite(bool hasLayer,ref SpriteObject obj,ref RenderTexture texture,float depth,Texture emissionMap = null)
			{
				if(!hasLayer) {
					if(obj!=null) {
						obj.sprite.Material.Dispose();
						obj.Dispose();
						obj = null;
					}

					if(texture!=null) {
						texture.Dispose();
						texture = null;
					}
				} else {
					if(obj!=null) {
						return;
					}

					obj = GameObject2D.Instantiate<SpriteObject>(position:new Vector2((position.x+0.5f)*ChunkSize,(position.y+0.5f)*ChunkSize),depth:depth,scale:new Vector2(ChunkSize,ChunkSize));

					Material mat = new Material("Level",Resources.Find<Shader>(emissionMap!=null ? "Game/SpriteNegativeEmissive" : "Game/Sprite"));
					mat.SetTexture("mainTex",texture);
					if(emissionMap!=null) {
						mat.SetTexture("emissionMap",emissionMap);
					}
					obj.sprite.Material = mat;
				}
			}

			GLDraw.DrawDelayed(() => {
				UpdateSprite(hasTiles,ref tileSprite,ref tileTexture,10f,lightTexture);
				UpdateSprite(hasWalls,ref wallSprite,ref wallTexture,-10f);
			});

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

		public Vector2Int LocalPointToWorld(int x,int y) => new Vector2Int(position.x*ChunkSize+x,position.y*ChunkSize+y);
	}
}
