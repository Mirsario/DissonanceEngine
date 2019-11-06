using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using GameEngine;
using GameEngine.Graphics;
using GameEngine.Physics;

namespace AbyssCrusaders.Core
{
	public partial class Chunk
	{
		private const int OcclusionSpread = 4;
		private const int EmissionSpread = 12;
		private const int MaxLightingSpread = EmissionSpread;
		private const int LightingDataTexSize = ChunkSize+MaxLightingSpread*2;

		public bool wasVisible;
		public bool updateTexture;
		public RenderTexture tileTexture;
		public RenderTexture wallTexture;
		public RenderTexture lightingDataTexture;
		public SpriteObject tileSprite;
		public SpriteObject wallSprite;
		public SpriteObject lightingSprite;

		public void InitRendering()
		{
			updateTexture = true;
		}
		public void UpdateIsVisible(bool isVisible)
		{
			if(updateTexture || (isVisible!=wasVisible)) {
				UpdateSprites(isVisible);
			}

			wasVisible = isVisible;
		}

		private void UpdateSprites(bool isVisible)
		{
			void UpdateSprite(bool hasLayer,ref SpriteObject obj,ref RenderTexture texture,float depth,Func<RenderTexture> renderAction,string layer = "Terrain",string shader = "Game/Sprite",Action<Material> onMaterialInit = null)
			{
				if(!hasLayer) {
					if(texture!=null) {
						texture.Dispose();
						texture = null;
					}

					if(obj!=null) {
						obj.Dispose();
						obj = null;
					}

					return;
				}

				texture = renderAction();

				if(obj!=null) {
					return;
				}

				obj = GameObject2D.Instantiate<SpriteObject>(position:new Vector2((position.x)*ChunkSize,(position.y)*ChunkSize),depth:depth);
				obj.layer = Layers.GetLayerIndex(layer);
				obj.sprite.FrameSizeInUnits = new Vector2(ChunkSize,ChunkSize);
				obj.sprite.Origin = Vector2.Zero;

				var material = new Material("Level",Resources.Find<Shader>(shader));
				material.SetTexture("mainTex",texture);

				onMaterialInit?.Invoke(material);

				obj.sprite.Material = material;
			}

			GLDraw.DrawDelayed(() => {
				bool hasTiles = false;
				bool hasWalls = false;

				var tileDrawLists = new Dictionary<ushort,List<Vector2Int>>();
				var tileCrackDrawList = new List<Vector2Int>();
				var wallDrawLists = new Dictionary<ushort,List<Vector2Int>>();

				if(isVisible) {
					//Loop through tiles and write down what we need to render.
					for(int y = -1;y<=ChunkSize;y++) {
						for(int x = -1;x<=ChunkSize;x++) {
							bool corner = x==-1 || y==-1 || x==ChunkSize || y==ChunkSize;

							ref Tile tile = ref (corner ? ref world[positionInTiles.x+x,positionInTiles.y+y] : ref tiles[x,y]);

							if((corner || tile.type==0) && tile.wall==0) {
								continue;
							}

							//Foreground
							if(!corner && tile.type!=0) {
								if(!tileDrawLists.TryGetValue(tile.type,out var list)) {
									tileDrawLists[tile.type] = list = new List<Vector2Int>();
								}
								list.Add(new Vector2Int(x,y));

								if(tile.tileDamage>0) {
									tileCrackDrawList.Add(new Vector2Int(x,y));
								}

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
				}

				//Foreground
				UpdateSprite(isVisible && hasTiles,ref tileSprite,ref tileTexture,10f,() => RenderForeground(tileDrawLists,tileCrackDrawList));

				//Background
				UpdateSprite(isVisible && hasWalls,ref wallSprite,ref wallTexture,-10f,() => RenderBackground(wallDrawLists));

				//Occlusion & Emission
				UpdateSprite(isVisible,ref lightingSprite,ref lightingDataTexture,10f,RenderLightingData,"TerrainLighting","Game/TerrainLighting",m => m.SetFloat("resolutionScale",ChunkSize/(float)LightingDataTexSize));
			});

			updateTexture = false;
		}

		private RenderTexture RenderForeground(Dictionary<ushort,List<Vector2Int>> drawLists,List<Vector2Int> tileCrackDrawList)
		{
			ReadyLayerTexture("Tiles",ref tileTexture,ChunkSizeInPixels,ChunkSizeInPixels);

			static void DrawOnTile(int x,int y,RectFloat srcRect)
			{
				const float TileSizeFactor = 1f/ChunkSize*2f;

				Vector4 dest = new Vector4(
					x*TileSizeFactor-1f,
					y*TileSizeFactor-1f,
					0f,
					0f
				);
				dest.z = dest.x+TileSizeFactor;
				dest.w = dest.y+TileSizeFactor;

				GLDraw.TexCoord2(srcRect.x,srcRect.Bottom);		GLDraw.Vertex2(dest.x,dest.w);
				GLDraw.TexCoord2(srcRect.x,srcRect.y);			GLDraw.Vertex2(dest.x,dest.y);
				GLDraw.TexCoord2(srcRect.Right,srcRect.y);		GLDraw.Vertex2(dest.z,dest.y);
				GLDraw.TexCoord2(srcRect.Right,srcRect.Bottom);	GLDraw.Vertex2(dest.z,dest.w);
			}

			DrawLayer(tileTexture,ChunkSizeInPixels,ChunkSizeInPixels,Resources.Find<Shader>("BasicTexture"),true,true,() => {
				//Draw Tiles
				foreach(var pair in drawLists) {
					ushort type = pair.Key;
					var list = pair.Value;

					var tilePreset = TilePreset.byId[type];
					var framesets = tilePreset?.frameset?.tileFramesets;
					if(framesets==null || !tilePreset.TryGetTexture(out var tex)) {
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

						if(tile.style>=framesets.Length) {
							tile.style = (byte)(framesets.Length-1);
						}

						var frame = framesets[tile.style][tile.tileFrame]*tilePreset.frameset.tileTextureFrameSize;

						RectFloat src = new RectFloat(
							frame.x*textureSizeDiv.x,
							frame.y*textureSizeDiv.y,
							tilePreset.frameset.tileTextureSize*textureSizeDiv.x,
							tilePreset.frameset.tileTextureSize*textureSizeDiv.y
						);

						DrawOnTile(x,y,src);
					}

					GLDraw.End();
				}

				//Draw tile cracks
				if(tileCrackDrawList.Count>0) {
					GLDraw.SetTextures(new Dictionary<string,Texture> {
						{ "mainTex",Resources.Get<Texture>("TileCracks.png") },
					});

					GLDraw.Begin(PrimitiveType.Quads);

					for(int i = 0;i<tileCrackDrawList.Count;i++) {
						Vector2Int tilePos = tileCrackDrawList[i];
						int x = tilePos.x;
						int y = tilePos.y;

						ref Tile tile = ref tiles[x,y];

						int frame = Mathf.FloorToInt(tile.tileDamage/(byte.MaxValue/4f));

						RectFloat src = new RectFloat(
							0.00f,
							0.25f*frame,
							1.00f,
							0.25f
						);

						DrawOnTile(x,y,src);
					}

					GLDraw.End();
				}
			});

			return tileTexture;
		}
		private RenderTexture RenderBackground(Dictionary<ushort,List<Vector2Int>> drawLists)
		{
			ReadyLayerTexture("Walls",ref wallTexture,ChunkSizeInPixels,ChunkSizeInPixels);

			DrawLayer(wallTexture,ChunkSizeInPixels,ChunkSizeInPixels,Resources.Find<Shader>("BasicTexture"),true,true,() => {
				foreach(ushort type in drawLists.Keys.OrderByDescending(key => key)) {
					var list = drawLists[type];

					var tilePreset = TilePreset.byId[type];
					var framesets = tilePreset?.frameset?.wallFrameset;

					if(framesets==null || !tilePreset.TryGetTexture(out var tex)) {
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

						ref Tile tile = ref ((x==-1 || y==-1 || x==ChunkSize || y==ChunkSize) ? ref world[positionInTiles.x+x,positionInTiles.y+y] : ref tiles[x,y]);

						var drawSteps = tilePreset.frameset.wallFrameset[tile.wallFrame];
						//var drawSteps = tilePreset.frameset.GetWallDrawInfo(x,y,tile.wallFrame);

						for(int j = 0;j<drawSteps.Length;j++) {
							var step = drawSteps[j];

							RectFloat src = new RectFloat(
								step.srcX*tilePreset.frameset.tileTextureFrameSize*textureSizeDiv.x,
								step.srcY*tilePreset.frameset.tileTextureFrameSize*textureSizeDiv.y,
								step.srcWidth*tilePreset.frameset.tileTextureSize*textureSizeDiv.x,
								step.srcHeight*tilePreset.frameset.tileTextureSize*textureSizeDiv.y
							);

							const float TileSizeFactor = 1f/ChunkSize*2f;
							RectFloat dest = new RectFloat(
								(x+step.destOffsetX)*TileSizeFactor-1f,
								(y+step.destOffsetY)*TileSizeFactor-1f,
								step.srcWidth*TileSizeFactor,
								step.srcHeight*TileSizeFactor
							);

							GLDraw.TexCoord2(src.x,src.Bottom);
							GLDraw.Vertex2(dest.x,dest.Bottom);
							GLDraw.TexCoord2(src.x,src.y);
							GLDraw.Vertex2(dest.x,dest.y);
							GLDraw.TexCoord2(src.Right,src.y);
							GLDraw.Vertex2(dest.Right,dest.y);
							GLDraw.TexCoord2(src.Right,src.Bottom);
							GLDraw.Vertex2(dest.Right,dest.Bottom);
						}
					}

					GLDraw.End();
				}
			});

			return wallTexture;
		}
		private RenderTexture RenderLightingData()
		{
			ReadyLayerTexture("LightingData",ref lightingDataTexture,LightingDataTexSize,LightingDataTexSize,FilterMode.Bilinear,TextureFormat.RG8);

			//Fill texture wtih 0/1 bits.
			var pixelData = new byte[LightingDataTexSize*LightingDataTexSize*2];
			int i = 0;

			for(int y = -MaxLightingSpread;y<ChunkSize+MaxLightingSpread;y++) {
				for(int x = -MaxLightingSpread;x<ChunkSize+MaxLightingSpread;x++) {
					Tile tile = (x<0 || y<0 || x>=ChunkSize || y>=ChunkSize) ? world[positionInTiles.x+x,positionInTiles.y+y] : tiles[x,y];

					bool passesLight = tile.type==0 || TilePreset.byId[tile.type].transparent;
					bool hasLight = passesLight && tile.wall==0;

					pixelData[i++] = passesLight ? (byte)255 : (byte)0;
					pixelData[i++] = hasLight ? (byte)255 : (byte)0;
				}
			}

			lightingDataTexture.SetPixels(pixelData,TextureFormat.RG8);

			//Process the bits, spreading them into the final texture.
			var blurShader = Resources.Find<Shader>("Game/TerrainEmissionBlur");

			DrawLayer(lightingDataTexture,LightingDataTexSize,LightingDataTexSize,blurShader,false,false,() => {
				blurShader.SetFloat("resolution",LightingDataTexSize);
				blurShader.SetFloat("emissionSpread",EmissionSpread);
				blurShader.SetFloat("occlusionSpread",OcclusionSpread);

				GLDraw.SetTextures(new Dictionary<string,Texture> {
					{ "inputMap",lightingDataTexture },
				});

				GLUtils.DrawQuadUVAttrib();
			});

			return lightingDataTexture;
		}

		private void ReadyLayerTexture(string layerName,ref RenderTexture texture,int xSize,int ySize,FilterMode filterMode = FilterMode.Point,TextureFormat textureFormat = TextureFormat.RGBA8)
		{
			if(texture==null) {
				texture = new RenderTexture($"Chunk_{position.x}_{position.y}_{layerName}",xSize,ySize,filterMode,TextureWrapMode.Clamp,false,textureFormat);
			}
		}

		private static void DrawLayer(RenderTexture texture,int xSize,int ySize,Shader shader,bool clear,bool blending,Action action)
		{
			using var framebuffer = Framebuffer.Create("TempFramebuffer");

			framebuffer.AttachRenderTexture(texture);

			GLDraw.SetRenderTarget(framebuffer);
			GLDraw.SetShader(shader);

			GLDraw.Viewport(0,0,xSize,ySize);

			if(clear) {
				GLDraw.ClearColor(default);
				GLDraw.Clear(ClearMask.ColorBufferBit);
			}

			if(blending) {
				GLDraw.Enable(GraphicsFeature.Blend);
				GLDraw.BlendFunc(BlendingFactor.SrcAlpha,BlendingFactor.OneMinusSrcAlpha);
			}

			action();

			if(blending) {
				GLDraw.Disable(GraphicsFeature.Blend);
			}

			GLDraw.SetShader(null);
			GLDraw.SetRenderTarget(null);
		}
	}
}
