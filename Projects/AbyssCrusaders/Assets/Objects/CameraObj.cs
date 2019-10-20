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
		public RectFloat rectInPixels;
		public RectFloat zoomedRect;
		public RectFloat zoomedRectInPixels;
		public Vector2 mousePosition;
		public float zoomGoal;

		public float zoom;
		
		public override void OnInit()
		{
			zoomGoal = zoom = DefaultZoom;

			camera = AddComponent<Camera>(c => c.orthographic = true);

			Depth = 1000f;
			//Depth = 50f;
			//Transform.EulerRot = new Vector3(0f,0f,180f);
		}
		public override void RenderUpdate()
		{
			float currentZoom = Main.UnitSizeInPixels*zoom;
			camera.orthographicSize = Math.Max(Screen.Width,Screen.Height)/currentZoom;

			//Width & Height
			rect.width = Screen.Width*Main.PixelSizeInUnits;
			rect.height = Screen.Height*Main.PixelSizeInUnits;
			rectInPixels.width = Screen.Width;
			rectInPixels.height = Screen.Height;
			zoomedRect.width = Mathf.Ceil(rect.width/currentZoom);
			zoomedRect.height = Mathf.Ceil(rect.height/currentZoom);
			zoomedRectInPixels.width = Mathf.Ceil(rectInPixels.width/currentZoom);
			zoomedRectInPixels.height = Mathf.Ceil(rectInPixels.height/currentZoom);

			//Position
			Vector2 position = UpdatePosition((Vector2Int)zoomedRectInPixels.Size);

			rect.x = position.x-rect.width*0.5f;
			rect.y = position.y-rect.height*0.5f;
			rectInPixels.x = position.x-rectInPixels.width*0.5f;
			rectInPixels.y = position.y-rectInPixels.height*0.5f;
			zoomedRect.x = position.x-zoomedRect.width*0.5f;
			zoomedRect.y = position.y-zoomedRect.height*0.5f;
			zoomedRectInPixels.x = position.x-zoomedRectInPixels.width*0.5f;
			zoomedRectInPixels.y = position.y-zoomedRectInPixels.height*0.5f;

			Vector2 mouseScreenPos = Input.MousePosition;
			mousePosition = new Vector2(
				Mathf.Lerp(zoomedRectInPixels.x,zoomedRectInPixels.x+zoomedRectInPixels.width,mouseScreenPos.x/Screen.Width),
				Mathf.Lerp(zoomedRectInPixels.y,zoomedRectInPixels.y+zoomedRectInPixels.height,mouseScreenPos.y/Screen.Height)
			);

			zoomGoal = Mathf.Clamp(zoomGoal+GameInput.zoom.Value,1f,16f);

			if(zoom!=zoomGoal) {
				zoom = Mathf.Lerp(zoom,zoomGoal,Time.RenderDeltaTime*16f);

				if(Mathf.Abs(zoom-zoomGoal)<0.01f) {
					zoom = zoomGoal;
				}
			}
		}

		private Vector2 UpdatePosition(Vector2Int viewport)
		{
			var position = Position;

			if(followObject!=null) {
				float step = Time.FixedDeltaTime*8f;

				position = pos = Vector2.Lerp(pos,followObject.Position,step);

				//var offset = (Input.MousePosition-Screen.Center)/Screen.Center*8f;

				//position = Vector2.SnapToGrid(pos+offset-rect.Size*0.5f,0.1f/zoom)+rect.Size*0.5f; //zoom!=zoomGoal ? pos : Vector2.SnapToGrid(pos,1f/Main.UnitSizeInPixels/zoom);
			}

			//position = Vector2.SnapToGrid(position+viewport*0.5f,Main.PixelSizeInUnits)-viewport*0.5f;

			Position = position;

			return position;
		}
	}
}
