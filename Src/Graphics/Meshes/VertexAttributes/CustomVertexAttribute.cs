using Dissonance.Engine.Core;
using Dissonance.Engine.Graphics.Meshes.Buffers;
using Dissonance.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace Dissonance.Engine.Graphics.Meshes.VertexAttributes
{
	public abstract class CustomVertexAttribute
	{
		private static class IDs<T> where T : CustomVertexAttribute
		{
#pragma warning disable CS0649 //Value is never assigned to.
			public static int id;
#pragma warning restore
		}

		internal static CustomVertexAttribute[] instances;

		private static Dictionary<Type, int> idByType;

		public static int Count { get; private set; }

		public string NameId { get; protected set; }
		public VertexAttribPointerType PointerType { get; protected set; }
		public bool IsNormalized { get; protected set; }
		public int Size { get; protected set; }
		public int Stride { get; protected set; }
		public int Offset { get; protected set; }

		public abstract Type BufferType { get; }

		internal CustomVertexAttribute() { }

		internal static void Initialize()
		{
			if(CustomVertexBuffer.TypeById == null) {
				throw new InvalidOperationException($"{nameof(CustomVertexAttribute)}.{nameof(Initialize)}() must be called after {nameof(CustomVertexBuffer)}.{nameof(CustomVertexAttribute.Initialize)}().");
			}

			idByType = new Dictionary<Type, int>();

			var instanceList = new List<CustomVertexAttribute>();
			var bufferAttachmentsList = new List<int>[CustomVertexBuffer.Count];

			foreach(var type in AssemblyCache.AllTypes.Where(t => !t.IsAbstract && typeof(CustomVertexAttribute).IsAssignableFrom(t) && t.BaseType.IsGenericType && t.BaseType.GetGenericTypeDefinition() == typeof(CustomVertexAttribute<>))) {
				var instance = (CustomVertexAttribute)Activator.CreateInstance(type, true);

				int id = instanceList.Count;

				instanceList.Add(instance);

				//Set attribute's id.
				typeof(IDs<>)
					.MakeGenericType(type)
					.GetField(nameof(IDs<CustomVertexAttribute>.id), BindingFlags.Public | BindingFlags.Static)
					.SetValue(null, id);

				idByType[type] = id;

				//Add this vertex attribute's id to its vertex buffer's attribute attachments list.
				var vertexBufferType = type.BaseType.GetGenericArguments()[0];
				int vertexBufferId = CustomVertexBuffer.GetId(vertexBufferType);
				var attachmentsList = bufferAttachmentsList[vertexBufferId] ?? (bufferAttachmentsList[vertexBufferId] = new List<int>());

				attachmentsList.Add(id);
			}

			instances = instanceList.ToArray();
			Count = instances.Length;

			CustomVertexBuffer.AttributeAttachmentsById = bufferAttachmentsList.Select(list => (list ?? new List<int>()).AsReadOnly()).ToList().AsReadOnly();
		}

		public static int GetId<T>() where T : CustomVertexAttribute => IDs<T>.id;
		public static int GetId(Type type) => idByType[type];
		public static T GetInstance<T>() where T : CustomVertexAttribute => (T)instances[IDs<T>.id];
		public static CustomVertexAttribute GetInstance(Type type) => instances[idByType[type]];
		public static CustomVertexAttribute GetInstance(int id) => instances[id];
	}

	public abstract class CustomVertexAttribute<TBuffer> : CustomVertexAttribute where TBuffer : CustomVertexBuffer, new()
	{
		public override Type BufferType => typeof(TBuffer);

		protected CustomVertexAttribute() : base()
		{
			//This is quite weird.

			Init(out var nameId, out var pointerType, out var isNormalized, out var size, out var stride, out var offset);

			NameId = nameId;
			PointerType = pointerType;
			IsNormalized = isNormalized;
			Size = size;
			Stride = stride;
			Offset = offset;
		}

		public abstract void Init(out string nameId, out VertexAttribPointerType pointerType, out bool isNormalized, out int size, out int stride, out int offset);
	}
}
