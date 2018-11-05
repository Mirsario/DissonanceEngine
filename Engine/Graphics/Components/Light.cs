using System;
using System.Collections.Generic;

namespace GameEngine
{
	public enum LightType
	{
		Point,
		Directional,
		Spot
	}
	public class Light : Component
	{
		public LightType type = LightType.Point;
		public Vector3 color = Vector3.one;
		public float range = 16f;
		
		protected override void OnEnable()
		{
			Graphics.lightList.Add(this);
		}
		protected override void OnDisable()
		{
			Graphics.lightList.Remove(this);
		}
		protected override void OnDispose()
		{
			Graphics.lightList.Remove(this);
		}
		public override void FixedUpdate()
		{
			
		}
	}
}