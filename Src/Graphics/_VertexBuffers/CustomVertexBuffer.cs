using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Silk.NET.OpenGL;
using static Dissonance.Engine.Graphics.OpenGLApi;

#pragma warning disable CS0649 // Value is never assigned to.

namespace Dissonance.Engine.Graphics
{
	public abstract class CustomVertexBuffer : MeshBuffer
	{
		internal static class IDs<T> where T : CustomVertexBuffer
		{
			public static int id;
		}

		protected const BufferTargetARB Target = BufferTargetARB.ArrayBuffer;

		public static IReadOnlyList<Type> TypeById { get; private set; }
		public static IReadOnlyList<IReadOnlyList<int>> AttributeAttachmentsById { get; internal set; }
		public static int Count { get; private set; }

		private static Dictionary<Type, int> idByType;

		public readonly int TypeId;

		internal protected CustomVertexBuffer()
		{
			TypeId = GetId(GetType());
		}

		public static int GetId<T>() where T : CustomVertexBuffer => IDs<T>.id;
		public static int GetId(Type type) => idByType[type];
		public static Type GetType(int id) => TypeById[id];
		public static CustomVertexBuffer CreateInstance(int id) => (CustomVertexBuffer)Activator.CreateInstance(GetType(id), true);

		internal static void Initialize()
		{
			idByType = new Dictionary<Type, int>();

			var typeList = new List<Type>();

			foreach (var type in AssemblyCache.AllTypes.Where(t => !t.IsAbstract && typeof(CustomVertexBuffer).IsAssignableFrom(t))) {
				typeof(IDs<>)
					.MakeGenericType(type)
					.GetField(nameof(IDs<CustomVertexBuffer>.id), BindingFlags.Public | BindingFlags.Static)
					.SetValue(null, typeList.Count);

				idByType[type] = typeList.Count;

				typeList.Add(type);
			}

			TypeById = typeList.AsReadOnly();
			Count = typeList.Count;
		}
	}

	public abstract class CustomVertexBuffer<T> : CustomVertexBuffer, IMeshBuffer<T> where T : unmanaged
	{
		public T[] data;

		public unsafe override void Apply()
		{
			var attributes = AttributeAttachmentsById[TypeId];

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

				var attribute = CustomVertexAttribute.GetInstance((int)attributeId);

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
