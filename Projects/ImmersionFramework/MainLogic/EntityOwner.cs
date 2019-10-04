using GameEngine;
using GameEngine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImmersionFramework
{
	public abstract class EntityController<TEntity> : InputProxy where TEntity : EntityBase
	{
		public bool hasEntity;

		protected TEntity entity;

		public virtual TEntity Entity {
			get => entity;
			set {
				if(entity==value) {
					return;
				}

				OnSwitchEntity(entity,value);

				entity = value;
				hasEntity = entity!=null;
			}
		}

		public override void Dispose()
		{
			base.Dispose();

			entity = null;
		}

		protected virtual void OnSwitchEntity(TEntity prevEntity,TEntity newEntity)
		{
			//prevEntity?.UpdateController(this);
			//newEntity?.UpdateController(this);

			if(newEntity is InputProxy p) {
				AttachProxy(p);
			} else if(newEntity is IInputProxyProvaider pp) {
				AttachProxy(pp.Proxy);
			}
		}

		public bool TryGetEntity(out TEntity entity)
		{
			entity = this.entity;
			return hasEntity;
		}
	}
}
