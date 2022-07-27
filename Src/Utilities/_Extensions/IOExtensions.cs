using System.IO;

namespace Dissonance.Engine.Utilities;

public static class IOExtensions
{
	// ReadVectorX

	public static Vector2 ReadVector2(this BinaryReader reader)
		=> new(reader.ReadSingle(), reader.ReadSingle());

	public static Vector3 ReadVector3(this BinaryReader reader)
		=> new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

	public static Vector4 ReadVector4(this BinaryReader reader)
		=> new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

	public static Vector2Int ReadVector2Int(this BinaryReader reader)
		=> new(reader.ReadInt32(), reader.ReadInt32());

	public static Vector3Int ReadVector3Int(this BinaryReader reader)
		=> new(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());

	public static Vector4Int ReadVector4Int(this BinaryReader reader)
		=> new(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());

	// Write(VectorX)

	public static void Write(this BinaryWriter writer, Vector2 vec)
	{
		writer.Write(vec.X);
		writer.Write(vec.Y);
	}

	public static void Write(this BinaryWriter writer, Vector3 vec)
	{
		writer.Write(vec.X);
		writer.Write(vec.Y);
		writer.Write(vec.Z);
	}

	public static void Write(this BinaryWriter writer, Vector4 vec)
	{
		writer.Write(vec.X);
		writer.Write(vec.Y);
		writer.Write(vec.Z);
		writer.Write(vec.W);
	}

	public static void Write(this BinaryWriter writer, Vector2Int vec)
	{
		writer.Write(vec.X);
		writer.Write(vec.Y);
	}

	public static void Write(this BinaryWriter writer, Vector3Int vec)
	{
		writer.Write(vec.X);
		writer.Write(vec.Y);
		writer.Write(vec.Z);
	}

	public static void Write(this BinaryWriter writer, Vector4Int vec)
	{
		writer.Write(vec.X);
		writer.Write(vec.Y);
		writer.Write(vec.Z);
		writer.Write(vec.W);
	}

	public static void Write(this BinaryWriter writer, Quaternion q)
	{
		writer.Write(q.X);
		writer.Write(q.Y);
		writer.Write(q.Z);
		writer.Write(q.W);
	}
}
