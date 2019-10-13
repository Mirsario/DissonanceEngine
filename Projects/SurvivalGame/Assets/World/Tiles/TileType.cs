using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GameEngine;
using GameEngine.Graphics;
using GameEngine.Extensions;

namespace SurvivalGame
{
	public abstract class TileType : ICloneable, IDisposable, IHasMaterial
	{
		public static class IDs<T> where T : TileType
		{
			public static readonly int Id;
			public static readonly ushort IdUShort;
		}

		//Static
		public static bool initialized;
		public static ushort typeCount;
		public static Dictionary<string,TileType> byName;
		public static TileType[] byId;
		public static Texture tileAtlas;
		
		//Instance
		public readonly string name;
		public byte numVariants;
		public Texture[] variantTextures;
		public Vector4[] variantUVs;
		public ushort type;
		public string grassMaterial; //Change to material id later?

		protected virtual string[] Variants => throw new NotImplementedException();
		#pragma warning disable 0675
		public virtual byte GetVariant(int x,int y) => (byte)((byte)(((y*y)+(x*x*y)))%numVariants);
		#pragma warning restore 0675

		//TODO: Move this, don't use virtual methods in the constructor.
		public TileType()
		{
			name = GetType().Name;
			var variants = Variants;
			numVariants = (byte)variants.Length;
			variantUVs = new Vector4[numVariants];
			variantTextures = new Texture[numVariants];
			for(int i=0;i<variants.Length;i++) {
				variantTextures[i] = Resources.Import<Texture>(variants[i]);
			}

			OnInit();
		}

		public abstract PhysicMaterial GetMaterial(Vector3? atPoint = null);

		public virtual void OnInit() { }
		public virtual void ModifyGrassMesh(Chunk chunk,Tile tile,Vector2Int tilePos,Vector3 localPos,Vector3 tileNormal,MeshInfo mesh) => throw new NotImplementedException();

		public object Clone() => MemberwiseClone();
		public void Dispose() {}

		public static void Initialize()
		{
			if(initialized) {
				return;
			}

			var assembly = Assembly.GetCallingAssembly();
			var thisType = typeof(TileType);
			var tileTypes = assembly.GetTypes().Where(t => t!=thisType && !t.IsAbstract && thisType.IsAssignableFrom(t)).ToArray();

			typeCount = (ushort)tileTypes.Length;
			byId = new TileType[typeCount];
			byName = new Dictionary<string,TileType>();

			var idType = typeof(IDs<>);

			for(ushort i = 0;i<tileTypes.Length;i++) {
				var type = tileTypes[i];
				var instance = (TileType)Activator.CreateInstance(type);
				instance.type = i;

				var flags = BindingFlags.Static|BindingFlags.Public|BindingFlags.NonPublic;

				var idTypeGeneric = idType.MakeGenericType(type);
				idTypeGeneric.GetField(nameof(IDs<TileType>.Id),flags).SetValue(null,i);
				idTypeGeneric.GetField(nameof(IDs<TileType>.IdUShort),flags).SetValue(null,i);

				byId[i] = instance;
				byName[instance.name] = instance;
			}

			if(Netplay.isClient) {
				GenerateTexture();
			}

			initialized = true;
		}
		public static void GenerateTexture() 
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
			var pixels = new Pixel[fullSize,fullSize];

			for(int yy = 0;yy<fullSize;yy++) {
				for(int xx = 0;xx<fullSize;xx++) {
					pixels[xx,yy] = new Pixel(255,0,255,255);
				}
			}

			for(var i=0;i<typeCount;i++) {
				var tile = byId[i];
				for(int j=0;j<tile.numVariants;j++) {
					//tile.variantUVs[j] = new Vector4((x/(float)tileLength),1f-((y+1)/(float)tileLength),((x+1)/(float)tileLength),1f-(y/(float)tileLength));
					tile.variantUVs[j] = new Vector4(x,y,x+1,y+1)/tileLength;
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
		}
		public static T Get<T>() where T : TileType => (T)byId[IDs<T>.Id];
		public static ushort GetId<T>() where T : TileType => IDs<T>.IdUShort;
	}
}
