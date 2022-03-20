using System;
using System.Collections.Generic;
using Dissonance.Engine.IO;
using Silk.NET.OpenGL;
using static Dissonance.Engine.Graphics.OpenGLApi;
using DSU = Dissonance.Engine.Graphics.DefaultShaderUniforms;

namespace Dissonance.Engine.Graphics
{
	//TODO: Should this be renamed to ShaderProgram?
	//TODO: Initialize static fields after Graphics.Init();
	//TODO: Uniforms' code is quite terrible. Should really do OOP uniforms.
	public sealed partial class Shader : IDisposable
	{
		internal static Dictionary<string, Shader> shadersByName = new(StringComparer.OrdinalIgnoreCase);
		internal static List<Shader> shaders = new();

		private static Asset<Shader> errorShader;

		public static Asset<Shader> ErrorShader => errorShader ??= Assets.Get<Shader>("Error");
		public static Shader ActiveShader { get; private set; }

		internal Dictionary<string, ShaderUniform> uniforms;
		internal bool[] hasDefaultUniform = new bool[DSU.Count];
		internal int[] defaultUniformIndex = new int[DSU.Count];

		public int Priority { get; set; }
		public uint StencilMask { get; set; }
		public string[] Defines { get; set; }
		public CullMode CullMode { get; set; } = CullMode.Front;
		public PolygonMode PolygonMode { get; set; } = PolygonMode.Fill;
		public BlendingFactor BlendFactorSrc { get; set; } = BlendingFactor.One;
		public BlendingFactor BlendFactorDst { get; set; } = BlendingFactor.Zero;
		public DepthFunction DepthTest { get; set; } = DepthFunction.Less;
		public bool DepthWrite { get; set; } = true;
		public uint Id { get; private set; }
		public string Name { get; private set; }
		public SubShader VertexShader { get; private set; }
		public SubShader FragmentShader { get; private set; }
		public SubShader GeometryShader { get; private set; }

		private Shader(string name = null)
		{
			Name = name;
			Id = OpenGL.CreateProgram();

			if (Name != null) {
				//TODO: Add a way to check if a function is implemented. Maybe make internal delegates accessible with properrties?
				try {
					OpenGL.ObjectLabel(ObjectIdentifier.Program, Id, (uint)Name.Length, Name);
				}
				catch { }
			}
		}

		public override string ToString()
			=> Name;

		public void Dispose()
		{
			if (Id > 0) {
				OpenGL.DeleteProgram(Id);

				Id = 0;
			}

			VertexShader?.Dispose();
			FragmentShader?.Dispose();
			GeometryShader?.Dispose();

			VertexShader = null;
			FragmentShader = null;
			GeometryShader = null;
			Name = null;

			GC.SuppressFinalize(this);
		}

		public int GetUniformLocation(string uniformName)
			=> uniforms.TryGetValue(uniformName, out var uniform) ? uniform.Location : throw new ArgumentException($"Shader '{Name}' doesn't have uniform '{uniformName}'.");

		public bool TryGetUniformLocation(string uniformName, out int location)
		{
			if (uniforms.TryGetValue(uniformName, out var uniform)) {
				location = uniform.Location;

				return true;
			}

			location = -1;

			return false;
		}

		// SetupUniforms

		internal void SetupCommonUniforms()
		{
			if (hasDefaultUniform[DSU.ScreenWidth]) {
				OpenGL.Uniform1(defaultUniformIndex[DSU.ScreenWidth], Screen.Width);
			}

			if (hasDefaultUniform[DSU.ScreenHeight]) {
				OpenGL.Uniform1(defaultUniformIndex[DSU.ScreenHeight], Screen.Height);
			}

			if (hasDefaultUniform[DSU.ScreenResolution]) {
				OpenGL.Uniform2(defaultUniformIndex[DSU.ScreenResolution], Screen.Width, (float)Screen.Height);
			}

			if (hasDefaultUniform[DSU.Time]) {
				OpenGL.Uniform1(defaultUniformIndex[DSU.Time], Time.RenderGameTime);
			}

			if (hasDefaultUniform[DSU.AmbientColor]) {
				OpenGL.Uniform3(defaultUniformIndex[DSU.AmbientColor], Rendering.AmbientColor.X, Rendering.AmbientColor.Y, Rendering.AmbientColor.Z);
			}
		}

		internal void SetupCameraUniforms(float nearClip, float farClip, Vector3 cameraPos)
		{
			if (hasDefaultUniform[DSU.NearClip]) {
				OpenGL.Uniform1(defaultUniformIndex[DSU.NearClip], nearClip);
			}

			if (hasDefaultUniform[DSU.FarClip]) {
				OpenGL.Uniform1(defaultUniformIndex[DSU.FarClip], farClip);
			}

			if (hasDefaultUniform[DSU.CameraPosition]) {
				OpenGL.Uniform3(defaultUniformIndex[DSU.CameraPosition], cameraPos.X, cameraPos.Y, cameraPos.Z);
			}

			if (hasDefaultUniform[DSU.CameraDirection]) {
				//TODO:
				// var forward = camera.Transform.Forward;
				//
				// OpenGL.Api.Uniform3(defaultUniformIndex[DSU.CameraDirection], forward.x, forward.y, forward.z);
			}
		}

		internal void SetupMatrixUniforms(
			in Matrix4x4 world, ref Matrix4x4 worldInverse,
			ref Matrix4x4 worldView, ref Matrix4x4 worldViewInverse,
			ref Matrix4x4 worldViewProj, ref Matrix4x4 worldViewProjInverse,
			in Matrix4x4 view, in Matrix4x4 viewInverse,
			in Matrix4x4 proj, in Matrix4x4 projInverse
		)
		{
			//TODO: The following is the most horrible part of this codebase. To be rewritten.

			#region World

			if (hasDefaultUniform[DSU.World] || hasDefaultUniform[DSU.WorldInverse] || hasDefaultUniform[DSU.WorldView] || hasDefaultUniform[DSU.WorldViewInverse] || hasDefaultUniform[DSU.WorldViewProj] || hasDefaultUniform[DSU.WorldViewProjInverse]) {
				if (hasDefaultUniform[DSU.World]) {
					UniformMatrix4(defaultUniformIndex[DSU.World], in world);
				}

				if (hasDefaultUniform[DSU.WorldInverse]) {
					worldInverse = world.Inverted;
					UniformMatrix4(defaultUniformIndex[DSU.WorldInverse], in worldInverse);
				}

				#region WorldView

				if (hasDefaultUniform[DSU.WorldView] || hasDefaultUniform[DSU.WorldViewInverse] || hasDefaultUniform[DSU.WorldViewProj] || hasDefaultUniform[DSU.WorldViewProjInverse]) {
					worldView = world * view;

					if (hasDefaultUniform[DSU.WorldView]) {
						UniformMatrix4(defaultUniformIndex[DSU.WorldView], in worldView);
					}

					if (hasDefaultUniform[DSU.WorldViewInverse]) {
						worldViewInverse = worldView.Inverted;

						UniformMatrix4(defaultUniformIndex[DSU.WorldViewInverse], in worldViewInverse);
					}

					#region WorldViewProj

					if (hasDefaultUniform[DSU.WorldViewProj] || hasDefaultUniform[DSU.WorldViewProjInverse]) {
						worldViewProj = worldView * proj;

						if (hasDefaultUniform[DSU.WorldViewProj]) {
							UniformMatrix4(defaultUniformIndex[DSU.WorldViewProj], in worldViewProj);
						}

						if (hasDefaultUniform[DSU.WorldViewProjInverse]) {
							worldViewProjInverse = worldViewProj.Inverted;

							UniformMatrix4(defaultUniformIndex[DSU.WorldViewProjInverse], in worldViewProjInverse);
						}
					}

					#endregion
				}

				#endregion
			}

			#endregion

			#region View

			if (hasDefaultUniform[DSU.View]) {
				UniformMatrix4(defaultUniformIndex[DSU.View], in view);
			}

			if (hasDefaultUniform[DSU.ViewInverse]) {
				UniformMatrix4(defaultUniformIndex[DSU.ViewInverse], in viewInverse);
			}

			#endregion

			#region Proj

			if (hasDefaultUniform[DSU.Proj]) {
				UniformMatrix4(defaultUniformIndex[DSU.Proj], in proj);
			}

			if (hasDefaultUniform[DSU.ProjInverse]) {
				UniformMatrix4(defaultUniformIndex[DSU.ProjInverse], in projInverse);
			}

			#endregion
		}

		private void Init()
		{
			if (shadersByName.TryGetValue(Name, out var oldShader) && oldShader != null) {
				oldShader.Dispose();
				shaders.Remove(oldShader);
			}

			shadersByName[Name] = this;

			shaders.Add(this);

			// Set uniform locations
			uniforms = new Dictionary<string, ShaderUniform>();

			Rendering.CheckGLErrors($"Start of {nameof(Shader)}.{nameof(Init)}()");

			OpenGL.GetProgram(Id, ProgramPropertyARB.ActiveUniforms, out int uniformCount);

			const int MaxUniformNameLength = 32;

			for (int i = 0; i < uniformCount; i++) {
				OpenGL.GetActiveUniform(Id, (uint)i, MaxUniformNameLength, out uint length, out int size, out UniformType uniformType, out string uniformName);

				int location = OpenGL.GetUniformLocation(Id, uniformName); // Uniform location != uniform index, and that's pretty ridiculous and painful.

				uniforms.Add(uniformName, new ShaderUniform(uniformName, uniformType, location));

				// Optimization for engine's uniforms
				int indexOf = Array.IndexOf(DSU.names, uniformName);

				if (indexOf >= 0) {
					hasDefaultUniform[indexOf] = true;
					defaultUniformIndex[indexOf] = location;
				}
			}

			Rendering.CheckGLErrors($"End of {nameof(Shader)}.{nameof(Init)}()");
		}

		public static void SetShader(Shader shader)
		{
			if (shader != null) {
				OpenGL.UseProgram(shader.Id);
				ActiveShader = shader;

				Rendering.SetStencilMask(shader.StencilMask);
				Rendering.SetBlendFunc(shader.BlendFactorSrc, shader.BlendFactorDst);
			} else {
				OpenGL.UseProgram(0);

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

						OpenGL.AttachShader(shader.Id, subShader.Id);
					} else {
						subShader.Dispose();
					}
				}
			}

			TryCompileShader(shader, ShaderType.VertexShader, vertexCode, (shader, subShader) => shader.VertexShader = subShader);
			TryCompileShader(shader, ShaderType.FragmentShader, fragmentCode, (shader, subShader) => shader.FragmentShader = subShader);
			TryCompileShader(shader, ShaderType.GeometryShader, geometryCode, (shader, subShader) => shader.GeometryShader = subShader);

			for (int i = 0; i < VertexAttributes.Count; i++) {
				var attribute = VertexAttributes.GetInstance(i);

				OpenGL.BindAttribLocation(shader.Id, (uint)i, attribute.NameId);
			}

			OpenGL.LinkProgram(shader.Id);

			shader.Init();

			return shader;
		}

		internal static unsafe void UniformMatrix4(int location, in Matrix4x4 matrix, bool transpose = false)
		{
			fixed(float* matrix_ptr = &matrix.m00) {
				OpenGL.UniformMatrix4(location, 1, transpose, matrix_ptr);
			}
		}
	}
}
