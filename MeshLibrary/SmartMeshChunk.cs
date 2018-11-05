using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ionic.Zlib;

namespace SmartMeshLibrary
{
	public class SmartMeshChunk : IDisposable
	{
		public readonly string name;
		/*public readonly long position;
		public readonly long dataPosition;*/
		public readonly ulong dataLength;
		public readonly Stream stream;

		public SmartMeshChunk(string name,Stream stream) //,long position,long dataPosition,ulong length,ulong dataLength)
		{
			this.name = name;
			this.stream = stream;
			/*this.position = position;
			this.dataPosition = dataPosition;
			this.length = length;
			this.dataLength = dataLength;*/
		}

		public void Dispose()
		{
			stream?.Dispose();
		}

		public BinaryReader GetReader(long? setPosition = 0)
		{
			if(setPosition!=null) {
				stream.Position = setPosition.Value;
			}
			return new BinaryReader(stream);
		}
		public BinaryWriter GetWriter(long? setPosition = 0)
		{
			if(setPosition!=null) {
				stream.Position = setPosition.Value;
			}
			return new BinaryWriter(stream);
		}

		public static SmartMeshChunk[] ReadChunks(BinaryReader reader)
		{
			var stream = reader.BaseStream;
			uint chunkAmount = reader.ReadUInt32();
			var chunks = new SmartMeshChunk[chunkAmount];
			for(uint i = 0;i<chunkAmount;i++) {
				string chunkName = Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadByte())); //new string(reader.ReadChars(reader.ReadByte()));
				ulong chunkDataLength = reader.ReadUInt64();

				var inputStream = new MemoryStream(reader.ReadBytes((int)chunkDataLength)); //Shouldn't there be an overload which'd take a long?
				var chunkStream = new MemoryStream();
				using(var deflateStream = new DeflateStream(inputStream,CompressionMode.Decompress)) {
					deflateStream.CopyTo(chunkStream);
				}
				chunkStream.Position = 0;

				chunks[i] = new SmartMeshChunk(chunkName,chunkStream);
			}
			return chunks;
		}
		public static void WriteChunks(BinaryWriter writer,SmartMeshChunk[] chunks)
		{
			uint chunkAmount = (uint)chunks.Length;
			writer.Write(chunkAmount);
			for(uint i = 0;i<chunkAmount;i++) {
				var chunk = chunks[i];
				writer.Write(Encoding.UTF8.GetBytes(chunk.name.ToCharArray()));
				//long lengthPos = writer.BaseStream.Position;

				chunk.stream.Position = 0;
				using(var deflateStream = new DeflateStream(chunk.stream,CompressionMode.Compress)) {
					Console.WriteLine($"{chunk.stream.Length} compressed to {deflateStream.Length}");
					Console.WriteLine($"{chunk.stream.Position} / {deflateStream.Position} positions");
					writer.Write(deflateStream.Position);
					deflateStream.CopyTo(writer.BaseStream);
				}
			}
		}
	}
}
