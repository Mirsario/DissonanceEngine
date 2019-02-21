using System.IO;

namespace AbyssCrusaders
{
	public struct Tile : ISaveable
	{
		public TilePreset TilePreset => TilePreset.byId[type];
		public TilePreset WallPreset => TilePreset.byId[wall];
		
		public ushort type;
		public ushort wall;
		public byte tileFrame;
		public byte wallFrame;
		public byte style;

		public Tile(ushort type = 0,ushort wall = 0,byte style = 0,byte tileFrame = 0,byte wallFrame = 0)
		{
			this.type = type;
			this.wall = wall;
			this.style = style;
			this.tileFrame = tileFrame;
			this.wallFrame = wallFrame;
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
	}
}
