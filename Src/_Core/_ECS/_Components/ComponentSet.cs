using System;
using System.Text;
using EntryType = System.UInt64;

namespace Dissonance.Engine;

// Not as performant as would be preferred.
// Perhaps some sort of linked list of ~512 bits entries could be better?
public unsafe class ComponentSet
{
	private const int EntrySizeInBytes = sizeof(EntryType);
	private const int EntrySizeInBits = EntrySizeInBytes * 8;

	private static int ExpectedLength => ComponentManager.ComponentTypeCount;

	private EntryType[] bits;

	public int Length { /*readonly*/ get; private set; }

	public EntryType[] Bits => bits;

	public bool this[int index] {
		/*readonly*/
		get {
			if (index < EntrySizeInBits) {
				return (bits[0] & ((EntryType)1 << index)) != 0;
			}

			if (index >= Length) {
				return false;
			}

			return (bits[index / EntrySizeInBits] & ((EntryType)1 << (index % EntrySizeInBits))) != 0;
		}
		set {
			int entryIndex, bitIndex;

			if (index < EntrySizeInBits) {
				entryIndex = 0;
				bitIndex = index;
			} else {
				entryIndex = index / EntrySizeInBits;
				bitIndex = index % EntrySizeInBits;
			}

			if (index >= Length) {
				UpdateSize();

				if (index >= Length) {
					throw new IndexOutOfRangeException();
				}
			}

			if (value) {
				bits[entryIndex] |= ((EntryType)1 << bitIndex);
			} else {
				bits[entryIndex] &= ~((EntryType)1 << bitIndex);
			}
		}
	}

	public ComponentSet()
	{
		Length = ExpectedLength;

		int arrayLength = Math.Max(GetNeededLength(Length), 1);

		bits = new EntryType[arrayLength];
	}

	public /*readonly*/ override int GetHashCode()
	{
		int hash = (int)bits[0];

		for (int i = 1; i < bits.Length; i++) {
			hash ^= (int)bits[i];
		}

		return hash;
	}

	public /*readonly*/ override bool Equals(object obj)
		=> obj is ComponentSet set && set == this;

	public /*readonly*/ override string ToString()
	{
		char[] bitChars = new char[Length];
		var extraInfo = new StringBuilder();

		for (int i = 0; i < Length; i++) {
			bool bit = this[i];

			bitChars[i] = bit ? '1' : '0';

			if (bit) {
				extraInfo.Append($"[{i}]");
			}
		}

		return $"Length: {Length}, Bits: {new string(bitChars)}, Active: {extraInfo}";
	}

	public /*readonly*/ bool Equals(ComponentSet set)
		=> set == this;
	
	public /*readonly*/ bool Contains<T>() where T : struct
		=> this[ComponentManager.GetComponentId<T>()];

	public ComponentSet Include<T>() where T : struct
	{
		this[ComponentManager.GetComponentId<T>()] = true;

		return this;
	}

	public ComponentSet Exclude<T>() where T : struct
	{
		this[ComponentManager.GetComponentId<T>()] = false;

		return this;
	}

	public bool Matches(Entity entity)
	{
		int arrayLength = bits.Length;
		int componentId = 0;

		fixed (EntryType* ptr = bits) {
			for (int i = 0; i < arrayLength; i++) {
				EntryType entry = ptr[i];
				int bitsLength = i == arrayLength - 1 ? (i == 0 ? Length : Length - i * EntrySizeInBits) : EntrySizeInBits;

				for (int j = 0; j < bitsLength; j++) {
					if ((entry & ((EntryType)1 << j)) != 0) {
						if (!ComponentManager.HasComponent(componentId, entity.WorldId, entity.Id)) {
							return false;
						}
					}

					componentId++;
				}						
			}
		}

		return true;
	}

	private void UpdateSize()
	{
		int expectedLength = ExpectedLength;

		if (Length != expectedLength) {
			Length = expectedLength;

			int neededArrayLength = GetNeededLength(expectedLength);

			Array.Resize(ref bits, neededArrayLength);
		}
	}

	public static bool operator ==(ComponentSet a, ComponentSet b)
	{
		int aLength = a.bits.Length;
		int bLength = b.bits.Length;

		if (aLength != bLength) {
			a.UpdateSize();
			b.UpdateSize();

			aLength = a.bits.Length;
			bLength = b.bits.Length;

#if DEBUG
			System.Diagnostics.Debug.Assert(aLength == bLength);
#endif
		}

		fixed (EntryType* aPtr = a.bits) {
			fixed (EntryType* bPtr = b.bits) {
				if (aLength == 1) {
					return aPtr[0] == bPtr[0];
				}

				for (int i = 0; i < aLength; i++) {
					if (aPtr[i] != bPtr[i]) {
						return false;
					}
				}

				return true;
			}
		}
	}

	public static bool operator !=(ComponentSet a, ComponentSet b)
		=> !(a == b);

	private static int GetNeededLength(int baseBitLength)
		=> (int)MathF.Ceiling(baseBitLength / (float)EntrySizeInBits);
}
