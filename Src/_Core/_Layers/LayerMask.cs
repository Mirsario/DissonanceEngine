using System.Linq;

namespace Dissonance.Engine;

public readonly struct LayerMask
{
	public static readonly LayerMask All = new(ulong.MaxValue);
	public static readonly LayerMask None = new(ulong.MinValue);

	private readonly ulong Value;

	internal LayerMask(ulong value)
	{
		Value = value;
	}

	public override int GetHashCode()
		=> Value.GetHashCode();

	public override bool Equals(object obj)
	{
		if (obj is not LayerMask layerMask) {
			return false;
		}

		return layerMask.Value == Value;
	}

	public override string ToString()
	{
		ulong value = Value;
		char[] chars = Enumerable.Range(0, sizeof(ulong) * 8)
			.Select(i => (value & (1ul << i)) == 0 ? '0' : '1')
			.ToArray();

		return $"{Value} - {new string(chars)}";
	}

	public static LayerMask operator ~(LayerMask a) => new(~a.Value);
	public static LayerMask operator &(LayerMask a, LayerMask b) => new(a.Value & b.Value);
	public static LayerMask operator |(LayerMask a, LayerMask b) => new(a.Value | b.Value);
	public static LayerMask operator ^(LayerMask a, LayerMask b) => new(a.Value ^ b.Value);

	public static bool operator ==(LayerMask a, LayerMask b) => a.Value == b.Value;
	public static bool operator !=(LayerMask a, LayerMask b) => a.Value != b.Value;
}
