using System.IO;

namespace SmartMeshLibrary
{
	public static class SmartMeshUtils
	{
		#region IO
		#region Vector2Array
		public static TVector2[] ReadVector2Array<TVector2>(BinaryReader reader,NewVector2<TVector2> newVector2)
		{
			int arrayLength = reader.ReadInt32();
			var array = new TVector2[arrayLength];
			for(uint i = 0;i<arrayLength;i++) {
				array[i] = newVector2(reader.ReadSingle(),reader.ReadSingle());
			}
			return array;
		}
		public static void WriteVector2Array<TVector2>(BinaryWriter writer,TVector2[] array,Vector2ToXY<TVector2> vector2ToXY)
		{
			int arrayLength = array.Length;
			writer.Write(arrayLength);
			for(uint i = 0;i<arrayLength;i++) {
				var (x,y) = vector2ToXY(array[i]);
				writer.Write(x);
				writer.Write(y);
			}
		}
		#endregion
		#region Vector3Array
		public static TVector3[] ReadVector3Array<TVector3>(BinaryReader reader,NewVector3<TVector3> newVector3)
		{
			int arrayLength = reader.ReadInt32();
			var array = new TVector3[arrayLength];
			for(uint i = 0;i<arrayLength;i++) {
				array[i] = newVector3(reader.ReadSingle(),reader.ReadSingle(),reader.ReadSingle());
			}
			return array;
		}
		public static void WriteVector3Array<TVector3>(BinaryWriter writer,TVector3[] array,Vector3ToXYZ<TVector3> vector3ToXYZ)
		{
			int arrayLength = array.Length;
			writer.Write(arrayLength);
			for(uint i = 0;i<arrayLength;i++) {
				var (x,y,z) = vector3ToXYZ(array[i]);
				writer.Write(x);
				writer.Write(y);
				writer.Write(z);
			}
		}
		#endregion
		#region Vector4Array
		public static TVector4[] ReadVector4Array<TVector4>(BinaryReader reader,NewVector4<TVector4> newVector4)
		{
			int arrayLength = reader.ReadInt32();
			var array = new TVector4[arrayLength];
			for(uint i = 0;i<arrayLength;i++) {
				array[i] = newVector4(reader.ReadSingle(),reader.ReadSingle(),reader.ReadSingle(),reader.ReadSingle());
			}
			return array;
		}
		public static void WriteVector4Array<TVector4>(BinaryWriter writer,TVector4[] array,Vector4ToXYZW<TVector4> vector4ToXYZW)
		{
			int arrayLength = array.Length;
			writer.Write(arrayLength);
			for(uint i = 0;i<arrayLength;i++) {
				var (x,y,z,w) = vector4ToXYZW(array[i]);
				writer.Write(x);
				writer.Write(y);
				writer.Write(z);
				writer.Write(w);
			}
		}
		#endregion
		#region DynamicIntArray
		public static int[] ReadDynamicIntArray(BinaryReader reader)
		{
			IntType intType = (IntType)reader.ReadByte();
			int arrayLength = reader.ReadInt32();
			int[] array = new int[arrayLength];
			switch(intType) {
				case IntType.UInt8:
					for(uint i = 0;i<arrayLength;i++) {
						array[i] = reader.ReadByte();
					}
					break;
				case IntType.UInt16:
					for(uint i = 0;i<arrayLength;i++) {
						array[i] = reader.ReadUInt16();
					}
					break;
				case IntType.Int32:
					for(uint i = 0;i<arrayLength;i++) {
						array[i] = reader.ReadInt32();
					}
					break;
			}
			return array;
		}
		public static void WriteDynamicIntArray(BinaryWriter writer,int[] array,IntType? intType = null)
		{
			int arrayLength = array.Length;
			if(intType==null) {
				int maxVertexId = 0;
				for(uint i = 0;i<arrayLength;i++) {
					int index = array[i];
					if(index>maxVertexId) {
						maxVertexId = index;
					}
				}
				intType = maxVertexId<=byte.MaxValue ? IntType.UInt8 : (maxVertexId<=ushort.MaxValue ? IntType.UInt16 : IntType.Int32);
			}
			writer.Write((byte)intType);
			writer.Write(arrayLength);
			switch(intType) {
				case IntType.UInt8:
					for(uint i = 0;i<arrayLength;i++) {
						writer.Write((byte)array[i]);
					}
					break;
				case IntType.UInt16:
					for(uint i = 0;i<arrayLength;i++) {
						writer.Write((ushort)array[i]);
					}
					break;
				case IntType.Int32:
					for(uint i = 0;i<arrayLength;i++) {
						writer.Write(array[i]);
					}
					break;
			}
		}
		#endregion
		#endregion
	}
}
