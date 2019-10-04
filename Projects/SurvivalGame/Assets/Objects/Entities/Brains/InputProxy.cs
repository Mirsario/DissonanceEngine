using GameEngine;
using GameEngine.Utils;
using ImmersionFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame
{
	//public InputSource Source { get; protected set; }
	//public InputProxy Parent { get; protected set; }
	//public InputProxy Child { get; protected set; }
	//
	//IInputProxy IInputProxy.Parent => Parent;
	//IInputProxy IInputProxy.Child => Child;

	/*public abstract class InputProxy : Entity, IInputProvaider
	{
		protected InputSignal[] inputs;

		public InputSignal[] Inputs => inputs;

		public InputProxy[] AttachedProxies { get; protected set; }
		public Vector3 LookDirection { get; protected set; }

		public void AttachProxy(InputProxy proxy)
		{
			var proxies = AttachedProxies;
			ArrayUtils.Add(ref proxies,proxy);
			AttachedProxies = proxies;
		}
		public T AttachProxy<T>(T proxy) where T : InputProxy
		{
			AttachProxy(proxy);
			return proxy;
		}

		public virtual void CopyInputs(InputSignal[] source,Vector3 lookDirection)
		{
			LookDirection = lookDirection;

			int length = source.Length;

			if(inputs==null || inputs.Length!=length) {
				inputs = new InputSignal[source.Length];
			}

			for(int i = 0;i<length;i++) {
				var src = source[i];
				var dest = this.inputs[i];

				dest.prevValue = dest.value;
				dest.value = src.value;
			}
		}
	}*/
}
