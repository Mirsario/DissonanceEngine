using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace GameEngine
{
	public class SkinnedMeshRenderer : MeshRenderer
	{
		public override Mesh Mesh {
			get => _mesh;
			set {
				if(value!=null) {
					if(skeleton!=null) {
						skeleton.Dispose();
						skeleton = null;
					}
					if(value.skeleton!=null) {
						skeleton = value.skeleton.Instantiate(Transform);
					}else{
						Debug.Log($"{gameObject.Name} - mesh's skeleton is null");
					}
				}
				_mesh = value;
			}
		}
		public AnimationSkeleton skeleton;
		
		public override void FixedUpdate()
		{
			
		}
		public override void ApplyUniforms(Shader shader)
		{
			//Debug.Log("lala1");
			if(skeleton!=null) {
				for(int i=0;i<skeleton.bones.Length;i++) {
					var matrix = skeleton.bones[i].transform.Matrix;
					matrix = Matrix4x4.CreateRotation(matrix.ExtractEuler());
					Shader.UniformMatrix4(GL.GetUniformLocation(shader.program,$"boneMatrices[{i}]"),ref matrix);
				}
			}
		}
	}
}