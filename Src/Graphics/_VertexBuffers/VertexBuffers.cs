﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Dissonance.Engine.Graphics
{
	[Autoload(DisablingGameFlags = GameFlags.NoGraphics)]
	[ModuleDependency(typeof(Rendering))]
	public sealed class VertexBuffers : EngineModule
	{
		internal static class IDs<T> where T : CustomVertexBuffer
		{
#pragma warning disable CS0649
			public static int Id;
#pragma warning restore CS0649
		}

		private static List<Type> typeById;
		private static Dictionary<Type, int> idByType;
		private static List<int>[] attributeAttachmentIdsByBufferIds;

		public static int Count { get; private set; }

		public static IReadOnlyList<IReadOnlyList<int>> AttributeAttachmentIdsByBufferIds => attributeAttachmentIdsByBufferIds;

		protected override void PreInit()
		{
			typeById = new();
			idByType = new();
			attributeAttachmentIdsByBufferIds = Array.Empty<List<int>>();

			AssemblyRegistrationModule.OnAssemblyRegistered += static (assembly, types) => {
				foreach (var type in types) {
					if (type.IsAbstract || !typeof(CustomVertexBuffer).IsAssignableFrom(type)) {
						continue;
					}

					typeof(IDs<>)
						.MakeGenericType(type)
						.GetField(nameof(IDs<CustomVertexBuffer>.Id), BindingFlags.Public | BindingFlags.Static)
						.SetValue(null, typeById.Count);

					idByType[type] = typeById.Count;

					typeById.Add(type);
				}

				Count = typeById.Count;
			};
		}

		public static int GetId<T>() where T : CustomVertexBuffer
			=> IDs<T>.Id;

		public static int GetId(Type type)
			=> idByType[type];

		public static Type GetType(int id)
			=> typeById[id];

		public static CustomVertexBuffer CreateInstance(int id)
			=> (CustomVertexBuffer)Activator.CreateInstance(GetType(id), true);

		internal static void AddAttributeAttachment(int vertexBufferId, int attachmentId)
		{
			if (attributeAttachmentIdsByBufferIds.Length < vertexBufferId + 1) {
				Array.Resize(ref attributeAttachmentIdsByBufferIds, vertexBufferId + 1);
			}

			var list = attributeAttachmentIdsByBufferIds[vertexBufferId] ??= new();

			list.Add(attachmentId);
		}
	}
}
