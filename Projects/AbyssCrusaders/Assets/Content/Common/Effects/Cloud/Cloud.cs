using AbyssCrusaders.Core;
using GameEngine;
using GameEngine.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbyssCrusaders.Content.Common.Effects
{
	public class Cloud : GameObject2D
	{
		private Sprite sprite;

		public override void OnInit()
		{
			base.OnInit();

			sprite = AddComponent<Sprite>(c => {
				c.Material = Resources.Get<Material>($"{GetType().Name}.material");
				c.Origin = new Vector2(0.5f,0.5f);

				if(Rand.Next(2)==0) {
					c.FrameSize = new Vector2(192,96);
					c.SourceRectangle = new RectFloat(0f,Rand.Next(4)*0.25f,0.5f,0.25f);
				} else {
					c.FrameSize = new Vector2(96,96);
					c.SourceRectangle = new RectFloat(0.5f+Rand.Next(2)*0.25f,Rand.Next(2)*0.25f,0.25f,0.25f);
				}
			});

			Depth = -20f;
		}
		public override void RenderUpdate()
		{
			base.RenderUpdate();

			var pos = Position;

			var frameSize = sprite.FrameSizeInUnits;
			var origin = sprite.Origin;
			float offsetToRight = frameSize.x*(1f-origin.x);
			float offsetToLeft = -frameSize.x*origin.x;

			pos += new Vector2(Time.RenderDeltaTime,0f);

			if(pos.x+offsetToLeft>Main.camera.rect.Right) {
				pos.x -= Main.camera.rect.width+frameSize.x;
			}else if(pos.x+offsetToRight<Main.camera.rect.x) {
				pos.x += Main.camera.rect.width+frameSize.x;
			}

			Position = pos;
		}
	}
}
