using System;
using System.Collections.Generic;

namespace GameEngine
{
	//TODO: Actually implement animations
	public class AnimationSkeleton : IDisposable
	{
		public string name;
		public AnimationBone rootBone;
		public AnimationBone[] bones;
		public Dictionary<string,AnimationBone> bonesByName;

		public AnimationSkeleton(string name)
		{
			this.name = name;
		}
		/*public Skeleton Copy(Transform parentTransform)
		{
			Skeleton skeleton = new Skeleton(name);
			skeleton.bonesByName = new Dictionary<string,Bone>();
			skeleton.bones = new Bone[bones.Length];
			for(int i=0;i<bones.Length;i++) {
				skeleton.bones[i] = new Bone(bones[i].name);
				skeleton.bones[i].transform.parent = parentTransform;
				skeleton.bones[i].transform.localPosition = bones[i].transform.localPosition;
				skeleton.bones[i].transform.localRotation = bones[i].transform.rotation;
				skeleton.bones[i].transform.scale = bones[i].transform.scale;
				skeleton.bonesByName.Add(skeleton.bones[i].name,skeleton.bones[i]);
			}
			return skeleton;
		}*/
		public AnimationSkeleton Instantiate(Transform parentTransform)
		{
			var skeleton = new AnimationSkeleton(name) {
				bonesByName = new Dictionary<string,AnimationBone>(),
				bones = new AnimationBone[bones.Length]
			};
			for(int i=0;i<bones.Length;i++) {
				var bone = new AnimationBone(bones[i].name) {
					baseMatrix = bones[i].baseMatrix,
					transform = new Transform()
				};
				skeleton.bones[i] = bone;
				skeleton.bonesByName.Add(bone.name,bone);
			}
			for(int i=0;i<bones.Length;i++) {
				var bone = skeleton.bones[i];
				bone.transform.parent = bone.parent==null ? parentTransform : bone.parent.transform;
				bone.transform.Matrix = bone.baseMatrix;

				/*GameObject box = new GameObject();
				box.transform = bone.transform;
				box.AddComponent<MeshRenderer>().mesh = Graphics.cubeMesh;*/
			}
			return skeleton;
		}
		public void Dispose()
		{
			for(int i=0;i<bones.Length;i++) {
				bones[i].Dispose();
			}
			bones = null;
			rootBone = null;
			bonesByName.Clear();
			bonesByName = null;
		}
	}
}
