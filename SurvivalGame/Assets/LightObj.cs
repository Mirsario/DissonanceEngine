using System;
using System.Collections.Generic;
using GameEngine;

namespace SurvivalGame
{
	public class LightObj : Entity
	{
		public Light light;
		public override void OnInit()
		{
			light = AddComponent<Light>();
			light.range = 16f;
			//light.color = Vector3.one;
			light.color = new Vector3(GameEngine.Rand.Range(0f,1f),GameEngine.Rand.Range(0f,1f),GameEngine.Rand.Range(0f,1f)).Normalized;
			//MeshRenderer renderer = AddComponent<MeshRenderer>();
			//renderer.mesh = Graphics.cubeMesh;
		}
		public override void FixedUpdate()
		{
			
		}
	}
}