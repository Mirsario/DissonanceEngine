using System;
using System.Runtime.InteropServices;
using Dissonance.Framework.Graphics;

#pragma warning disable CS0649 //Value is never assigned to.

namespace Dissonance.Engine.Graphics
{
	public class IndexBuffer : MeshBuffer
	{
		protected const BufferTarget Target = BufferTarget.ElementArrayBuffer;

		public uint[] data;

		public override void Apply()
		{
			if(data==null) {
				if(BufferId!=0) {
					GL.DeleteBuffer(BufferId);

					BufferId = 0;
				}

				return;
			}

			if(BufferId==0) {
				BufferId = GL.GenBuffer();
			}

			GL.BindBuffer(BufferTarget.ElementArrayBuffer,BufferId);

			DataLength = (uint)data.Length;

			GL.BufferData(BufferTarget.ElementArrayBuffer,(int)(DataLength*sizeof(int)),data,mesh.bufferUsage);
		}
		public override void Dispose()
		{
			if(BufferId!=0) {
				GL.DeleteBuffer(BufferId);

				BufferId = 0;
			}
		}
		public override void SetData(byte[] data) => SetDataHelper(ref this.data,data);
	}
}
