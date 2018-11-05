using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using ShaderTypeGL = OpenTK.Graphics.OpenGL.ShaderType;
using Newtonsoft.Json;


namespace GameEngine
{
	internal class ShaderUniforms
	{
		//Bad looking internal code,all for the sake of performance
		public static string[] names = {
			//Matrices
			"world",			"worldInverse",
			"worldView",		"worldViewInverse",
			"worldViewProj",	"worldViewProjInverse",
			"view",				"viewInverse",
			"proj",				"projInverse",
			//Camera
			"nearClip",			"farClip",
			"screenWidth",		"screenHeight",
			"cameraPosition"
		};
		public const int
			//Matrices
			world = 0,			worldInverse = 1,
			worldView = 2,		worldViewInverse = 3,
			worldViewProj = 4,	worldViewProjInverse = 5,
			view = 6,			viewInverse = 7,
			proj = 8,			projInverse = 9,
			//Camera
			nearClip = 10,		farClip = 11,
			screenWidth = 12,	screenHeight = 13,
			cameraPosition = 14,
			//
			count = 15;
	}
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
		internal bool[] hasDefaultUniform = new bool[ShaderUniforms.count];
		internal int[] defaultUniformIndex = new int[ShaderUniforms.count];
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
					int indexOf = Array.IndexOf(ShaderUniforms.names,uniform.name);
					if(indexOf>=0) {
						hasDefaultUniform[indexOf] = true;
						defaultUniformIndex[indexOf] = location;
					}
				}
			}
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
			if(hasDefaultUniform[ShaderUniforms.nearClip]) { GL.Uniform1(defaultUniformIndex[ShaderUniforms.nearClip],camera.nearClip); }
			if(hasDefaultUniform[ShaderUniforms.farClip]) { GL.Uniform1(defaultUniformIndex[ShaderUniforms.farClip],camera.farClip); }
			if(hasDefaultUniform[ShaderUniforms.screenWidth]) { GL.Uniform1(defaultUniformIndex[ShaderUniforms.screenWidth],Graphics.ScreenWidth); }
			if(hasDefaultUniform[ShaderUniforms.screenHeight]) { GL.Uniform1(defaultUniformIndex[ShaderUniforms.screenHeight],Graphics.ScreenHeight); }
			if(hasDefaultUniform[ShaderUniforms.cameraPosition]) { GL.Uniform3(defaultUniformIndex[ShaderUniforms.cameraPosition],cameraPos); }

			#region World
			//bool needsWorld;
			if(hasDefaultUniform[ShaderUniforms.world] || hasDefaultUniform[ShaderUniforms.worldInverse] || hasDefaultUniform[ShaderUniforms.worldView] || hasDefaultUniform[ShaderUniforms.worldViewInverse] || hasDefaultUniform[ShaderUniforms.worldViewProj] || hasDefaultUniform[ShaderUniforms.worldViewProjInverse]) {
				//Check
				if(!uniformComputed[ShaderUniforms.world]) { world = transform.WorldMatrix; uniformComputed[ShaderUniforms.world] = true; }

				if(hasDefaultUniform[ShaderUniforms.world]) {
					//Assign
					UniformMatrix4(defaultUniformIndex[ShaderUniforms.world],ref world);
				}
				if(hasDefaultUniform[ShaderUniforms.worldInverse]) {
					//Check, Assign
					if(!uniformComputed[ShaderUniforms.worldInverse]) { worldInverse = world.Inverted; uniformComputed[ShaderUniforms.worldInverse] = true; }
					UniformMatrix4(defaultUniformIndex[ShaderUniforms.worldInverse],ref worldInverse);
				}

				#region WorldView
				if(hasDefaultUniform[ShaderUniforms.worldView] || hasDefaultUniform[ShaderUniforms.worldViewInverse] || hasDefaultUniform[ShaderUniforms.worldViewProj] || hasDefaultUniform[ShaderUniforms.worldViewProjInverse]) {
					//Check
					if(!uniformComputed[ShaderUniforms.worldView]) { worldView = world*camera.matrix_view; uniformComputed[ShaderUniforms.worldView] = true; }

					if(hasDefaultUniform[ShaderUniforms.worldView]) {
						//Assign
						UniformMatrix4(defaultUniformIndex[ShaderUniforms.worldView],ref worldView);
					}
					if(hasDefaultUniform[ShaderUniforms.worldViewInverse]) {
						//Check, Assign
						if(!uniformComputed[ShaderUniforms.worldViewInverse]) { worldViewInverse = worldView.Inverted; uniformComputed[ShaderUniforms.worldViewInverse] = true; }
						UniformMatrix4(defaultUniformIndex[ShaderUniforms.worldViewInverse],ref worldViewInverse);
					}

					#region WorldViewProj
					if(hasDefaultUniform[ShaderUniforms.worldViewProj] || hasDefaultUniform[ShaderUniforms.worldViewProjInverse]) {
						//Check
						if(!uniformComputed[ShaderUniforms.worldViewProj]) { worldViewProj = worldView*camera.matrix_proj; uniformComputed[ShaderUniforms.worldViewProj] = true; }

						if(hasDefaultUniform[ShaderUniforms.worldViewProj]) {
							//Assign
							UniformMatrix4(defaultUniformIndex[ShaderUniforms.worldViewProj],ref worldViewProj);
						}
						if(hasDefaultUniform[ShaderUniforms.worldViewProjInverse]) {
							//Check, Assign
							if(!uniformComputed[ShaderUniforms.worldViewProjInverse]) { worldViewProjInverse = worldViewProj.Inverted; uniformComputed[ShaderUniforms.worldViewProjInverse] = true; }
							UniformMatrix4(defaultUniformIndex[ShaderUniforms.worldViewProjInverse],ref worldViewProjInverse);
						}
					}
					#endregion
				}
				#endregion
			}
			#endregion
			#region View
			if(hasDefaultUniform[ShaderUniforms.view])			{ UniformMatrix4(defaultUniformIndex[ShaderUniforms.view],			ref view); }
			if(hasDefaultUniform[ShaderUniforms.viewInverse])	{ UniformMatrix4(defaultUniformIndex[ShaderUniforms.viewInverse],	ref viewInverse); }
			#endregion
			#region Proj
			if(hasDefaultUniform[ShaderUniforms.proj])			{ UniformMatrix4(defaultUniformIndex[ShaderUniforms.proj],			ref proj); }
			if(hasDefaultUniform[ShaderUniforms.projInverse])	{ UniformMatrix4(defaultUniformIndex[ShaderUniforms.projInverse],	ref projInverse); }
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
			if(hasDefaultUniform[ShaderUniforms.nearClip]) { GL.Uniform1(defaultUniformIndex[ShaderUniforms.nearClip],camera.nearClip); }
			if(hasDefaultUniform[ShaderUniforms.farClip]) { GL.Uniform1(defaultUniformIndex[ShaderUniforms.farClip],camera.farClip); }
			if(hasDefaultUniform[ShaderUniforms.screenWidth]) { GL.Uniform1(defaultUniformIndex[ShaderUniforms.screenWidth],Graphics.ScreenWidth); }
			if(hasDefaultUniform[ShaderUniforms.screenHeight]) { GL.Uniform1(defaultUniformIndex[ShaderUniforms.screenHeight],Graphics.ScreenHeight); }
			if(hasDefaultUniform[ShaderUniforms.cameraPosition]) { GL.Uniform3(defaultUniformIndex[ShaderUniforms.cameraPosition],cameraPos); }

			#region World
			//bool needsWorld;
			if(hasDefaultUniform[ShaderUniforms.world] || hasDefaultUniform[ShaderUniforms.worldInverse] || hasDefaultUniform[ShaderUniforms.worldView] || hasDefaultUniform[ShaderUniforms.worldViewInverse] || hasDefaultUniform[ShaderUniforms.worldViewProj] || hasDefaultUniform[ShaderUniforms.worldViewProjInverse]) {
				if(!dontCalculateWorld) {
					world = transform.WorldMatrix;
				}

				if(hasDefaultUniform[ShaderUniforms.world]) {
					UniformMatrix4(defaultUniformIndex[ShaderUniforms.world],ref world);
				}
				if(hasDefaultUniform[ShaderUniforms.worldInverse]) {
					worldInverse = world.Inverted;
					UniformMatrix4(defaultUniformIndex[ShaderUniforms.worldInverse],ref worldInverse);
				}

				#region WorldView
				if(hasDefaultUniform[ShaderUniforms.worldView] || hasDefaultUniform[ShaderUniforms.worldViewInverse] || hasDefaultUniform[ShaderUniforms.worldViewProj] || hasDefaultUniform[ShaderUniforms.worldViewProjInverse]) {
					worldView = world*camera.matrix_view;

					if(hasDefaultUniform[ShaderUniforms.worldView]) {
						UniformMatrix4(defaultUniformIndex[ShaderUniforms.worldView],ref worldView);
					}
					if(hasDefaultUniform[ShaderUniforms.worldViewInverse]) {
						worldViewInverse = worldView.Inverted;
						UniformMatrix4(defaultUniformIndex[ShaderUniforms.worldViewInverse],ref worldViewInverse);
					}

					#region WorldViewProj
					if(hasDefaultUniform[ShaderUniforms.worldViewProj] || hasDefaultUniform[ShaderUniforms.worldViewProjInverse]) {
						worldViewProj = worldView*camera.matrix_proj;

						if(hasDefaultUniform[ShaderUniforms.worldViewProj]) {
							UniformMatrix4(defaultUniformIndex[ShaderUniforms.worldViewProj],ref worldViewProj);
						}
						if(hasDefaultUniform[ShaderUniforms.worldViewProjInverse]) {
							worldViewProjInverse = worldViewProj.Inverted;
							UniformMatrix4(defaultUniformIndex[ShaderUniforms.worldViewProjInverse],ref worldViewProjInverse);
						}
					}
					#endregion
				}
				#endregion
			}
			#endregion
			#region View
			if(hasDefaultUniform[ShaderUniforms.view])			{ UniformMatrix4(defaultUniformIndex[ShaderUniforms.view],			ref view); }
			if(hasDefaultUniform[ShaderUniforms.viewInverse])	{ UniformMatrix4(defaultUniformIndex[ShaderUniforms.viewInverse],	ref viewInverse); }
			#endregion
			#region Proj
			if(hasDefaultUniform[ShaderUniforms.proj])			{ UniformMatrix4(defaultUniformIndex[ShaderUniforms.proj],			ref proj); }
			if(hasDefaultUniform[ShaderUniforms.projInverse])	{ UniformMatrix4(defaultUniformIndex[ShaderUniforms.projInverse],	ref projInverse); }
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
			Debug.Log("Compiling shader "+name);
			
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

			Graphics.CheckGLErrors();
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
				//\bworld\b
				var matches = Regex.Matches(code,@"uniform\s+?(\S+?)\s+?(\S+?)\s*?\;");
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

			GL.BindAttribLocation(shader.program,(int)AttributeId.Vertex,		"vertex");
			GL.BindAttribLocation(shader.program,(int)AttributeId.Normal,		"normal");
			GL.BindAttribLocation(shader.program,(int)AttributeId.Tangent,		"tangent");
			GL.BindAttribLocation(shader.program,(int)AttributeId.Color,		"color");
			GL.BindAttribLocation(shader.program,(int)AttributeId.BoneIndices,	"boneIndices");
			GL.BindAttribLocation(shader.program,(int)AttributeId.BoneWeights,	"boneWeights");
			GL.BindAttribLocation(shader.program,(int)AttributeId.Uv0,			"uv0");
			Graphics.CheckGLErrors();
			
			GL.LinkProgram(shader.program);
			Graphics.CheckGLErrors();
			shader.Init();
			Graphics.CheckGLErrors();
			return shader;
		}
		internal void CompileShader(ShaderType type,string code,string shaderName = "")
		{
			int shader = GL.CreateShader((ShaderTypeGL)type);
			GL.ShaderSource(shader,code);
			GL.CompileShader(shader);
			string info = GL.GetShaderInfoLog(shader);
			if(!string.IsNullOrEmpty(info)) {
				Debug.Log("Error compilling shader: \n"+info+"\n\n"+code);
				//throw new Exception("Error compiling shader"+(!string.IsNullOrEmpty(shaderName) ? " "+shaderName : "")+": \t"+info);
				GL.DeleteShader(shader);
				if(type==ShaderType.Vertex) {
					GL.AttachShader(program,ErrorShader.vertexShader);
				}else if(type==ShaderType.Fragment) {
					GL.AttachShader(program,ErrorShader.fragmentShader);
				}
			}else{
				GL.AttachShader(program,shader);
			}
			if(Graphics.CheckGLErrors(false)) {
				throw new GraphicsException($"Unable to compile {type} shader '{shaderName}'");
			}
		}
		internal static unsafe void UniformMatrix4(int location,ref Matrix4x4 matrix,bool transpose = false)
		{
			fixed(float*matrix_ptr=&matrix.m00) {
				GL.UniformMatrix4(location,1,transpose,matrix_ptr);
			}
			/*GL.UniformMatrix4(location,1,transpose,new float[] {
				matrix.m00,matrix.m01,matrix.m02,matrix.m03,
				matrix.m10,matrix.m11,matrix.m12,matrix.m13,
				matrix.m20,matrix.m21,matrix.m22,matrix.m23,
				matrix.m30,matrix.m31,matrix.m32,matrix.m33,
			});*/
		}
	}
}