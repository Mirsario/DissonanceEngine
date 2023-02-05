namespace Dissonance.Engine;

public readonly struct Tag
{
	public readonly uint Id;

	public string Name => Tags.GetName(Id);
	public bool IsValid => Id > 0;

	internal Tag(uint id)
	{
		Id = id;
	}

	public override int GetHashCode()
		=> (int)Id;

	public override string ToString()
		=> $"Tag - {Name} (#{Id})";

	public override bool Equals(object? obj)
		=> obj is Tag tag && tag.Id == Id;

	public static bool operator ==(Tag a, Tag b)
		=> a.Id == b.Id;

	public static bool operator !=(Tag a, Tag b)
		=> a.Id != b.Id;
}
