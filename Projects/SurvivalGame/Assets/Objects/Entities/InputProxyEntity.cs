using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameEngine;
using ImmersionFramework;

namespace SurvivalGame
{
	//bad bad bad
	public abstract class InputProxyEntity : PhysicalEntity, IInputProxyProvaider, IInputProvaider
	{
		protected InputProxy inputProxy;

		public InputProxy Proxy => inputProxy;

		public InputSignal[] Inputs => inputProxy?.Inputs;
		public Vector3 LookDirection => inputProxy==null ? Vector3.Forward : inputProxy.LookDirection;
		public Vector3 LookRotation => inputProxy==null ? default : inputProxy.LookRotation;

		public virtual bool HideOnAttach => true;

		public virtual void ModifyInputs(InputSignal[] signals) {}

		public override void OnInit()
		{
			base.OnInit();

			inputProxy = new CustomInputProxy(signals => ModifyInputs(signals));
		}

		protected override void OnAttachedTo(PhysicalEntity entity)
		{
			base.OnAttachedTo(entity);

			if(entity is IInputProxyProvaider provaider) {
				Proxy.AttachProxy(provaider.Proxy);
			}
		}
		protected override void OnDetachedFrom(PhysicalEntity entity)
		{
			base.OnDetachedFrom(entity);

			Proxy.DetachProxy();
		}

		//public T AttachTo<T>(T proxy) where T : InputProxy => Proxy.AttachProxy(proxy);
		//public T AttachTo<T>(T provaider) where T : IInputProxyProvaider => Proxy.AttachProxy(provaider.Proxy);
	}
}
