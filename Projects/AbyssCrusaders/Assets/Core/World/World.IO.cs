using System.IO;
using System.Linq;
using System.Text;
using GameEngine;

namespace AbyssCrusaders.Core
{
	partial class World
	{
		public static readonly byte[] FileHeader = Encoding.UTF8.GetBytes("game world file."); //h

		public bool Save(string path)
		{
			using var writer = new BinaryWriter(File.OpenWrite(path));

			writer.Write(FileHeader);
			writer.Write(name);
			writer.Write(displayName);
			writer.Write(new Vector2Int(width,height));

			int chunkWidth = ChunkWidth;
			int chunkHeight = ChunkHeight;

			for(int y = 0;y<chunkHeight;y++) {
				for(int x = 0;x<chunkWidth;x++) {
					chunks[x,y].Save(writer);
				}
			}

			return true;
		}

		public static bool ReadInfoHeader(BinaryReader reader,out WorldInfo info)
		{
			info = default;

			try {
				if(!reader.ReadBytes(16).SequenceEqual(FileHeader)) {
					return false;
				}

				info.name = reader.ReadString();
				info.displayName = reader.ReadString();
				info.size = reader.ReadVector2Int();

				return info.size.x%Chunk.ChunkSize==0 && info.size.y%Chunk.ChunkSize==0;
			}
			catch {
				return false;
			}
		}
		public bool Load(string path)
		{
			using var reader = new BinaryReader(File.OpenRead(path));

			if(!ReadInfoHeader(reader,out WorldInfo info)) {
				return false;
			}

			localPath = path;

			return true;
		}
	}
}
