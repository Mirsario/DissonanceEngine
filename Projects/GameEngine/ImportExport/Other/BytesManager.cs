using System.IO;

namespace GameEngine
{
	public class BytesManager : AssetManager<byte[]>
	{
		public override string[] Extensions => new [] { ".bytes" };
		
		public override byte[] Import(Stream stream,string fileName)
		{
			var bytes = new byte[stream.Length];
			stream.Read(bytes,0,bytes.Length);
			return bytes;
		}
		public override void Export(byte[] bytes,Stream stream)
		{
			stream.Write(bytes,0,bytes.Length);
		}
	}
}