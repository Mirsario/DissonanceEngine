using Microsoft.CodeAnalysis;

namespace SourceGenerators;

internal static class DiagnosticRules
{
	// Errors
	
	public static readonly DiagnosticDescriptor MissingPartialOnSystemType = new(
		"DE0101",
		"Missing 'partial' modifier on a system-containing type",
		"Types that contain system methods must have the 'partial' modifier for source generators to function correctly",
		"ECS",
		DiagnosticSeverity.Error,
		true
	);

	public static readonly DiagnosticDescriptor MissingPartialOnSystemMethod = new(
		"DE0102",
		"Missing 'partial' modifier on system methods",
		"System methods must have the 'partial' modifier for source generators to function correctly",
		"ECS",
		DiagnosticSeverity.Error,
		true
	);

	public static readonly DiagnosticDescriptor MultipleSystemAttributes = new(
		"DE0103",
		"Method contains multiple system attributes",
		"A system method may only contain one kind of system attributes",
		"ECS",
		DiagnosticSeverity.Error,
		true
	);

	public static readonly DiagnosticDescriptor InvalidSystemParameter = new(
		"DE0104",
		"Invalid system method parameter",
		"Unhandled system method parameter: {0}",
		"ECS",
		DiagnosticSeverity.Error,
		true
	);

	public static readonly DiagnosticDescriptor UnknownSystemParameterSource = new(
		"DE0105",
		"Unknown system method parameter source",
		"Cannot pick a source for system method parameter '{0}', specify it using a correct From(Entity/World/Global) attribute",
		"ECS",
		DiagnosticSeverity.Error,
		true
	);

	public static readonly DiagnosticDescriptor InvalidMessageEntityParameter = new(
		"DE0106",
		"Message does not contain an entity field or property",
		"The argument for system '{0}'s parameter '{1}' cannot come from an entity, because message '{2}' does not contain an 'Entity Entity' field or property",
		"ECS",
		DiagnosticSeverity.Error,
		true
	);

	// Warnings

	public static readonly DiagnosticDescriptor SystemContainsNoCallbacks = new(
		"DE0201",
		"System method does not contain a callback attribute",
		"System '{0}'s method does not contain a callback attribute, so it may not get called",
		"ECS",
		DiagnosticSeverity.Warning,
		true
	);
}
