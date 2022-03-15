using System;
using Silk.NET.OpenGL;
using static Dissonance.Engine.Graphics.OpenGLApi;

namespace Dissonance.Engine.Graphics
{
	public unsafe class IndexBuffer : MeshBuffer, IMeshBuffer<uint>
	{
		protected const BufferTargetARB Target = BufferTargetARB.ElementArrayBuffer;

		public uint[] data;

		public override void Apply()
		{
			if (data == null) {
				if (BufferId != 0) {
					OpenGL.DeleteBuffer(BufferId);

					BufferId = 0;
				}

				return;
			}

			if (BufferId == 0) {
				BufferId = OpenGL.GenBuffer();
			}

			OpenGL.BindBuffer(BufferTargetARB.ElementArrayBuffer, BufferId);

			DataLength = (uint)data.Length;

			fixed (void* dataPtr = data) {
				OpenGL.BufferData(BufferTargetARB.ElementArrayBuffer, (nuint)(DataLength * sizeof(int)), dataPtr, Mesh.BufferUsage);
			}
		}

		public override void Dispose()
		{
			if (BufferId != 0) {
				OpenGL.DeleteBuffer(BufferId);

				BufferId = 0;
			}

			GC.SuppressFinalize(this);
		}

		public override void SetData(byte[] byteData)
			=> SetDataHelper(ref data, byteData);

		public void SetData<TProvidedData>(byte[] byteData, Func<TProvidedData, uint> cast) where TProvidedData : unmanaged
			=> SetDataHelper(ref data, byteData, cast);
	}
}
