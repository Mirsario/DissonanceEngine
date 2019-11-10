using AbyssCrusaders.Core;
using GameEngine;
using GameEngine.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbyssCrusaders.Content.TileEntities.Nature.Trees
{
	public abstract class Tree : TileEntity
	{
		public override void OnInit()
		{
			base.OnInit();

			var sprite = AddComponent<Sprite>(c => {
				c.Material = Resources.Get<Material>($"{GetType().Name}.material");
				c.Origin = new Vector2(0.5f,1.0f);
				c.spriteEffects = (Sprite.SpriteEffects)Rand.Next(2);
			});

			Depth = -1f;
		}
		public override void RenderUpdate()
		{
			base.RenderUpdate();

			Rotation = Mathf.Sin(Time.RenderGameTime+Position.x*0.1f);
		}
	}
}
