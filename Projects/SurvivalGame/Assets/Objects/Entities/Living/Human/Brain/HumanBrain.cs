using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameEngine;
using GameEngine.Graphics;
using GameEngine.Physics;
using ImmersionFramework;

namespace SurvivalGame
{
	public class HumanBrain : Brain
	{
		public override void SetupRenderers(ref Renderer[] renderers)
		{
			string typeName = GetType().Name;

			renderers = new[] {
				AddComponent<MeshRenderer>(c => {
					c.Mesh = Resources.Get<Mesh>($"{typeName}.obj");
					c.Material = Resources.Find<Material>($"{typeName}");
				})
			};
		}
		public override void SetupPhysicsComponents(ref Collider[] colliders,ref Rigidbody rigidbody)
		{
			colliders = new[] {
				AddComponent<MeshCollider>(c => c.Mesh = Resources.Get<ConvexCollisionMesh>($"{GetType().Name}.obj"))
			};

			rigidbody = AddComponent<Rigidbody>(c => c.Mass = 1f);
		}

		public override void FixedUpdate()
		{
			if(Input.GetKeyDown(Keys.I)) {
				Detach();
			}
		}

		/*public override void ModifyInputs(InputSignal[] signals)
		{
			base.ModifyInputs(signals);

			//Drunk test
			for(int i = 0;i<signals.Length;i++) {
				ref var signal = ref signals[i];

				if(signal.value!=signal.prevValue) {
					float goalValue = signal.value;
					signal.value = signal.prevValue;
					signal.value = Mathf.Lerp(signal.value,goalValue,Time.FixedDeltaTime);
					signal.value = Mathf.StepTowards(signal.value,goalValue,Time.FixedDeltaTime*0.01f);
				}
			}
		}*/
	}
}
