using System;
using Silk.NET.OpenGL;
using static Dissonance.Engine.Graphics.OpenGLApi;

namespace Dissonance.Engine.Graphics;

unsafe partial class Rendering
{
	private static void TryEnablingDebugging()
	{
		//TODO: Add a way to avoid trycatches like this.
		try {
			OpenGL.Enable(EnableCap.DebugOutput);
			OpenGL.Enable(EnableCap.DebugOutputSynchronous);

			CheckGLErrors("After attempting to enable debugging.");

			debugCallback = DebugCallback;

			OpenGL.DebugMessageCallback(debugCallback, IntPtr.Zero);

			Debug.Log("Activated OpenGL Debugging.");
		}
		catch {
			Debug.Log("Couldn't enable GL debugging.");
		}
	}

	private static void DebugCallback(GLEnum glSource, GLEnum glType, int id, GLEnum glSeverity, int length, nint message, nint userParameter)
	{
		var source = (DebugSource)glSource;
		var type = (DebugType)glType;
		var severity = (DebugSeverity)glSeverity;

		if (severity == DebugSeverity.DebugSeverityNotification) {
			return;
		}

		string sourceName = source switch {
			DebugSource.DebugSourceApi => "API",
			DebugSource.DebugSourceWindowSystem => "WINDOW SYSTEM",
			DebugSource.DebugSourceShaderCompiler => "SHADER COMPILER",
			DebugSource.DebugSourceThirdParty => "THIRD PARTY",
			DebugSource.DebugSourceApplication => "APPLICATION",
			DebugSource.DebugSourceOther => "OTHER",
			_ => "UNKNOWN",
		};
		string typeName = type switch {
			DebugType.DebugTypeError => "ERROR",
			DebugType.DebugTypeDeprecatedBehavior => "DEPRECATED BEHAVIOR",
			DebugType.DebugTypeUndefinedBehavior => "UDEFINED BEHAVIOR",
			DebugType.DebugTypePortability => "PORTABILITY",
			DebugType.DebugTypePerformance => "PERFORMANCE",
			DebugType.DebugTypeOther => "OTHER",
			DebugType.DebugTypeMarker => "MARKER",
			_ => "UNKNOWN",
		};
		string severityName = severity switch {
			DebugSeverity.DebugSeverityHigh => "HIGH",
			DebugSeverity.DebugSeverityMedium => "MEDIUM",
			DebugSeverity.DebugSeverityLow => "LOW",
			DebugSeverity.DebugSeverityNotification => "NOTIFICATION",
			_ => "UNKNOWN",
		};

		if (severity == DebugSeverity.DebugSeverityHigh) {
			throw new GraphicsException($"GL Error - ID: {id}; Type: {typeName}; Severity: {severityName}; From: {sourceName};\r\n{message}");
		}

		Debug.Log($"GL Debug - ID: {id}; Type: {typeName}; Severity: {severityName}; From: {sourceName};\r\n{message}");
	}
}
