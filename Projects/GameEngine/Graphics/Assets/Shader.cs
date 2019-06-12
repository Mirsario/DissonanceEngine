using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using ShaderTypeGL = OpenTK.Graphics.OpenGL.ShaderType;
using DSU = GameEngine.Graphics.DefaultShaderUniforms;

namespace GameEngine.Graphics
{
	internal class ShaderUniform
	{
		public string name;
		public string type;
		public int location;

		public ShaderUniform(string name,string type,int location)
		{
			this.name = name;
			this.type = type;
			this.location = location;
		}
	}
	
	public enum ShaderType
	{
		Vertex = 35633,
		Fragment = 35632,
		Geometry = 36313
	}
	public enum PolygonMode
	{
		Point = 6912,
		Line = 6913,
		Fill = 6914
	}
	public enum ShaderCombination
	{
		Vertex,
		Fragment,
		Geometry,
		VertexFragment,
		VertexGeometry,
		FragmentGeometry,
		VertexFragmentGeometry
	}
	public class Shader : Asset<Shader>
	{
		//TODO: Implement OnDispose
		internal static Shader activeShader;

		//TODO: Initialize these after Graphics.Init();
		internal static Dictionary<string,Shader> shadersByName = new Dictionary<string,Shader>(StringComparer.OrdinalIgnoreCase);
		internal static List<Shader> shaders = new List<Shader>();
		internal Dictionary<string,ShaderUniform> uniforms = new Dictionary<string,ShaderUniform>();
		internal bool[] hasDefaultUniform = new bool[DSU.Count];
		internal int[] defaultUniformIndex = new int[DSU.Count];
		internal List<Material> materialAttachments = new List<Material>();

		public readonly string name;
		public int program;
		public int vertexShader;
		public int fragmentShader;
		public int geometryShader;
		public string[] defines;
		public CullMode cullMode = CullMode.Front;
		public PolygonMode polygonMode = PolygonMode.Fill;
		
		private static Shader _errorShader;
		public static Shader ErrorShader => _errorShader ?? (_errorShader = Resources.Find<Shader>("Error"));

		public override string GetAssetName() => name;

		private Shader(string name)
		{
			this.name = name;
		}
		private void Init()
		{
			if(shadersByName.TryGetValue(name,out var oldShader) && oldShader!=null) {
				oldShader.Dispose();
				shaders.Remove(oldShader);
			}
			shadersByName[name] = this;
			shaders.Add(this);

			//Set uniform locations
			var keys = uniforms.Keys.ToArray();
			for(int i=0;i<keys.Length;i++) {
				string key = keys[i];
				var uniform = uniforms[key];
				if(uniform.location<0) {
					int location = GL.GetUniformLocation(program,uniform.name);
					uniform.location = location;
					//Optimization for engine's uniforms
					int indexOf = Array.IndexOf(DSU.names,uniform.name);
					if(indexOf>=0) {
						hasDefaultUniform[indexOf] = true;
						defaultUniformIndex[indexOf] = location;
					}
				}
			}
			Rendering.CheckGLErrors();
		}
		internal void SetupUniformsCached(ref Camera camera,ref Vector3 cameraPos,Transform transform,bool[] uniformComputed,
			ref Matrix4x4 world,			ref Matrix4x4 worldInverse,
			ref Matrix4x4 worldView,		ref Matrix4x4 worldViewInverse,
			ref Matrix4x4 worldViewProj,	ref Matrix4x4 worldViewProjInverse,
			ref Matrix4x4 view,				ref Matrix4x4 viewInverse,
			ref Matrix4x4 proj,				ref Matrix4x4 projInverse
		){
			//Heavily optimized shitcode below
			//forgiv mi future me
			//no
			if(hasDefaultUniform[DSU.NearClip]) {
				GL.Uniform1(defaultUniformIndex[DSU.NearClip],camera.nearClip);
			}
			if(hasDefaultUniform[DSU.FarClip]) {
				GL.Uniform1(defaultUniformIndex[DSU.FarClip],camera.farClip);
			}
			if(hasDefaultUniform[DSU.ScreenWidth]) {
				GL.Uniform1(defaultUniformIndex[DSU.ScreenWidth],Screen.Width);
			}
			if(hasDefaultUniform[DSU.ScreenHeight]) {
				GL.Uniform1(defaultUniformIndex[DSU.ScreenHeight],Screen.Height);
			}
			if(hasDefaultUniform[DSU.ScreenResolution]) {
				GL.Uniform2(defaultUniformIndex[DSU.ScreenResolution],Screen.sizeFloat);
			}
			if(hasDefaultUniform[DSU.CameraPosition]) {
				GL.Uniform3(defaultUniformIndex[DSU.CameraPosition],cameraPos);
			}

			#region World
			//bool needsWorld;
			if(hasDefaultUniform[DSU.World] || hasDefaultUniform[DSU.WorldInverse] || hasDefaultUniform[DSU.WorldView] || hasDefaultUniform[DSU.WorldViewInverse] || hasDefaultUniform[DSU.WorldViewProj] || hasDefaultUniform[DSU.WorldViewProjInverse]) {
				//Check
				if(!uniformComputed[DSU.World]) { world = transform.WorldMatrix; uniformComputed[DSU.World] = true; }

				if(hasDefaultUniform[DSU.World]) {
					//Assign
					UniformMatrix4(defaultUniformIndex[DSU.World],ref world);
				}
				if(hasDefaultUniform[DSU.WorldInverse]) {
					//Check, Assign
					if(!uniformComputed[DSU.WorldInverse]) { worldInverse = world.Inverted; uniformComputed[DSU.WorldInverse] = true; }
					UniformMatrix4(defaultUniformIndex[DSU.WorldInverse],ref worldInverse);
				}

				#region WorldView
				if(hasDefaultUniform[DSU.WorldView] || hasDefaultUniform[DSU.WorldViewInverse] || hasDefaultUniform[DSU.WorldViewProj] || hasDefaultUniform[DSU.WorldViewProjInverse]) {
					//Check
					if(!uniformComputed[DSU.WorldView]) { worldView = world*camera.matrix_view; uniformComputed[DSU.WorldView] = true; }

					if(hasDefaultUniform[DSU.WorldView]) {
						//Assign
						UniformMatrix4(defaultUniformIndex[DSU.WorldView],ref worldView);
					}
					if(hasDefaultUniform[DSU.WorldViewInverse]) {
						//Check, Assign
						if(!uniformComputed[DSU.WorldViewInverse]) { worldViewInverse = worldView.Inverted; uniformComputed[DSU.WorldViewInverse] = true; }
						UniformMatrix4(defaultUniformIndex[DSU.WorldViewInverse],ref worldViewInverse);
					}

					#region WorldViewProj
					if(hasDefaultUniform[DSU.WorldViewProj] || hasDefaultUniform[DSU.WorldViewProjInverse]) {
						//Check
						if(!uniformComputed[DSU.WorldViewProj]) { worldViewProj = worldView*camera.matrix_proj; uniformComputed[DSU.WorldViewProj] = true; }

						if(hasDefaultUniform[DSU.WorldViewProj]) {
							//Assign
							UniformMatrix4(defaultUniformIndex[DSU.WorldViewProj],ref worldViewProj);
						}
						if(hasDefaultUniform[DSU.WorldViewProjInverse]) {
							//Check, Assign
							if(!uniformComputed[DSU.WorldViewProjInverse]) { worldViewProjInverse = worldViewProj.Inverted; uniformComputed[DSU.WorldViewProjInverse] = true; }
							UniformMatrix4(defaultUniformIndex[DSU.WorldViewProjInverse],ref worldViewProjInverse);
						}
					}
					#endregion
				}
				#endregion
			}
			#endregion
			#region View
			if(hasDefaultUniform[DSU.View])			{ UniformMatrix4(defaultUniformIndex[DSU.View],			ref view); }
			if(hasDefaultUniform[DSU.ViewInverse])	{ UniformMatrix4(defaultUniformIndex[DSU.ViewInverse],	ref viewInverse); }
			#endregion
			#region Proj
			if(hasDefaultUniform[DSU.Proj])			{ UniformMatrix4(defaultUniformIndex[DSU.Proj],			ref proj); }
			if(hasDefaultUniform[DSU.ProjInverse])	{ UniformMatrix4(defaultUniformIndex[DSU.ProjInverse],	ref projInverse); }
			#endregion
		}
		public void SetupUniforms(ref Camera camera,ref Vector3 cameraPos,Transform transform,
			ref Matrix4x4 world,			ref Matrix4x4 worldInverse,
			ref Matrix4x4 worldView,		ref Matrix4x4 worldViewInverse,
			ref Matrix4x4 worldViewProj,	ref Matrix4x4 worldViewProjInverse,
			ref Matrix4x4 view,				ref Matrix4x4 viewInverse,
			ref Matrix4x4 proj,				ref Matrix4x4 projInverse,
			bool dontCalculateWorld = false
		){
			//Heavily optimized shitcode below
			//forgiv mi future me
			//again, no.
			if(hasDefaultUniform[DSU.NearClip]) { GL.Uniform1(defaultUniformIndex[DSU.NearClip],camera.nearClip); }
			if(hasDefaultUniform[DSU.FarClip]) { GL.Uniform1(defaultUniformIndex[DSU.FarClip],camera.farClip); }
			if(hasDefaultUniform[DSU.ScreenWidth]) { GL.Uniform1(defaultUniformIndex[DSU.ScreenWidth],Screen.Width); }
			if(hasDefaultUniform[DSU.ScreenHeight]) { GL.Uniform1(defaultUniformIndex[DSU.ScreenHeight],Screen.Height); }
			if(hasDefaultUniform[DSU.CameraPosition]) { GL.Uniform3(defaultUniformIndex[DSU.CameraPosition],cameraPos); }

			#region World
			//bool needsWorld;
			if(hasDefaultUniform[DSU.World] || hasDefaultUniform[DSU.WorldInverse] || hasDefaultUniform[DSU.WorldView] || hasDefaultUniform[DSU.WorldViewInverse] || hasDefaultUniform[DSU.WorldViewProj] || hasDefaultUniform[DSU.WorldViewProjInverse]) {
				if(!dontCalculateWorld) {
					world = transform.WorldMatrix;
				}

				if(hasDefaultUniform[DSU.World]) {
					UniformMatrix4(defaultUniformIndex[DSU.World],ref world);
				}
				if(hasDefaultUniform[DSU.WorldInverse]) {
					worldInverse = world.Inverted;
					UniformMatrix4(defaultUniformIndex[DSU.WorldInverse],ref worldInverse);
				}

				#region WorldView
				if(hasDefaultUniform[DSU.WorldView] || hasDefaultUniform[DSU.WorldViewInverse] || hasDefaultUniform[DSU.WorldViewProj] || hasDefaultUniform[DSU.WorldViewProjInverse]) {
					worldView = world*camera.matrix_view;

					if(hasDefaultUniform[DSU.WorldView]) {
						UniformMatrix4(defaultUniformIndex[DSU.WorldView],ref worldView);
					}
					if(hasDefaultUniform[DSU.WorldViewInverse]) {
						worldViewInverse = worldView.Inverted;
						UniformMatrix4(defaultUniformIndex[DSU.WorldViewInverse],ref worldViewInverse);
					}

					#region WorldViewProj
					if(hasDefaultUniform[DSU.WorldViewProj] || hasDefaultUniform[DSU.WorldViewProjInverse]) {
						worldViewProj = worldView*camera.matrix_proj;

						if(hasDefaultUniform[DSU.WorldViewProj]) {
							UniformMatrix4(defaultUniformIndex[DSU.WorldViewProj],ref worldViewProj);
						}
						if(hasDefaultUniform[DSU.WorldViewProjInverse]) {
							worldViewProjInverse = worldViewProj.Inverted;
							UniformMatrix4(defaultUniformIndex[DSU.WorldViewProjInverse],ref worldViewProjInverse);
						}
					}
					#endregion
				}
				#endregion
			}
			#endregion
			#region View
			if(hasDefaultUniform[DSU.View])			{ UniformMatrix4(defaultUniformIndex[DSU.View],			ref view); }
			if(hasDefaultUniform[DSU.ViewInverse])	{ UniformMatrix4(defaultUniformIndex[DSU.ViewInverse],	ref viewInverse); }
			#endregion
			#region Proj
			if(hasDefaultUniform[DSU.Proj])			{ UniformMatrix4(defaultUniformIndex[DSU.Proj],			ref proj); }
			if(hasDefaultUniform[DSU.ProjInverse])	{ UniformMatrix4(defaultUniformIndex[DSU.ProjInverse],	ref projInverse); }
			#endregion
		}

		internal void MaterialDetach(Material material) => materialAttachments.Remove(material);
		internal void MaterialAttach(Material material) => materialAttachments.Add(material);
		internal static void SetShader(Shader shader)
		{
			if(shader!=null) {
				if(shader!=activeShader) {
					GL.UseProgram(shader.program);
					activeShader = shader;
				}
			}else if(activeShader!=null) {
				GL.UseProgram(0);
				activeShader = null;
			}
		}

		internal static Shader FromCode(string name,string vertexCode,string fragmentCode = "",string geometryCode = "",string[] defines = null)
		{
			//Debug.Log("Compiling shader "+name);
			
			if(defines!=null && defines.Length==0) {
				defines = null;
			}
			if(defines!=null) {
				string defString = "";
				void PrepareCode(ref string code)
				{
					if(string.IsNullOrEmpty(code)) {
						return;
					}
					int index = code.IndexOf("version",StringComparison.Ordinal);
					if(index>=0) {
						index = code.IndexOf("\n",index,StringComparison.Ordinal)+1;
						code = code.Insert(index,defString);
					}
				}

				for(int i=0;i<defines.Length;i++) {
					defString += "#define "+defines[i]+" \n";
				}
				PrepareCode(ref vertexCode);
				PrepareCode(ref fragmentCode);
				PrepareCode(ref geometryCode);
			}

			Rendering.CheckGLErrors();
			var shader = new Shader(name) {
				defines = defines,
				program = GL.CreateProgram()
			};

			void CompileCode(Shader s,ShaderType shaderType,string code)
			{
				if(string.IsNullOrEmpty(code)) {
					return;
				}
				s.CompileShader(shaderType,code,name);
				
				var matches = RegexCache.shaderUniforms.Matches(code);
				foreach(Match match in matches) {
					if(!match.Success) {
						continue;
					}
					string uniformType = match.Groups[1].Value;
					string uniformName = match.Groups[2].Value;
					s.uniforms[uniformName] = new ShaderUniform(uniformName,uniformType,-1);//location will be set in init
				}
			}
			CompileCode(shader,ShaderType.Vertex,vertexCode);
			CompileCode(shader,ShaderType.Fragment,fragmentCode);
			CompileCode(shader,ShaderType.Geometry,geometryCode);

			GL.BindAttribLocation(shader.program,(int)AttributeId.Vertex,"vertex");
			GL.BindAttribLocation(shader.program,(int)AttributeId.Normal,"normal");
			GL.BindAttribLocation(shader.program,(int)AttributeId.Tangent,"tangent");
			GL.BindAttribLocation(shader.program,(int)AttributeId.Color,"color");
			GL.BindAttribLocation(shader.program,(int)AttributeId.BoneIndices,"boneIndices");
			GL.BindAttribLocation(shader.program,(int)AttributeId.BoneWeights,"boneWeights");
			GL.BindAttribLocation(shader.program,(int)AttributeId.Uv0,"uv0");
			Rendering.CheckGLErrors();
			
			GL.LinkProgram(shader.program);
			Rendering.CheckGLErrors();
			shader.Init();
			return shader;
		}
		internal void CompileShader(ShaderType type,string code,string shaderName = "")
		{
			code = code.Trim();
			code = RegexCache.shaderFSuffixA.Replace(code,@"$1$2.0"); //Some broken Nvidia drivers don't support 'f' suffix, even though it was added in GLSL 1.2 decades ago. Zoinks.
			code = RegexCache.shaderFSuffixB.Replace(code,@"$1$2");
			
			int shader = GL.CreateShader((ShaderTypeGL)type);
			GL.ShaderSource(shader,code);
			GL.CompileShader(shader);

			string info = GL.GetShaderInfoLog(shader);
			if(!string.IsNullOrEmpty(info)) {
				Debug.Log($"Error compilling shader: \n{info}\n\n{code}");
				GL.DeleteShader(shader);

				if(type==ShaderType.Vertex) {
					GL.AttachShader(program,ErrorShader.vertexShader);
				}else if(type==ShaderType.Fragment) {
					GL.AttachShader(program,ErrorShader.fragmentShader);
				}
			}else{
				GL.AttachShader(program,shader);
			}

			if(Rendering.CheckGLErrors(false)) {
				throw new GraphicsException($"Unable to compile {type} shader '{shaderName}'");
			}
		}
		internal static unsafe void UniformMatrix4(int location,ref Matrix4x4 matrix,bool transpose = false)
		{
			fixed(float* matrix_ptr = &matrix.m00) {
				GL.UniformMatrix4(location,1,transpose,matrix_ptr);
			}
		}
	}
}