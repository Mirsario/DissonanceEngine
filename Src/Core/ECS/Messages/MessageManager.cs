﻿using System;
using System.Collections.Generic;

namespace Dissonance.Engine
{
	internal sealed class MessageManager : EngineModule
	{
		private static class MessageData<T> where T : struct
		{
			public static List<T>[] messagesByWorld = Array.Empty<List<T>>();

			static MessageData()
			{
				ClearLists += Clear;
			}

			private static void Clear()
			{
				for (int i = WorldManager.DefaultWorldId; i < messagesByWorld.Length; i++) {
					messagesByWorld[i].Clear();
				}
			}
		}

		private static event Action ClearLists;

		[HookPosition(1000)]
		protected override void FixedUpdate()
		{
			ClearMessages();
		}

		[HookPosition(1000)]
		protected override void RenderUpdate()
		{
			ClearMessages();
		}

		internal static void SendMessage<T>(int worldId, in T message) where T : struct
		{
			if (worldId >= MessageData<T>.messagesByWorld.Length) {
				Array.Resize(ref MessageData<T>.messagesByWorld, worldId + 1);

				MessageData<T>.messagesByWorld[worldId] = new List<T>();
			}

			MessageData<T>.messagesByWorld[worldId].Add(message);
		}

		internal static MessageEnumerator<T> ReadMessages<T>(int worldId) where T : struct
		{
			if (worldId >= MessageData<T>.messagesByWorld.Length) {
				return default;
			}

			return new MessageEnumerator<T>(MessageData<T>.messagesByWorld[worldId]);
		}

		internal static void ClearMessages()
		{
			ClearLists?.Invoke();
		}
	}
}
