using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Silk.NET.OpenGL;
using static Dissonance.Engine.Graphics.OpenGLApi;

namespace Dissonance.Engine.Graphics;

public class SubShader : IDisposable
{
	private static readonly Regex RegexFSuffixA = new(@"([^.]|^)([\d]+)f(?=[^\w])", RegexOptions.Compiled);
	private static readonly Regex RegexFSuffixB = new(@"(\.)([\d]+)f(?=[^\w])", RegexOptions.Compiled);

	public readonly ShaderType Type;

	private IntPtr namePtr;

	public uint Id { get; private set; }
	public string Name { get; private set; }

	public SubShader(string name, ShaderType type)
	{
		Type = type;
		Id = OpenGL.CreateShader(Type);
		Name = name;

		if (Name != null) {
			namePtr = Marshal.StringToHGlobalAnsi(Name);

			//TODO: Add a way to check if a function is implemented. Maybe make internal delegates accessible with properrties?
			try {
				unsafe {
					OpenGL.ObjectLabel(ObjectIdentifier.Shader, Id, (uint)Name.Length, (byte*)namePtr);
				}
			}
			catch { }
		}
	}

	public void Dispose()
	{
		if (Id > 0) {
			OpenGL.DeleteShader(Id);

			Id = 0;
		}

		if (namePtr != IntPtr.Zero) {
			Marshal.FreeHGlobal(namePtr);

			namePtr = IntPtr.Zero;
		}

		GC.SuppressFinalize(this);
	}

	public bool CompileShader(string code)
	{
		code = code.Trim();

		// Some broken Nvidia drivers don't support 'f' suffix, even though it was added in GLSL 1.2 decades ago. Zoinks.
		code = RegexFSuffixB.Replace(code, @"$1$2");
		code = RegexFSuffixA.Replace(code, @"$1$2.0");

		OpenGL.ShaderSource(Id, code);
		OpenGL.CompileShader(Id);

		string info = OpenGL.GetShaderInfoLog(Id);

		if (!string.IsNullOrEmpty(info)) {
			Debug.Log($"Error compilling {Type} '{Name}':\r\n{info}\r\n\r\n{code}");

			return false;
		}

		if (Rendering.CheckGLErrors(throwException: false)) {
			throw new Exception($"Unable to compile {Type} '{Name}'.");
		}

		return true;
	}
}
