using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GameEngine;

namespace Game
{
	public class TileType : ICloneable, IDisposable, IHasMaterial
	{
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
		public virtual byte GetVariant(int x,int y) => (byte)(((y*y)+(x*x*y))%numVariants);
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

		public virtual void OnInit() {}
		public virtual void ModifyGrassMesh(Chunk chunk,Tile tile,Vector2Int tilePos,Vector3 localPos,Vector3 tileNormal,MeshInfo mesh) => throw new NotImplementedException();
		public virtual PhysicMaterial GetMaterial(Vector3? atPoint = null) => throw new NotImplementedException();

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
			for(ushort i=0;i<tileTypes.Length;i++) {
				var type = tileTypes[i];
				var instance = (TileType)Activator.CreateInstance(type);
				instance.type = i;
				byName[instance.name] = instance;
				byId[i] = instance;
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
		}

		public object Clone()
		{
			return MemberwiseClone();
		}
		public void Dispose()
		{

		}
	}
}
