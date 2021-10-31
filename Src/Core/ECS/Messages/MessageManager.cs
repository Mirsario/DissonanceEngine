using System;
using System.Collections.Generic;

namespace Dissonance.Engine
{
	internal sealed class MessageManager : EngineModule
	{
		private static class MessageData<T> where T : struct
		{
			public static List<T>[] MessagesByWorld = Array.Empty<List<T>>();

			static MessageData()
			{
				ClearLists += Clear;
			}

			private static void Clear()
			{
				for (int i = WorldManager.DefaultWorldId; i < MessagesByWorld.Length; i++) {
					MessagesByWorld[i].Clear();
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
			int oldArraySize = MessageData<T>.MessagesByWorld.Length;

			if (worldId >= oldArraySize) {
				int newArraySize = worldId + 1;

				Array.Resize(ref MessageData<T>.MessagesByWorld, newArraySize);

				for (int i = oldArraySize; i < newArraySize; i++) {
					MessageData<T>.MessagesByWorld[i] = new List<T>();
				}
			}

			MessageData<T>.MessagesByWorld[worldId].Add(message);
		}

		internal static MessageEnumerator<T> ReadMessages<T>(int worldId) where T : struct
		{
			if (worldId >= MessageData<T>.MessagesByWorld.Length) {
				return default;
			}

			return new MessageEnumerator<T>(MessageData<T>.MessagesByWorld[worldId]);
		}

		internal static void ClearMessages()
		{
			ClearLists?.Invoke();
		}
	}
}
