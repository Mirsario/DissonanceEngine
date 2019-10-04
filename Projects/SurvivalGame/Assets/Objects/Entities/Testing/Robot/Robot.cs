using GameEngine;
using GameEngine.Graphics;
using ImmersionFramework;

namespace SurvivalGame
{
	public class Robot : Entity
	{
		public SkinnedMeshRenderer renderer;

		public override void OnInit()
		{
			renderer = AddComponent<SkinnedMeshRenderer>(c => {
				c.Mesh = Resources.Import<Mesh>("Robot.mesh");
				c.Material = Resources.Find<Material>("DualSided");
			});
		}
		public override void FixedUpdate()
		{
			if(Input.GetKey(Keys.Up)) {
				Transform.Position += Vector3.Up*Time.FixedDeltaTime*4f;
			}
			if(Input.GetKey(Keys.Down)) {
				Transform.Position += Vector3.Down*Time.FixedDeltaTime*4f;
			}
			if(Input.GetKey(Keys.Right)) {
				//renderer.skeleton.bonesByName["RightUpperArm"].transform.eulerRot += new Vector3(0f,0f,Time.fixedDeltaTime*30f);
			}
		}
	}
}