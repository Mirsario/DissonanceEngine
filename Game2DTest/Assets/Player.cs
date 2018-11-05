using System;
using System.Collections.Generic;
using GameEngine;

namespace Game
{
	public class Player : GameObject
	{
		public Sprite sprite;
		Vector3 rotation = new Vector3(0f,0f,0f);

		public override void OnInit()
		{
			layer = Layers.GetLayerIndex("Entity");
			sprite = AddComponent<Sprite>();
			sprite.Material = new Material("Player",Resources.Find<Shader>("Transparent/Cutout/Diffuse"));
			var texture = Resources.Get<Texture>("Player.png");
			sprite.Material.SetTexture("mainTex",texture);
			Transform.LocalScale = new Vector3(texture.Width/Main.unitSizeInPixels,texture.Height/Main.unitSizeInPixels,1f);
		}
		public override void FixedUpdate()
		{
			Vector3 offset = new Vector3(
				(Input.GetKey(Keys.Right) ? 1f : 0f)-(Input.GetKey(Keys.Left) ? 1f : 0f),
				(Input.GetKey(Keys.Up) ? 1f : 0f)-(Input.GetKey(Keys.Down) ? 1f : 0f),
				0f
			);
			rotation.z = Mathf.Repeat(rotation.z+((offset.x-offset.y)*Time.DeltaTime*360f*2f),360f);
			Transform.EulerRot = rotation;
			Transform.Position += offset*Time.DeltaTime*10f;
		}
	}
}