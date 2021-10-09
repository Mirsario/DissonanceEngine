using System;
using System.Collections.Generic;
using System.Linq;
using Dissonance.Engine.Utilities;
using Dissonance.Framework.Graphics;

namespace Dissonance.Engine.Graphics
{
	//TODO: Implement OnDispose
	public sealed class Material : ICloneable
	{
		private static readonly List<Material> ById = new();
		private static readonly Dictionary<string, Material> ByName = new();

		private readonly List<KeyValuePair<string, Texture>> Textures = new();
		private readonly Dictionary<string, (byte size, float[] data)> UniformsFloat = new();

		private Shader shader;

		internal List<IRenderer> rendererAttachments;

		public int Id { get; }
		public string Name { get; }

		public Shader Shader {
			get => shader;
			set {
				if (shader == value) {
					return;
				}

				shader?.MaterialDetach(this);

				shader = value;

				shader?.MaterialAttach(this);
			}
		}

		public Material(string name, Shader shader)
		{
			Name = name;
			Id = InternalUtils.GenContentId(this, ById);
			Shader = shader;

			ByName[name] = this;
		}

		public Material Clone()
		{
			var clone = new Material(Name, Shader);

			UniformsFloat.CopyTo(clone.UniformsFloat);

			clone.Textures.AddRange(Textures);

			return clone;
		}

		internal void ApplyTextures(Shader shader)
		{
			ShaderUniform uniform;

			if (Textures.Count > 0) {
				for (int i = 0; i < Textures.Count && i < 32; i++) {
					string textureName = Textures[i].Key;
					var texture = Textures[i].Value;

					if (texture == null || !shader.uniforms.TryGetValue(textureName, out uniform)) {
						continue;
					}

					GL.ActiveTexture((TextureUnit)((int)TextureUnit.Texture0 + i));
					GL.BindTexture(TextureTarget.Texture2D, texture.Id);
					GL.Uniform1(uniform.location, i);
				}
			} else if (shader.uniforms.TryGetValue("mainTex", out uniform)) {
				GL.ActiveTexture(TextureUnit.Texture0);
				GL.BindTexture(TextureTarget.Texture2D, Rendering.whiteTexture.Id);
				GL.Uniform1(uniform.location, 0);
			}
		}

		internal void ApplyUniforms(Shader shader)
		{
			foreach (var pair in UniformsFloat) {
				(byte vecSize, var data) = pair.Value;

				int location = shader.uniforms[pair.Key].location;

				switch (vecSize) {
					case 1:
						GL.Uniform1(location, data.Length, data);
						break;
					case 2:
						GL.Uniform2(location, data.Length / 2, data);
						break;
					case 3:
						GL.Uniform3(location, data.Length / 3, data);
						break;
					case 4:
						GL.Uniform4(location, data.Length / 4, data);
						break;
				}
			}
		}

		private void CheckUniform(string name)
		{
			Shader shader = Shader;

			if (shader == null) {
				throw new Exception($"Material's Shader is null.");
			}

			if (!shader.uniforms.ContainsKey(name)) {
				throw new Exception($"Uniform {name} doesn't exist in shader ''{shader.Name}''.");
			}
		}

		public unsafe void SetFloat<T>(string name, byte vectorSize, T[] data) where T : unmanaged
		{
			if (vectorSize < 1 || vectorSize > 4) {
				throw new ArgumentException($"{nameof(vectorSize)} must be in [1..4] range (inclusively.)");
			}

			CheckUniform(name);

			float[] floatData = new float[data.Length * vectorSize];

			fixed(T* tPtr = data) {
				float* floatPtr = (float*)tPtr;

				for (int i = 0; i < floatData.Length; i++) {
					floatData[i] = floatPtr[i];
				}
			}

			UniformsFloat[name] = (vectorSize, floatData);
		}

		public void SetFloat(string name, params float[] values)
		{
			CheckUniform(name);

			UniformsFloat[name] = (1, values);
		}

		public void SetVector2(string name, params Vector2[] values)
			=> SetFloat(name, 2, values);

		public void SetVector3(string name, params Vector3[] values)
			=> SetFloat(name, 3, values);

		public void SetVector4(string name, params Vector4[] values)
			=> SetFloat(name, 4, values);

		public void SetVector(string name, float[] val)
		{
			CheckUniform(name);

			switch (val.Length) {
				case 2:
					SetVector2(name, new Vector2(val[0], val[1]));
					break;
				case 3:
					SetVector3(name, new Vector3(val[0], val[1], val[2]));
					break;
				case 4:
					SetVector4(name, new Vector4(val[0], val[1], val[2], val[3]));
					break;
				default:
					throw new Exception("Array's length must be in range [2..4]");
			}
		}

		public void SetTexture(string name, Texture texture)
		{
			CheckUniform(name);

			for (int i = 0; i < Textures.Count; i++) {
				if (Textures[i].Key == name) {
					Textures[i] = new KeyValuePair<string, Texture>(name, texture);
					return;
				}
			}

			Textures.Add(new KeyValuePair<string, Texture>(name, texture));
		}

		public bool GetTexture(string name, out Texture texture)
			=> (texture = Textures.FirstOrDefault(pair => pair.Key == name).Value) != null;

		object ICloneable.Clone()
			=> Clone();
	}
}
