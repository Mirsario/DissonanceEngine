using Microsoft.CodeAnalysis;

namespace SourceGenerators.Subsystems;

internal static class DiagnosticRules
{
	public static readonly DiagnosticDescriptor MissingPartialOnSystemClass = new(
		"DE0101",
		"Missing 'partial' modifier on a GameSystem-deriving class",
		"GameSystem-deriving classes with subsystems in them must have the 'partial' modifier for source generators to function correctly",
		"ECS",
		DiagnosticSeverity.Error,
		true
	);

	public static readonly DiagnosticDescriptor MissingPartialOnSubsystemMethod = new(
		"DE0102",
		"Missing 'partial' modifier on subsystem-marked methods",
		"Subsystem-marked methods in GameSystem-deriving classes must have the 'partial' modifier for source generators to function correctly",
		"ECS",
		DiagnosticSeverity.Error,
		true
	);

	public static readonly DiagnosticDescriptor MultipleSubsystemAttributes = new(
		"DE0103",
		"Method contains multiple subsystem attributes",
		"A subsystem method may only contain one kind of subsystem attributes",
		"ECS",
		DiagnosticSeverity.Error,
		true
	);

	public static readonly DiagnosticDescriptor InvalidSubsystemParameter = new(
		"DE0104",
		"Invalid subsystem method parameter",
		"Unhandled subsystem method parameter: {0}",
		"ECS",
		DiagnosticSeverity.Error,
		true
	);

	public static readonly DiagnosticDescriptor UnknownSubsystemParameterSource = new(
		"DE0105",
		"Unknown subsystem method parameter source",
		"Cannot pick a source for subsystem method parameter '{0}', specify it using a correct From(Entity/World/Global) attribute",
		"ECS",
		DiagnosticSeverity.Error,
		true
	);

	public static readonly DiagnosticDescriptor InvalidMessageEntityParameter = new(
		"DE0106",
		"Message does not contain an entity field",
		"The argument for subsystem '{0}'s parameter '{1}' cannot come from an entity, because message '{2}' does not contain an 'Entity' Entity field",
		"ECS",
		DiagnosticSeverity.Error,
		true
	);
}
