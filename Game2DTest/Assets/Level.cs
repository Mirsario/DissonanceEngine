using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameEngine;
using Game2DTest.Tiles;

namespace Game2DTest
{
	public class Level : GameObject
	{
		private Tile[,] tiles;
		public int width;
		public int height;
		public bool updateTexture;
		public Sprite sprite;
		
		public override void OnInit()
		{
			tiles = new Tile[width,height];
			updateTexture = true;

			sprite = AddComponent<Sprite>();
			sprite.Material = new Material("Level",Resources.Find<Shader>("Transparent/Cutout/Diffuse"));

			Transform.LocalScale = new Vector3(width*TileType.tileSize,height*TileType.tileSize,1f);
		}
		public override void RenderUpdate()
		{
			if(updateTexture) {
				GenerateTexture();
			}
		}

		protected void GenerateTexture()
		{
			int pixelWidth = width*TileType.tileTextureSize;
			int pixelHeight = height*TileType.tileTextureSize;
			Pixel[,] pixels = new Pixel[pixelWidth,pixelHeight];
			Dictionary<(ushort,byte),Pixel[,]> tilePixelCache = new Dictionary<(ushort,byte),Pixel[,]>();
			for(int y=0;y<height;y++) {
				for(int x=0;x<width;x++) {
					Tile tile = tiles[x,y];
					if(tile==null) {
						continue;
					}
					var tileType = tile.Type;
					byte variant = tileType.GetVariant(x,y);
					Pixel[,] tilePixels;
					if(tilePixelCache.TryFirst(t => t.Key.Item1==tile.typeId && t.Key.Item2==variant,out var pair)) {
						tilePixels = pair.Value;
					}else{
						tilePixelCache.Add((tile.typeId,variant),tilePixels = tileType.variantTextures[variant].GetPixels());
					}
					tilePixels.CopyPixels(null,pixels,new Vector2Int(y*TileType.tileTextureSize,x*TileType.tileTextureSize));
				}
			}
			var texture = new Texture(pixelWidth,pixelHeight,wrapMode:TextureWrapMode.Clamp);
			texture.SetPixels(pixels);
			texture.Save("levelBatch.png");

			sprite.Material.SetTexture("mainTex",texture);

			updateTexture = false;
		}
		
		public static Level Create(int width,int height)
		{
			var level = Instantiate<Level>(init:false);
			level.width = width;
			level.height = height;
			level.Init();
			return level;
		}
		public static Level Create(int[,] tileTypes)
		{
			var level = Instantiate<Level>(init:false);
			level.width = tileTypes.GetLength(0);
			level.height = tileTypes.GetLength(1);
			level.Init();
			for(int y=0;y<level.height;y++) {
				for(int x=0;x<level.width;x++) {
					int id = tileTypes[x,y];
					if(id>=0) {
						level.tiles[x,y] = new Tile((ushort)id);
					}
				}
			}
			return level;
		}
	}
}
