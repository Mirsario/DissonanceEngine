using System;
using System.Runtime.InteropServices;
using Silk.NET.OpenGL;
using static Dissonance.Engine.Graphics.OpenGLApi;

namespace Dissonance.Engine.Graphics
{
	public abstract class CustomVertexBuffer : MeshBuffer
	{
		protected const BufferTargetARB Target = BufferTargetARB.ArrayBuffer;

		public readonly int TypeId;

		internal protected CustomVertexBuffer()
		{
			TypeId = VertexBuffers.GetId(GetType());
		}
	}

	public abstract class CustomVertexBuffer<T> : CustomVertexBuffer, IMeshBuffer<T> where T : unmanaged
	{
		public T[] data;

		public unsafe override void Apply()
		{
			var attributes = VertexBuffers.AttributeAttachmentIdsByBufferIds[TypeId];

			if (data == null) {
				if (BufferId != 0) {
					OpenGL.DeleteBuffer(BufferId);

					BufferId = 0;
				}

				foreach (uint attributeId in attributes) {
					OpenGL.DisableVertexAttribArray(attributeId);
				}

				return;
			}

			if (BufferId == 0) {
				BufferId = OpenGL.GenBuffer();
			}

			int tSize = Marshal.SizeOf<T>();

			OpenGL.BindBuffer(Target, BufferId);

			DataLength = (uint)data.Length;

			fixed (T* dataPtr = data) {
				OpenGL.BufferData(Target, (uint)(DataLength * tSize), dataPtr, Mesh.BufferUsage);
			}

			foreach (uint attributeId in attributes) {
				OpenGL.EnableVertexAttribArray(attributeId);

				var attribute = VertexAttributes.GetInstance((int)attributeId);

				OpenGL.VertexAttribPointer(attributeId, attribute.Size, attribute.PointerType, attribute.IsNormalized, attribute.Stride, (void*)attribute.Offset);
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

		public override void SetData(byte[] data)
			=> SetDataHelper(ref this.data, data);

		public void SetData<TProvidedData>(byte[] byteData, Func<TProvidedData, T> cast) where TProvidedData : unmanaged
			=> SetDataHelper(ref data, byteData, cast);
	}
}
