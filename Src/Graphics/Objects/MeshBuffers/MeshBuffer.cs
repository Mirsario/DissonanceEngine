using System;
using System.Runtime.InteropServices;

#pragma warning disable CS0649 //Value is never assigned to.

namespace Dissonance.Engine.Graphics
{
	public interface IMeshBuffer : IDisposable
	{
		void SetData(byte[] byteData);
	}

	public interface IMeshBuffer<TData> : IMeshBuffer
	{
		void SetData<TProvidedData>(byte[] byteData,Func<TProvidedData,TData> cast) where TProvidedData : unmanaged;
	}

	public abstract class MeshBuffer : IMeshBuffer
	{
		public Mesh mesh;

		public uint BufferId { get; protected set; }
		public uint DataLength { get; protected set; }

		public abstract void Apply();
		public abstract void Dispose();
		public abstract void SetData(byte[] data);

		protected unsafe static void SetDataHelper<T>(ref T[] data,byte[] byteData) where T : unmanaged
		{
			if(!PrepareSetData<T,T>(ref data,byteData)) {
				return;
			}

			fixed(byte* bytePtr = byteData) {
				var tPtr = (T*)bytePtr;

				for(int i = 0;i<data.Length;i++) {
					data[i] = tPtr[i];
				}
			}
		}
		protected unsafe static void SetDataHelper<TLocalData,TProvidedData>(ref TLocalData[] data,byte[] byteData,Func<TProvidedData,TLocalData> cast)
			where TLocalData : unmanaged
			where TProvidedData : unmanaged
		{
			if(!PrepareSetData<TLocalData,TProvidedData>(ref data,byteData)) {
				return;
			}

			fixed(byte* bytePtr = byteData) {
				var tPtr = (TProvidedData*)bytePtr;

				for(int i = 0;i<data.Length;i++) {
					data[i] = cast(tPtr[i]);
				}
			}
		}

		private static bool PrepareSetData<TLocalData,TProvidedData>(ref TLocalData[] data,byte[] byteData)
			where TLocalData : unmanaged
			where TProvidedData : unmanaged
		{
			if(byteData==null) {
				data = null;
				return false;
			}

			int tSize = Marshal.SizeOf<TProvidedData>();

			if(byteData.Length%tSize!=0) {
				throw new ArgumentException($"Data array's length {byteData.Length} must be dividable by size of {typeof(TProvidedData).Name}, which is {tSize}.");
			}

			int newLength = byteData.Length/tSize;

			data = new TLocalData[newLength];

			return byteData.Length!=0;
		}
	}
}
