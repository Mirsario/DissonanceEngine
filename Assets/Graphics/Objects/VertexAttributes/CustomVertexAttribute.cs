using Dissonance.Framework.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#pragma warning disable CS0649 //Value is never assigned to.

namespace GameEngine.Graphics
{
	public abstract class CustomVertexAttribute
	{
		private static class IDs<T> where T : CustomVertexAttribute
		{
			public static int id;
		}

		internal static CustomVertexAttribute[] instances;

		public string NameId { get; protected set; }
		public VertexAttribPointerType PointerType { get; protected set; }
		public bool IsNormalized { get; protected set; }
		public int Size { get; protected set; }
		public int Offset { get; protected set; }

		public static int Count { get; private set; }

		internal CustomVertexAttribute() { }

		internal static void Initialize()
		{
			if(CustomVertexBuffer.TypeById==null) {
				throw new InvalidOperationException($"{nameof(CustomVertexAttribute)}.{nameof(Initialize)}() must be called after {nameof(CustomVertexBuffer)}.{nameof(CustomVertexAttribute.Initialize)}().");
			}

			var instanceList = new List<CustomVertexAttribute>();
			var bufferAttachmentsList = new List<int>[CustomVertexBuffer.Count];

			foreach(var type in ReflectionCache.allTypes.Where(t => !t.IsAbstract && typeof(CustomVertexAttribute).IsAssignableFrom(t) && t.BaseType.IsGenericType && t.BaseType.GetGenericTypeDefinition()==typeof(CustomVertexAttribute<>))) {
				var instance = (CustomVertexAttribute)Activator.CreateInstance(type,true);

				int id = instanceList.Count;

				instanceList.Add(instance);

				//Set attribute's id.
				typeof(IDs<>)
					.MakeGenericType(type)
					.GetField(nameof(IDs<CustomVertexAttribute>.id),BindingFlags.Public|BindingFlags.Static)
					.SetValue(null,id);

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
		public static T GetInstance<T>() where T : CustomVertexAttribute => (T)instances[IDs<T>.id];
		public static CustomVertexAttribute GetInstance(int id) => instances[id];
	}

	public abstract class CustomVertexAttribute<TBuffer> : CustomVertexAttribute where TBuffer : CustomVertexBuffer, new()
	{
		protected CustomVertexAttribute() : base()
		{
			//This is quite weird.

			Init(out var nameId,out var pointerType,out var isNormalized,out var size,out var offset);

			NameId = nameId;
			PointerType = pointerType;
			IsNormalized = isNormalized;
			Size = size;
			Offset = offset;
		}

		public abstract void Init(out string nameId,out VertexAttribPointerType pointerType,out bool isNormalized,out int size,out int offset);
	}
}
