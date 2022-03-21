using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dissonance.Engine.Graphics
{
	[Autoload(DisablingGameFlags = GameFlags.NoGraphics)]
	[ModuleDependency<VertexBuffers>]
	public sealed class VertexAttributes : EngineModule
	{
		private static class IDs<T> where T : CustomVertexAttribute
		{
#pragma warning disable CS0649
			public static int Id;
#pragma warning restore CS0649
		}

		private static Dictionary<Type, int> idByType = new();
		private static List<CustomVertexAttribute> instances = new();

		public static int Count => instances.Count;

		protected override void InitializeForAssembly(Assembly assembly)
		{
			foreach (var type in assembly.GetTypes()) {
				if (type.IsAbstract || !typeof(CustomVertexAttribute).IsAssignableFrom(type)) {
					continue;
				}

				var baseType = type.BaseType;

				if (!baseType.IsGenericType || baseType.GetGenericTypeDefinition() != typeof(CustomVertexAttribute<>)) {
					continue;
				}

				var instance = (CustomVertexAttribute)Activator.CreateInstance(type, true);
				int id = instances.Count;

				// Add to instance list
				instances.Add(instance);

				// Set attribute's id.
				typeof(IDs<>)
					.MakeGenericType(type)
					.GetField(nameof(IDs<CustomVertexAttribute>.Id), BindingFlags.Public | BindingFlags.Static)
					.SetValue(null, id);

				idByType[type] = id;

				// Add this vertex attribute's id to its vertex buffer's attribute attachments list.
				var vertexBufferType = type.BaseType.GetGenericArguments()[0];
				int vertexBufferId = VertexBuffers.GetId(vertexBufferType);

				VertexBuffers.AddAttributeAttachment(vertexBufferId, id);
			}
		}

		public static int GetId<T>() where T : CustomVertexAttribute
			=> IDs<T>.Id;

		public static int GetId(Type type)
			=> idByType[type];

		public static T GetInstance<T>() where T : CustomVertexAttribute
			=> (T)instances[IDs<T>.Id];

		public static CustomVertexAttribute GetInstance(Type type)
			=> instances[idByType[type]];

		public static CustomVertexAttribute GetInstance(int id)
			=> instances[id];
	}
}
