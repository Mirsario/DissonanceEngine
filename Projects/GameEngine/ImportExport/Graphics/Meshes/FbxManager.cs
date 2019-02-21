using System;
using System.IO;

namespace GameEngine
{
	public class FbxManager : AssetManager<Mesh>
	{
		public override string[] Extensions => new [] { ".fbx" };

		private static char[] signature = "Kaydara FBX Binary  \0\x1a\0".ToCharArray();
		
		public override Mesh Import(Stream stream,string fileName)
		{
			Debug.Log("Loading FBX-"+fileName);
			var reader = new BinaryReader(stream);
			if(string.Compare(new string(reader.ReadChars(signature.Length)),new string(signature))!=0) {
				throw new NotSupportedException("Specified stream is not a fbx file");
			}
			reader.ReadBytes(2);//Skip 2 useless bytes
			uint versionNumber = reader.ReadUInt32();
			Debug.Log("FBX version number-"+versionNumber);
			ReadNode(reader);

			throw new NotImplementedException();
		}
		public void ReadNode(BinaryReader reader)
		{
			uint endOffset = reader.ReadUInt32();
			long endPos = reader.BaseStream.Position+endOffset;
			uint numProperties = reader.ReadUInt32();
			uint propertyCount = reader.ReadUInt32();
			byte nameLength = reader.ReadByte();
			string name = new string(reader.ReadChars(nameLength));
			Debug.Log("endOffset: "+endOffset);
			Debug.Log("numProperties: "+numProperties);
			Debug.Log("propertyListLength: "+propertyCount);
			Debug.Log("nameLength: "+nameLength);
			Debug.Log("name: "+name);
			for(int i=0;i<propertyCount;i++) {
				char type = reader.ReadChar();
				Debug.Log("property["+i+"] type is ''"+type+"''");
			}
			reader.BaseStream.Position = endPos;//Skip everything else
		}
	}
}
