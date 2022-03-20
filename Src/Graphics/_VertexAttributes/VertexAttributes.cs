using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dissonance.Engine.Graphics
{
	[Autoload(DisablingGameFlags = GameFlags.NoGraphics)]
	[ModuleDependency(typeof(VertexBuffers))]
	public sealed class VertexAttributes : EngineModule
	{
		private static class IDs<T> where T : CustomVertexAttribute
		{
#pragma warning disable CS0649
			public static int Id;
#pragma warning restore CS0649
		}

		internal static List<CustomVertexAttribute> instances;

		private static Dictionary<Type, int> idByType;

		public static int Count { get; private set; }

		protected override void PreInit()
		{
			instances = new List<CustomVertexAttribute>();
			idByType = new Dictionary<Type, int>();

			AssemblyRegistrationModule.OnAssemblyRegistered += static (assembly, types) => {
				foreach (var type in types) {
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

				Count = instances.Count;
			};
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
