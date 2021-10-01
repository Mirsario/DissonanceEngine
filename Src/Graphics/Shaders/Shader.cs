using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Dissonance.Engine.IO;
using Dissonance.Engine.Utilities;
using Dissonance.Framework.Graphics;

namespace Dissonance.Engine.Graphics
{
	//TODO: Initialize static fields after Rendering.Init();
	public partial class Shader : Asset
	{
		private struct UniformInfo
		{
			public readonly string Name;
			public readonly ActiveUniformType Type;
			public readonly int Location;

			public UniformInfo(string name, ActiveUniformType type, int location)
			{
				Name = name;
				Type = type;
				Location = location;
			}
		}

		internal static Dictionary<string, Shader> shadersByName = new(InternalUtils.DefaultStringComparer);
		internal static List<Shader> shaders = new();

		private static Shader errorShader;

		public static Shader ErrorShader => errorShader ??= Resources.Find<Shader>("Error");
		public static Shader ActiveShader { get; private set; }

		internal List<Material> materialAttachments = new();
		internal AutomaticUniformUsageInfo[] defaultUniforms = Array.Empty<AutomaticUniformUsageInfo>();

		private IntPtr namePtr;
		private UniformInfo[] uniforms;
		private Dictionary<string, int> uniformIdByName = new(InternalUtils.DefaultStringComparer);

		public int Priority { get; set; }
		public uint StencilMask { get; set; }
		public string[] Defines { get; set; }
		public CullMode CullMode { get; set; } = CullMode.Front;
		public PolygonMode PolygonMode { get; set; } = PolygonMode.Fill;
		public BlendingFactor BlendFactorSrc { get; set; } = BlendingFactor.One;
		public BlendingFactor BlendFactorDst { get; set; } = BlendingFactor.Zero;
		public uint Id { get; private set; }
		public string Name { get; private set; }
		public SubShader VertexShader { get; private set; }
		public SubShader FragmentShader { get; private set; }
		public SubShader GeometryShader { get; private set; }

		public override string AssetName => Name;

		private Shader(string name = null)
		{
			Name = name;
			Id = GL.CreateProgram();

			if (Name != null) {
				namePtr = Marshal.StringToHGlobalAnsi(Name);

				//TODO: Add a way to check if a function is implemented. Maybe make internal delegates accessible with properrties?
				try {
					GL.ObjectLabel(GLConstants.PROGRAM, Id, Name.Length, namePtr);
				}
				catch { }
			}
		}

		public override void Dispose()
		{
			base.Dispose();

			if (Id > 0) {
				GL.DeleteProgram(Id);

				Id = 0;
			}

			VertexShader?.Dispose();
			FragmentShader?.Dispose();
			GeometryShader?.Dispose();

			VertexShader = null;
			FragmentShader = null;
			GeometryShader = null;

			if (namePtr != IntPtr.Zero) {
				Marshal.FreeHGlobal(namePtr);

				namePtr = IntPtr.Zero;
			}

			Name = null;

			GC.SuppressFinalize(this);
		}

		public override string ToString()
			=> Name;

		public int GetUniformLocation(string uniformName)
			=> TryGetUniformLocation(uniformName, out int result) ? result : throw new ArgumentException($"Shader '{Name}' doesn't have uniform '{uniformName}'.");

		public bool TryGetUniformLocation(string uniformName, out int location)
		{
			if (uniformIdByName.TryGetValue(uniformName, out int uniformId)) {
				location = uniforms[uniformId].Location;

				return true;
			}

			location = -1;

			return false;
		}

		public void SetupDefaultUniforms(in Transform transform, in RenderViewData.RenderView viewData)
		{
			AutomaticUniformModule.Apply(this, in transform, in viewData);
		}

		internal void MaterialDetach(Material material)
			=> materialAttachments.Remove(material);

		internal void MaterialAttach(Material material)
			=> materialAttachments.Add(material);

		private void Init()
		{
			if (shadersByName.TryGetValue(Name, out var oldShader) && oldShader != null) {
				oldShader.Dispose();
				shaders.Remove(oldShader);
			}

			shadersByName[Name] = this;

			shaders.Add(this);

			// Set uniform locations
			Rendering.CheckGLErrors($"Start of {nameof(Shader)}.{nameof(Init)}()");

			GL.GetProgram(Id, GetProgramParameter.ActiveUniforms, out int uniformCount);

			uniforms = new UniformInfo[uniformCount];

			const int MaxUniformNameLength = 32;

			for (int i = 0; i < uniformCount; i++) {
				GL.GetActiveUniform(Id, (uint)i, MaxUniformNameLength, out int length, out int size, out ActiveUniformType uniformType, out string uniformName);

				int location = GL.GetUniformLocation(Id, uniformName); // Note: Uniform location is not the same thing as uniform index.

				uniforms[i] = new UniformInfo(uniformName, uniformType, location);
				uniformIdByName[uniformName] = i;
			}

			defaultUniforms = AutomaticUniformModule.GetPresentDefaultUniformInfo(this);

			Rendering.CheckGLErrors($"End of {nameof(Shader)}.{nameof(Init)}()");
		}

		public static void SetShader(Shader shader)
		{
			if (shader != null) {
				GL.UseProgram(shader.Id);
				ActiveShader = shader;

				Rendering.SetStencilMask(shader.StencilMask);
				Rendering.SetBlendFunc(shader.BlendFactorSrc, shader.BlendFactorDst);
			} else {
				GL.UseProgram(0);

				ActiveShader = null;
			}
		}

		public static Shader FromCode(string name, string vertexCode, string fragmentCode = "", string geometryCode = "", string[] defines = null)
		{
			if (defines != null && defines.Length == 0) {
				defines = null;
			}

			if (defines != null) {
				string defString = "";

				void PrepareCode(ref string code)
				{
					if (string.IsNullOrEmpty(code)) {
						return;
					}

					int index = code.IndexOf("version", StringComparison.Ordinal);

					if (index >= 0) {
						index = code.IndexOf("\n", index, StringComparison.Ordinal) + 1;
						code = code.Insert(index, defString);
					}
				}

				for (int i = 0; i < defines.Length; i++) {
					defString += $"#define {defines[i]}\r\n";
				}

				PrepareCode(ref vertexCode);
				PrepareCode(ref fragmentCode);
				PrepareCode(ref geometryCode);
			}

			var shader = new Shader(name) {
				Defines = defines
			};

			void TryCompileShader(Shader shader, ShaderType shaderType, string code, Action<Shader, SubShader> setter)
			{
				if (!string.IsNullOrEmpty(code)) {
					var subShader = new SubShader(name, shaderType);

					if (subShader.CompileShader(code)) {
						setter(shader, subShader);

						GL.AttachShader(shader.Id, subShader.Id);
					} else {
						subShader.Dispose();
					}
				}
			}

			TryCompileShader(shader, ShaderType.VertexShader, vertexCode, (shader, subShader) => shader.VertexShader = subShader);
			TryCompileShader(shader, ShaderType.FragmentShader, fragmentCode, (shader, subShader) => shader.FragmentShader = subShader);
			TryCompileShader(shader, ShaderType.GeometryShader, geometryCode, (shader, subShader) => shader.GeometryShader = subShader);

			for (int i = 0; i < CustomVertexAttribute.Count; i++) {
				var attribute = CustomVertexAttribute.GetInstance(i);

				GL.BindAttribLocation(shader.Id, (uint)i, attribute.NameId);
			}

			GL.LinkProgram(shader.Id);

			shader.Init();

			return shader;
		}

		internal static unsafe void UniformMatrix4(int location, in Matrix4x4 matrix, bool transpose = false)
		{
			fixed (float* matrix_ptr = &matrix.m00) {
				GL.UniformMatrix4(location, 1, transpose, matrix_ptr);
			}
		}
	}
}
