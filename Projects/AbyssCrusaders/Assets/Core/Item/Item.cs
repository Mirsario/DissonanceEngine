using GameEngine;
using GameEngine.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AbyssCrusaders.Core
{
	public abstract class Item : PhysicalEntity
	{
		public static class IDs<T> where T : Item
		{
			public static readonly int Id;
		}

		public static Type[] typeById;

		public virtual Material UsedMaterial {
			get {
				string name = GetType().Name;

				var material = Resources.Find<Material>($"Game/Content/Items/{name}",false);
				if(material==null) {
					material = Resources.Find<Material>("Game/SpriteDefault").Clone();
					material.name = name;

					material.SetTexture("mainTex",Resources.Get<Texture>($"{name}.png"));

					material.RegisterAsset();
				}

				return material;
			}
		}

		public override void OnInit()
		{
			base.OnInit();

			autoStepUp = autoStepDown = false;

			var sprite = AddComponent<Sprite>(c => {
				c.Material = UsedMaterial;
				c.Origin = new Vector2(0.5f,0.5f);
			});
		}
		public override void FixedUpdate()
		{
			base.FixedUpdate();

			ApplyFriction();

			if(velocity.y!=0f) {
				Rotation += Math.Max(Math.Abs(velocity.x),Math.Abs(velocity.y))*10f*Time.FixedDeltaTime;
			} else {
				Rotation = Mathf.LerpAngle(Rotation,0f,3f*Time.FixedDeltaTime);
			}
		}

		public static int GetTypeId<T>() where T : Item => IDs<T>.Id;

		internal static void Initialize()
		{
			var baseIDType = typeof(IDs<>);

			int id = 0;

			IEnumerable<Type> Enumeration()
			{
				foreach(var type in Assembly.GetExecutingAssembly().GetTypes()) {
					if(type.IsAbstract || !typeof(Item).IsAssignableFrom(type)) {
						continue;
					}

					typeof(IDs<>).MakeGenericType(type)
						.GetField(nameof(IDs<Item>.Id),BindingFlags.Static|BindingFlags.Public)
						.SetValue(null,id);

					yield return type;

					id++;
				}
			}

			typeById = Enumeration().ToArray();

			Debug.Log("Wood: "+GetTypeId<Content.Items.Placeables.Building.Wood>());
			Debug.Log("Stone: "+GetTypeId<Content.Items.Placeables.Building.Stone>());
		}
	}
}
