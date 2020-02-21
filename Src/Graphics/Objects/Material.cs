using System;
using System.Linq;
using System.Collections.Generic;
using Dissonance.Framework.Graphics;
using Dissonance.Engine.Utils.Extensions;
using Dissonance.Engine.Utils.Internal;

namespace Dissonance.Engine.Graphics
{
	//TODO: Implement OnDispose
	public class Material : Asset, ICloneable
	{
		private static readonly List<Material> ById = new List<Material>();
		private static readonly Dictionary<string,Material> ByName = new Dictionary<string,Material>();

		public static Material defaultMat;

		private readonly Dictionary<string,(byte vecSize,float[] data)> UniformsFloat;
		private readonly List<KeyValuePair<string,Texture>> Textures;

		public readonly int Id;

		public string name;

		protected Shader shader;

		internal List<Renderer> rendererAttachments;

		public Shader Shader {
			get => shader;
			set {
				if(shader==value) {
					return;
				}

				shader?.MaterialDetach(this);

				shader = value;

				shader?.MaterialAttach(this);
			}
		}

		public override string AssetName => name;

		public Material(string name,Shader shader)
		{
			this.name = name;

			Id = InternalUtils.GenContentId(this,ById);

			Shader = shader;

			ByName[name] = this;

			UniformsFloat = new Dictionary<string,(byte,float[])>();
			Textures = new List<KeyValuePair<string,Texture>>();
		}

		public Material Clone()
		{
			var clone = new Material(name,Shader);

			UniformsFloat.CopyTo(clone.UniformsFloat);

			clone.Textures.AddRange(Textures);

			return clone;
		}

		internal void ApplyTextures(Shader shader)
		{
			ShaderUniform uniform;
			if(Textures.Count>0) {
				for(int i = 0;i<Textures.Count && i<32;i++) {
					string textureName = Textures[i].Key;
					var texture = Textures[i].Value;
					if(texture==null || !shader.uniforms.TryGetValue(textureName,out uniform)) {
						continue;
					}
					GL.ActiveTexture((TextureUnit)((int)TextureUnit.Texture0+i));
					GL.BindTexture(TextureTarget.Texture2D,texture.Id);
					GL.Uniform1(uniform.location,i);
				}
			}else if(shader.uniforms.TryGetValue("mainTex",out uniform)) {
				GL.ActiveTexture(TextureUnit.Texture0);
				GL.BindTexture(TextureTarget.Texture2D,Rendering.whiteTexture.Id);
				GL.Uniform1(uniform.location,0);
			}
		}
		internal void ApplyUniforms(Shader shader)
		{
			foreach(var pair in UniformsFloat) {
				(byte vecSize,var data) = pair.Value;
				int location = shader.uniforms[pair.Key].location;

				switch(vecSize) {
					case 1: GL.Uniform1(location,data.Length,  data); break;
					case 2: GL.Uniform2(location,data.Length/2,data); break;
					case 3: GL.Uniform3(location,data.Length/3,data); break;
					case 4: GL.Uniform4(location,data.Length/4,data); break;
				}
			}
		}

		private void CheckUniform(string name,string methodName)
		{
			Shader shader;
			if((shader = Shader)==null) {
				throw new Exception($"{methodName} cannot be used when material's Shader is null.");
			}
			if(!shader.uniforms.ContainsKey(name)) {
				throw new Exception($"Uniform {name} doesn't exist in shader ''{shader.Name}''.");
			}
		}

		public void SetFloat(string name,float value)
		{
			CheckUniform(name,"SetFloat");

			UniformsFloat[name] = (1,new[] { value });
		}
		public void SetFloat(string name,params float[] values)
		{
			CheckUniform(name,"SetFloat");

			UniformsFloat[name] = (1,values);
		}
		public void SetVector2(string name,Vector2 value)
		{
			CheckUniform(name,"SetVector2");

			UniformsFloat[name] = (2,new[] { value.x,value.y });
		}
		public void SetVector2(string name,params Vector2[] values)
		{
			CheckUniform(name,"SetVector2");

			const int VecLength = 2;

			int iBase;
			var data = new float[values.Length*VecLength];

			for(int i = 0;i<values.Length;i++) {
				iBase = i*VecLength;

				data[iBase] = values[i].x;
				data[iBase+1] = values[i].y;
			}

			UniformsFloat[name] = (VecLength,data);
		}
		public void SetVector3(string name,Vector3 value)
		{
			CheckUniform(name,"SetVector3");

			UniformsFloat[name] = (3,new[] { value.x,value.y,value.z });
		}
		public void SetVector3(string name,params Vector3[] values)
		{
			CheckUniform(name,"SetVector3");

			const int VecLength = 3;

			int iBase;

			var data = new float[values.Length*VecLength];

			for(int i = 0;i<values.Length;i++) {
				iBase = i*VecLength;

				data[iBase] = values[i].x;
				data[iBase+1] = values[i].y;
				data[iBase+2] = values[i].z;
			}

			UniformsFloat[name] = (VecLength,data);
		}
		public void SetVector4(string name,Vector4 value)
		{
			CheckUniform(name,"SetVector4");

			UniformsFloat[name] = (4,new[] { value.x,value.y,value.z,value.w });
		}
		public void SetVector4(string name,params Vector4[] values)
		{
			CheckUniform(name,nameof(SetVector4));

			const int VecLength = 4;

			int iBase;

			var data = new float[values.Length*VecLength];

			for(int i = 0;i<values.Length;i++) {
				iBase = i*VecLength;

				data[iBase] = values[i].x;
				data[iBase+1] = values[i].y;
				data[iBase+2] = values[i].z;
				data[iBase+3] = values[i].w;
			}

			UniformsFloat[name] = (VecLength,data);
		}
		public void SetVector(string name,float[] val)
		{
			CheckUniform(name,"SetVector");

			switch(val.Length) {
				case 2:
					SetVector2(name,new Vector2(val[0],val[1]));
					break;
				case 3:
					SetVector3(name,new Vector3(val[0],val[1],val[2]));
					break;
				case 4:
					SetVector4(name,new Vector4(val[0],val[1],val[2],val[3]));
					break;
				default:
					throw new Exception("Array's length must be in range [2..4]");
			}
		}
		public void SetTexture(string name,Texture texture)
		{
			CheckUniform(name,"SetTexture");

			for(int i = 0;i<Textures.Count;i++) {
				if(Textures[i].Key==name) {
					Textures[i] = new KeyValuePair<string,Texture>(name,texture);
					return;
				}
			}

			Textures.Add(new KeyValuePair<string,Texture>(name,texture));
		}
		public bool GetTexture(string name,out Texture texture) => (texture = Textures.FirstOrDefault(pair => pair.Key==name).Value)!=null;

		object ICloneable.Clone() => Clone();
	}
}