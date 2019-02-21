using System;
using GameEngine;

namespace AbyssCrusaders
{
	public class CameraObj : GameObject2D
	{
		public const float DefaultZoom = 2f;
		
		private Vector2 pos;
		public Camera camera;
		public GameObject2D followObject;
		public RectFloat rect;
		public Vector2 mousePosition;
		public float zoomGoal;

		public float zoom;
		
		public override void OnInit()
		{
			zoomGoal = DefaultZoom;
			zoom = DefaultZoom;

			camera = AddComponent<Camera>();
			camera.orthographic = true;

			Depth = 1000f;
			//Depth = 50f;
			//Transform.EulerRot = new Vector3(0f,0f,180f);
		}
		public override void FixedUpdate()
		{
			if(followObject!=null) {
				float step = Time.FixedDeltaTime*8f;
				Position = pos = Vector2.Lerp(pos,followObject.Position,step); //zoom!=zoomGoal ? pos : Vector2.SnapToGrid(pos,1f/Main.UnitSizeInPixels/zoom);
				/*nextPos = Vector2.Lerp(pos,followObject.Position,step);
				lastUpdateTime = Time.GameTime;
				nextUpdateTime = Time.GameTime+Time.FixedDeltaTime;*/
			}
			//Transform.EulerRot = new Vector3(Mathf.Cos(Time.GameTime/3f)*2f,Mathf.Sin(Time.GameTime/7f)*2f,Mathf.Cos(Time.GameTime/5f)*10f);
		}
		public override void RenderUpdate()
		{
			float currentZoom = Main.UnitSizeInPixels*zoom;
			camera.orthographicSize = Math.Max(Screen.Width,Screen.Height)/currentZoom;
			Vector2 position = Position;
			//Vector2 position = Vector2.Lerp(nextPos,pos,(nextUpdateTime-Time.GameTime)/(nextUpdateTime-lastUpdateTime)); //Position;
			//Position = position;

			rect.width = Screen.Width/currentZoom;
			rect.height = Screen.Height/currentZoom;
			rect.x = position.x-rect.width*0.5f;
			rect.y = position.y-rect.height*0.5f;

			Vector2 mouseScreenPos = Input.MousePosition;
			mousePosition = new Vector2(
				Mathf.Lerp(rect.x,rect.x+rect.width,mouseScreenPos.x/Screen.Width),
				Mathf.Lerp(rect.y,rect.y+rect.height,mouseScreenPos.y/Screen.Height)
			);

			zoomGoal = Mathf.Clamp(zoomGoal+GameInput.zoom.Value,1f,4f);
			if(zoom!=zoomGoal) {
				zoom = Mathf.Lerp(zoom,zoomGoal,Time.RenderDeltaTime*16f);

				if(Mathf.Abs(zoom-zoomGoal)<0.01f) {
					zoom = zoomGoal;
				}
			}
		}
	}
}
