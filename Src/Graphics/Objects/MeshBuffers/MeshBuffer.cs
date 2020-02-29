using System;
using System.Runtime.InteropServices;

#pragma warning disable CS0649 //Value is never assigned to.

namespace Dissonance.Engine.Graphics
{
	public abstract class MeshBuffer : IDisposable
	{
		public Mesh mesh;

		public uint BufferId { get; protected set; }
		public uint DataLength { get; protected set; }

		public abstract void Apply();
		public abstract void Dispose();
		public abstract void SetData(byte[] data);

		protected unsafe static void SetDataHelper<T>(ref T[] data,byte[] byteData) where T : unmanaged
		{
			if(byteData==null) {
				data = null;
				return;
			}

			int tSize = Marshal.SizeOf<T>();

			if(byteData.Length%tSize!=0) {
				throw new ArgumentException($"Data array's length {byteData.Length} must be dividable by size of {typeof(T).Name}, which is {tSize}.");
			}

			int newLength = byteData.Length/tSize;

			data = new T[newLength];

			if(byteData.Length==0) {
				return;
			}

			fixed(byte* bytePtr = byteData) {
				var tPtr = (T*)bytePtr;

				for(int i = 0;i<data.Length;i++) {
					data[i] = tPtr[i];
				}
			}
		}
	}
}
