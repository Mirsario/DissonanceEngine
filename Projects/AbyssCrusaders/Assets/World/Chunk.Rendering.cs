using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using GameEngine;
using GameEngine.Graphics;
using GameEngine.Physics;

namespace AbyssCrusaders
{
	public partial class Chunk
	{
		private static (Vector2Int pos,float lightFactor)[] occlusionChecks;
		private static (Vector2Int pos,float lightFactor)[] emissionChecks;

		public bool wasVisible;
		public bool updateTexture;
		public RenderTexture tileTexture;
		public RenderTexture wallTexture;
		public RenderTexture emissionTexture;
		public RenderTexture occlusionTexture;
		public SpriteObject tileSprite;
		public SpriteObject wallSprite;
		public SpriteObject lightingSprite;

		public void InitRendering()
		{
			updateTexture = true;

			if(occlusionChecks==null) {
				PrepareLightChecks(ref occlusionChecks,3);
				PrepareLightChecks(ref emissionChecks,10);
			}
		}
		public void UpdateIsVisible(bool isVisible)
		{
			if(updateTexture || (isVisible!=wasVisible)) {
				UpdateSprites(isVisible);
			}

			wasVisible = isVisible;
		}

		private void PrepareLightChecks(ref (Vector2Int pos,float lightFactor)[] array,int maxDistance)
		{
			float maxDistanceDiv = 1f/maxDistance;

			var finalChecks = new List<(Vector2Int,float)>();

			var pointsToCheck = new List<Vector2Int> { default };
			var points = new HashSet<Vector2Int> { default };

			Vector2 vecCenter = default;
			int i = 0;

			while(i<pointsToCheck.Count) {
				var point = pointsToCheck[i];

				float actualDistance = Math.Max(0f,Vector2.Distance(vecCenter,new Vector2(point.x,point.y))-1f);
				float usedDistance = 1f-Math.Min(1f,actualDistance*maxDistanceDiv);

				finalChecks.Add((point,usedDistance*usedDistance));

				if(actualDistance<=maxDistance) {
					void TryAdd(int xx,int yy)
					{
						var ivec = new Vector2Int(xx,yy);
						if(points.Add(ivec)) {
							pointsToCheck.Add(ivec);
						}
					}

					TryAdd(point.x-1,point.y);
					TryAdd(point.x,point.y-1);
					TryAdd(point.x+1,point.y);
					TryAdd(point.x,point.y+1);
				}

				i++;
			}

			array = finalChecks.ToArray();
		}

		private void UpdateSprites(bool isVisible)
		{
			bool hasTiles = false;
			bool hasWalls = false;

			if(isVisible) {
				var tileDrawLists = new Dictionary<ushort,List<Vector2Int>>();
				var wallDrawLists = new Dictionary<ushort,List<Vector2Int>>();

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

				if(hasTiles) {
					//Render Foreground
					GLDraw.DrawDelayed(() => RenderForeground(tileDrawLists));

					//Render Occlusion
					GLDraw.DrawDelayed(RenderOcclusion);
				}

				//Render Background
				if(hasWalls) {
					GLDraw.DrawDelayed(() => RenderBackground(wallDrawLists));
				}

				RenderEmission();
			}

			void UpdateSprite(bool hasLayer,ref SpriteObject obj,ref RenderTexture texture,float depth,string layer = "Terrain",string shader = "Game/Sprite")
			{
				switch(UpdateSpriteExt(hasLayer,ref obj,depth,layer,shader)) {
					case false:
						if(texture!=null) {
							texture.Dispose();
							texture = null;
						}
						break;
					case true:
						obj.sprite.Material.SetTexture("mainTex",texture);
						break;
				}
			}
			bool? UpdateSpriteExt(bool hasLayer,ref SpriteObject obj,float depth,string layer = "Terrain",string shader = "Game/Sprite")
			{
				if(!hasLayer) {
					if(obj!=null) {
						obj.sprite.Material.Dispose();
						obj.Dispose();
						obj = null;
					}

					return false;
				}

				if(obj!=null) {
					return null;
				}

				obj = GameObject2D.Instantiate<SpriteObject>(position: new Vector2((position.x+0.5f)*ChunkSize,(position.y+0.5f)*ChunkSize),depth: depth,scale: new Vector2(ChunkSize,ChunkSize));
				obj.layer = Layers.GetLayerIndex(layer);
				obj.sprite.Material = new Material("Level",Resources.Find<Shader>(shader));

				return true;
			}

			GLDraw.DrawDelayed(() => {
				//Foreground
				UpdateSprite(hasTiles,ref tileSprite,ref tileTexture,10f);

				//Background
				UpdateSprite(hasWalls,ref wallSprite,ref wallTexture,-10f);

				//Occlusion & Emission
				switch(UpdateSpriteExt(hasTiles,ref lightingSprite,10f,"TerrainLighting","Game/TerrainLighting")) {
					case false:
						if(occlusionTexture!=null) {
							occlusionTexture.Dispose();
							occlusionTexture = null;
						}
						if(emissionTexture!=null) {
							emissionTexture.Dispose();
							emissionTexture = null;
						}
						break;
					case true:
						var material = lightingSprite.sprite.Material;
						material.SetTexture("emissionMap",emissionTexture);
						material.SetTexture("occlusionMap",occlusionTexture);
						break;
				}
			});

			updateTexture = false;
		}

		private void RenderForeground(Dictionary<ushort,List<Vector2Int>> drawLists)
		{
			ReadyLayerTexture("Tiles",ref tileTexture,ChunkSizeInPixels,ChunkSizeInPixels);

			DrawLayer(ref tileTexture,ChunkSizeInPixels,ChunkSizeInPixels,Resources.Find<Shader>("BasicTexture"),() => {
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

						const float TileSizeFactor = 1f/ChunkSize*2f;
						RectFloat dest = new RectFloat(
							x*TileSizeFactor-1f,
							y*TileSizeFactor-1f,
							TileSizeFactor,
							TileSizeFactor
						);

						GLDraw.TexCoord2(src.x,src.Bottom);		GLDraw.Vertex2(dest.x,dest.Bottom);
						GLDraw.TexCoord2(src.x,src.y);			GLDraw.Vertex2(dest.x,dest.y);
						GLDraw.TexCoord2(src.Right,src.y);		GLDraw.Vertex2(dest.Right,dest.y);
						GLDraw.TexCoord2(src.Right,src.Bottom);	GLDraw.Vertex2(dest.Right,dest.Bottom);
					}

					GLDraw.End();
				}
			});
		}
		private void RenderBackground(Dictionary<ushort,List<Vector2Int>> drawLists)
		{
			ReadyLayerTexture("Walls",ref wallTexture,ChunkSizeInPixels,ChunkSizeInPixels);

			DrawLayer(ref wallTexture,ChunkSizeInPixels,ChunkSizeInPixels,Resources.Find<Shader>("BasicTexture"),() => {
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
		}
		private void RenderOcclusion()
		{
			const int TexSize = ChunkSize+2;

			var shader = Resources.Find<Shader>("BasicVertexColor");

			ReadyLayerTexture("Occlusion",ref occlusionTexture,TexSize,TexSize,FilterMode.Bilinear);

			DrawLayer(ref occlusionTexture,TexSize,TexSize,shader,() => {
				const float PixelSize = 1f/TexSize;

				GLDraw.Begin(PrimitiveType.Points);

				for(int y = -1;y<=ChunkSize;y++) {
					for(int x = -1;x<=ChunkSize;x++) {
						Tile tile = (x<0 || y<0 || x>=ChunkSize || y>=ChunkSize) ? world[positionInTiles.x+x,positionInTiles.y+y] : tiles[x,y];

						float lightLevel;
						if(tile.type==0) {
							lightLevel = 1f;
						} else {
							lightLevel = 0f;

							Vector2Int basePoint = LocalPointToWorld(x,y);

							for(int i = 0;i<occlusionChecks.Length;i++) {
								(var relativePos,float pointLightLevel) = occlusionChecks[i];

								int xx = x+relativePos.x;
								int yy = y+relativePos.y;

								var thisTile = (xx<0 || yy<0 || xx>=ChunkSize || yy>=ChunkSize) ? world[positionInTiles.x+xx,positionInTiles.y+yy] : tiles[xx,yy];

								if(thisTile.type==0) {
									lightLevel = Math.Max(lightLevel,pointLightLevel);
								}
							}
						}

						GLDraw.VertexAttrib1(AttributeId.Color,1f-lightLevel);
						GLDraw.Vertex2(-1f+(x+1)*PixelSize*2f,-1f+(y+1)*PixelSize*2f);
					}
				}

				GLDraw.End();
			});
		}
		private void RenderEmission()
		{
			const int TexSize = ChunkSize+2;

			var shader = Resources.Find<Shader>("BasicVertexColor");

			ReadyLayerTexture("Emission",ref emissionTexture,TexSize,TexSize,FilterMode.Bilinear);

			DrawLayer(ref emissionTexture,TexSize,TexSize,shader,() => {
				const float PixelSize = 1f/TexSize;

				GLDraw.Begin(PrimitiveType.Points);

				for(int y = -1;y<=ChunkSize;y++) {
					for(int x = -1;x<=ChunkSize;x++) {
						Tile tile = (x<0 || y<0 || x>=ChunkSize || y>=ChunkSize) ? world[positionInTiles.x+x,positionInTiles.y+y] : tiles[x,y];


						float lightLevel;
						if(tile.type==0 && tile.wall==0) {
							lightLevel = 1f;
						} else {
							lightLevel = 0f;

							Vector2Int basePoint = LocalPointToWorld(x,y);

							int stopAt = -1;

							for(int i = 0;i<emissionChecks.Length;i++) {
								if(i==stopAt) {
									break;
								}

								(var relativePos, float pointLightLevel) = emissionChecks[i];

								int xx = x+relativePos.x;
								int yy = y+relativePos.y;

								var thisTile = (xx<0 || yy<0 || xx>=ChunkSize || yy>=ChunkSize) ? world[positionInTiles.x+xx,positionInTiles.y+yy] : tiles[xx,yy];

								if(thisTile.type==0 && thisTile.wall==0) {
									lightLevel = Math.Max(lightLevel,pointLightLevel);
									//stopAt = ((i/16)+1)*16;
								}
							}
						}

						GLDraw.VertexAttrib1(AttributeId.Color,lightLevel);
						GLDraw.Vertex2(-1f+(x+1)*PixelSize*2f,-1f+(y+1)*PixelSize*2f);
					}
				}

				GLDraw.End();
			});
		}

		private void ReadyLayerTexture(string layerName,ref RenderTexture texture,int xSize,int ySize,FilterMode filterMode = FilterMode.Point)
		{
			if(texture==null) {
				texture = new RenderTexture($"Chunk_{position.x}_{position.y}_{layerName}",xSize,ySize,filterMode,TextureWrapMode.Clamp);
			}
		}

		private static void DrawLayer(ref RenderTexture texture,int xSize,int ySize,Shader shader,Action action)
		{
			using var framebuffer = Framebuffer.Create("TempFramebuffer");

			framebuffer.AttachRenderTexture(texture);

			GLDraw.SetRenderTarget(framebuffer);
			GLDraw.SetShader(shader);

			GLDraw.Viewport(0,0,xSize,ySize);

			GLDraw.ClearColor(default);
			GLDraw.Clear(ClearMask.ColorBufferBit);

			GLDraw.Enable(GraphicsFeature.Blend);
			GLDraw.BlendFunc(BlendingFactor.SrcAlpha,BlendingFactor.OneMinusSrcAlpha);

			action();

			GLDraw.Disable(GraphicsFeature.Blend);

			GLDraw.SetShader(null);
			GLDraw.SetRenderTarget(null);
		}
	}
}
