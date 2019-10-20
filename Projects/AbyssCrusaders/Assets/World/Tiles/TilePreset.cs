using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GameEngine;
using GameEngine.Graphics;

namespace AbyssCrusaders
{
	public abstract class TilePreset : IDisposable //ICloneable, 
	{
		//Static/Consts
		public static ushort typeCount;
		public static Dictionary<Type,TilePreset> byType;
		public static Dictionary<string,TilePreset> byName;
		public static TilePreset[] byId;
		
		//Instance
		public readonly string Name;
		public readonly TileFrameset frameset;
		public bool canBeWall;
		public bool allowDroppingThrough;
		public CollisionInfo collision;

		public ushort Id { get; private set; }

		protected abstract TileFrameset Frameset { get; }
		public virtual Texture Texture => Resources.Get<Texture>($"{GetType().Name}.png");

		protected TilePreset()
		{
			Name = GetType().Name;
			collision = CollisionInfo.None;
			frameset = Frameset;

			OnInit();
		}

		public virtual void OnInit() {}
		public virtual void Dispose() {}
		public virtual bool BlendsWithTile(Tile thisTile,Tile otherTile) => false;

		public bool TryGetTexture(out Texture texture) => (texture = Texture)!=null;

		public static void Initialize()
		{
			Assembly assembly = Assembly.GetCallingAssembly();
			Type thisType = typeof(TilePreset);
			Type[] tileTypes = assembly.GetTypes().Where(t => t!=thisType && !t.IsAbstract && thisType.IsAssignableFrom(t)).ToArray();
			typeCount = (ushort)tileTypes.Length;

			//ID Claiming, only used by Tiles.Air currently.
			for(ushort i=0;i<tileTypes.Length;i++) {
				Type type = tileTypes[i];
				var forceIdAttribute = type.GetCustomAttribute<ForceIDAttribute>();
				if(forceIdAttribute==null) {
					continue;
				}
				if(forceIdAttribute.id>=typeCount) {
					throw new Exception($"ID '{forceIdAttribute.id}' less than the amount of Tile types defined.");
				}
				Type otherType = tileTypes[forceIdAttribute.id];
				tileTypes[i] = otherType;
				tileTypes[forceIdAttribute.id] = type;
				//Write down that we swapped IDs, so that we can throw an exception if multiple types fight for the same ID?
			}

			byId = new TilePreset[typeCount];
			byName = new Dictionary<string,TilePreset>();
			byType = new Dictionary<Type,TilePreset>();
			for(ushort i = 0;i<tileTypes.Length;i++) {
				var type = tileTypes[i];
				var instance = (TilePreset)Activator.CreateInstance(type);
				instance.Id = i;
				byType[type] = instance;
				byName[instance.Name] = instance;
				byId[i] = instance;
			}
			//GenerateTexture();
		}
		
		//public object Clone() => MemberwiseClone();
		/*public static void GenerateTexture() 
		{
			int tileLength = 2;
			int totalVariants = byId.Sum(t => t.numVariants);
			while(tileLength*tileLength<=totalVariants) {
				tileLength *= tileLength;
			}
			int sizePerTile = 32;
			int fullSize = tileLength*sizePerTile;
			int x = 0;
			int y = 0;
			Pixel[,] pixels = new Pixel[fullSize,fullSize];
			for(var i=0;i<typeCount;i++) {
				var tile = byId[i];
				for(int j=0;j<tile.numVariants;j++) {
					//tile.variantUVs[j] = new Vector4((x/(float)tileLength),1f-((y+1)/(float)tileLength),((x+1)/(float)tileLength),1f-(y/(float)tileLength));
					tile.variantUVs[j] = new Vector4(y,x,y+1,x+1)/tileLength;
					tile.variantTextures[j].GetPixels().CopyPixels(null,pixels,new Vector2Int(x*sizePerTile,y*sizePerTile));
					x++;
					if(x>=tileLength) {
						y++;
						x = 0;
					}
				}
			}
			tileAtlas = new Texture(fullSize,fullSize,wrapMode:TextureWrapMode.Clamp);
			tileAtlas.SetPixels(pixels);
			tileAtlas.Save("tileBatch.png");
		}*/

		public static T GetType<T>() where T : TilePreset => (T)byType[typeof(T)];
		public static ushort GetTypeId<T>() where T : TilePreset => byType[typeof(T)].Id;
	}
}
