using System;

namespace SourceGenerators;

[Flags]
public enum MemberFlag
{
	// Types
	Class = 1,
	Delegate = 1 << 1,
	Field = 1 << 2,
	Property = 1 << 3,
	Event = 1 << 4,
	Indexer = 1 << 5,
	Constructor = 1 << 6,
	Destructor = 1 << 7,
	Method = 1 << 8,
	// Protection Modifiers
	Public = 1 << 9,
	Protected = 1 << 10,
	Internal = 1 << 11,
	Private = 1 << 12,
	// Other modifiers
	NonStatic = 1 << 13,
	ReadOnly = 1 << 14,
	Abstract = 1 << 15,
	Virtual = 1 << 16,
	Override = 1 << 17,
	Explicit = 1 << 18,
	Implicit = 1 << 19,
}
