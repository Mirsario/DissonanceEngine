using GameEngine;
using GameEngine.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbyssCrusaders.Core
{
	public abstract class Item : Entity
	{
		public virtual Material UsedMaterial {
			get {
				string name = GetType().Name;

				var material = Resources.Find<Material>($"Game/Content/Items/{name}");
				if(material==null) {
					material = Resources.Get<Material>("Game/SpriteDefault").Clone();

					material.SetTexture("mainTex",Resources.Get<Texture>(""));
				}

				return material;
			}
		}
	}
}
