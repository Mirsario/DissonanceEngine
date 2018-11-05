using System;
using System.Collections.Generic;
using GameEngine;

namespace Game
{
	public abstract class Entity : GameObject
	{
		public bool IsLocal => Main.LocalEntity==this;
		public Vector3 size = Vector3.one;
		
		public override void OnInit()
		{
			layer = Layers.GetLayerIndex("Entity");

			//light = AddComponent<Light>();
			//light.color = new Vector3(GameEngine.Random.Range(0f,1f),GameEngine.Random.Range(0f,1f),GameEngine.Random.Range(0f,1f)).normalized;
		}
		public override void FixedUpdate()
		{
			/*if(!audioSource.isPlaying && GameEngine.Random.Next(150)==0) {
				audioSource.Play();
				Debug.Log("CUBE!");
			}*/
		}
		public virtual void UpdateIsPlayer(bool isPlayer)
		{
			
		}
		public virtual void UpdateCamera()
		{
			//Still has some temporary shit
			var cameraRotation = Main.cameraRotation;
			cameraRotation.x = Mathf.Clamp(cameraRotation.x-(Input.MouseDelta.y/15f),-89f,89f);
			cameraRotation.y -= Input.MouseDelta.x/15f;
			cameraRotation.z = Mathf.Lerp(cameraRotation.z,Input.GetDirection(Keys.Q,Keys.E)*15f,Time.DeltaTime*10f);
			Main.camera.Transform.LocalPosition = new Vector3(0f,2f,0f).RotatedBy(0,0f,cameraRotation.z*2f).RotatedBy(0f,cameraRotation.y,0f);
			
			Main.cameraRotation = cameraRotation;
			Main.camera.Transform.EulerRot = cameraRotation;
		}
	}
}