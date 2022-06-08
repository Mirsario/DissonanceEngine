namespace Dissonance.Engine;

public sealed class World
{
	internal readonly int Id;

	/// <summary> Whether or not this is a default engine-provided world. Default worlds cannot be removed. </summary>
	public bool IsDefault => Id == WorldManager.DefaultWorldId || Id == WorldManager.PrefabWorldId;

	public readonly WorldCreationOptions Options; 

	internal World(int id, WorldCreationOptions options)
	{
		Id = id;
		Options = options;
	}

	// Entities

	public Entity CreateEntity(bool activate = true)
		=> EntityManager.CreateEntity(Id, activate);

	public EntitySet GetEntitySet(ComponentSet componentSet)
		=> EntityManager.GetEntitySet(Id, componentSet);

	public EntityEnumerator ReadEntities(bool? active = true)
		=> EntityManager.ReadEntities(Id, active);

	// Components
	
	public bool Has<T>() where T : struct
		=> ComponentManager.HasComponent<T>(Id);

	public ref T Get<T>() where T : struct
		=> ref ComponentManager.GetComponent<T>(Id);

	public void Set<T>(T value) where T : struct
		=> ComponentManager.SetComponent(Id, value);

	// Messages

	public MessageEnumerator<T> ReadMessages<T>() where T : struct
		=> MessageManager.ReadMessages<T>(Id);

	public void SendMessage<T>(in T message) where T : struct
		=> MessageManager.SendMessage(Id, message);
}
