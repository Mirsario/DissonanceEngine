using System.IO;

namespace Dissonance.Engine.Utilities
{
	public static class IOExtensions
	{
		// ReadVectorX

		public static Vector2 ReadVector2(this BinaryReader reader)
			=> new Vector2(reader.ReadSingle(), reader.ReadSingle());

		public static Vector3 ReadVector3(this BinaryReader reader)
			=> new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

		public static Vector4 ReadVector4(this BinaryReader reader)
			=> new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

		public static Vector2Int ReadVector2Int(this BinaryReader reader)
			=> new Vector2Int(reader.ReadInt32(), reader.ReadInt32());

		public static Vector3Int ReadVector3Int(this BinaryReader reader)
			=> new Vector3Int(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());

		public static Vector4Int ReadVector4Int(this BinaryReader reader)
			=> new Vector4Int(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());

		// Write(VectorX)

		public static void Write(this BinaryWriter writer, Vector2 vec)
		{
			writer.Write(vec.x);
			writer.Write(vec.y);
		}

		public static void Write(this BinaryWriter writer, Vector3 vec)
		{
			writer.Write(vec.x);
			writer.Write(vec.y);
			writer.Write(vec.z);
		}

		public static void Write(this BinaryWriter writer, Vector4 vec)
		{
			writer.Write(vec.x);
			writer.Write(vec.y);
			writer.Write(vec.z);
			writer.Write(vec.w);
		}

		public static void Write(this BinaryWriter writer, Vector2Int vec)
		{
			writer.Write(vec.x);
			writer.Write(vec.y);
		}

		public static void Write(this BinaryWriter writer, Vector3Int vec)
		{
			writer.Write(vec.x);
			writer.Write(vec.y);
			writer.Write(vec.z);
		}

		public static void Write(this BinaryWriter writer, Vector4Int vec)
		{
			writer.Write(vec.x);
			writer.Write(vec.y);
			writer.Write(vec.z);
			writer.Write(vec.w);
		}

		public static void Write(this BinaryWriter writer, Quaternion q)
		{
			writer.Write(q.x);
			writer.Write(q.y);
			writer.Write(q.z);
			writer.Write(q.w);
		}
	}
}
