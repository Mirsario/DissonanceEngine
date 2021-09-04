using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Dissonance.Framework.Graphics;

namespace Dissonance.Engine.Graphics
{
	public class SubShader : IDisposable
	{
		private static readonly Regex RegexFSuffixA = new Regex(@"([^.]|^)([\d]+)f(?=[^\w])", RegexOptions.Compiled);
		private static readonly Regex RegexFSuffixB = new Regex(@"(\.)([\d]+)f(?=[^\w])", RegexOptions.Compiled);

		public readonly ShaderType Type;

		private IntPtr namePtr;

		public uint Id { get; private set; }
		public string Name { get; private set; }

		public SubShader(string name, ShaderType type)
		{
			Type = type;
			Id = GL.CreateShader(Type);
			Name = name;

			if (Name != null) {
				namePtr = Marshal.StringToHGlobalAnsi(Name);

				//TODO: Add a way to check if a function is implemented. Maybe make internal delegates accessible with properrties?
				try {
					GL.ObjectLabel(GLConstants.SHADER, Id, Name.Length, namePtr);
				}
				catch { }
			}
		}

		public void Dispose()
		{
			if (Id > 0) {
				GL.DeleteShader(Id);

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

			GL.ShaderSource(Id, code);
			GL.CompileShader(Id);

			string info = GL.GetShaderInfoLog(Id);

			if (!string.IsNullOrEmpty(info)) {
				Debug.Log($"Error compilling {Type} '{Name}':\r\n{info}\r\n\r\n{code}");

				return false;
			}

			if (Rendering.CheckGLErrors(throwException: false)) {
				throw new GraphicsException($"Unable to compile {Type} '{Name}'.");
			}

			return true;
		}
	}
}
