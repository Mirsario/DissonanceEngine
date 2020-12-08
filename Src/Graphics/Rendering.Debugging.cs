using Dissonance.Framework.Graphics;
using System;

namespace Dissonance.Engine.Graphics
{
	partial class Rendering
	{
		private static void TryEnablingDebugging()
		{
			//TODO: Add a way to avoid trycatches like this.
			try {
				GL.Enable(EnableCap.DebugOutput);
				GL.Enable(EnableCap.DebugOutputSynchronous);

				CheckGLErrors("After attempting to enable debugging.");

				debugCallback = (uint source, uint type, uint id, uint severity, int length, string message, IntPtr userParameter) => {
					if(severity == GLConstants.DEBUG_SEVERITY_NOTIFICATION) {
						return;
					}

					string sourceName = source switch
					{
						GLConstants.DEBUG_SOURCE_API => "API",
						GLConstants.DEBUG_SOURCE_WINDOW_SYSTEM => "WINDOW SYSTEM",
						GLConstants.DEBUG_SOURCE_SHADER_COMPILER => "SHADER COMPILER",
						GLConstants.DEBUG_SOURCE_THIRD_PARTY => "THIRD PARTY",
						GLConstants.DEBUG_SOURCE_APPLICATION => "APPLICATION",
						GLConstants.DEBUG_SOURCE_OTHER => "OTHER",
						_ => "UNKNOWN",
					};
					string typeName = type switch
					{
						GLConstants.DEBUG_TYPE_ERROR => "ERROR",
						GLConstants.DEBUG_TYPE_DEPRECATED_BEHAVIOR => "DEPRECATED BEHAVIOR",
						GLConstants.DEBUG_TYPE_UNDEFINED_BEHAVIOR => "UDEFINED BEHAVIOR",
						GLConstants.DEBUG_TYPE_PORTABILITY => "PORTABILITY",
						GLConstants.DEBUG_TYPE_PERFORMANCE => "PERFORMANCE",
						GLConstants.DEBUG_TYPE_OTHER => "OTHER",
						GLConstants.DEBUG_TYPE_MARKER => "MARKER",
						_ => "UNKNOWN",
					};
					string severityName = severity switch
					{
						GLConstants.DEBUG_SEVERITY_HIGH => "HIGH",
						GLConstants.DEBUG_SEVERITY_MEDIUM => "MEDIUM",
						GLConstants.DEBUG_SEVERITY_LOW => "LOW",
						GLConstants.DEBUG_SEVERITY_NOTIFICATION => "NOTIFICATION",
						_ => "UNKNOWN",
					};

					if(severity == GLConstants.DEBUG_SEVERITY_HIGH) {
						throw new GraphicsException($"GL Error - ID: {id}; Type: {typeName}; Severity: {severityName}; From: {sourceName};\r\n{message}");
					}

					Debug.Log($"GL Debug - ID: {id}; Type: {typeName}; Severity: {severityName}; From: {sourceName};\r\n{message}");
				};

				GL.DebugMessageCallback(debugCallback, IntPtr.Zero);

				Debug.Log("Activated OpenGL Debugging.");
			}
			catch {
				Debug.Log("Couldn't enable GL debugging.");
			}
		}
	}
}
