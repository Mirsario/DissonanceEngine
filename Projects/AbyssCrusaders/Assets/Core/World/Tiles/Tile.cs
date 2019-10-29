using AbyssCrusaders.Core;
using GameEngine;
using System.IO;
using System.Runtime.InteropServices;

namespace AbyssCrusaders.Core
{
	public struct Tile : ISaveable
	{
		public static readonly int SizeInBytes = Marshal.SizeOf<Tile>();
		
		public ushort type;
		public ushort wall;
		public byte tileFrame;
		public byte wallFrame;
		public byte style;
		public byte tileDamage;
		public byte wallDamage;

		public TilePreset TilePreset => TilePreset.byId[type];
		public TilePreset WallPreset => TilePreset.byId[wall];

		public Tile(ushort type = 0,ushort wall = 0,byte style = 0,byte tileFrame = 0,byte wallFrame = 0)
		{
			this.type = type;
			this.wall = wall;
			this.style = style;
			this.tileFrame = tileFrame;
			this.wallFrame = wallFrame;

			tileDamage = 0;
			wallDamage = 0;
		}

		public void Save(BinaryWriter writer)
		{
			writer.Write(type);
			writer.Write(wall);
			writer.Write(tileFrame);
			writer.Write(wallFrame);
		}
		public void Load(BinaryReader reader)
		{
			type = reader.ReadUInt16();
			wall = reader.ReadUInt16();
			tileFrame = reader.ReadByte();
			wallFrame = reader.ReadByte();
		}
		public void PlayTileSound(int x,int y,string type)
		{
			if(TilePreset is IHasMaterial materialProvaider) {
				var material = materialProvaider.GetMaterial();

				if(material.TryGetSound(type,out string sound)) {
					SoundInstance.Create(sound,new Vector2(x+0.5f,y+0.5f));
				}
			}
		}
		public void PlayWallSound(int x,int y,string type)
		{
			if(WallPreset is IHasMaterial materialProvaider) {
				var material = materialProvaider.GetMaterial();

				if(material.TryGetSound(type,out string sound)) {
					SoundInstance.Create(sound,new Vector2(x+0.5f,y+0.5f));
				}
			}
		}
	}
}
