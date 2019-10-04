/*using GameEngine;
using GameEngine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImmersionFramework
{
	public abstract class InputSource : IInputProvaider
	{
		public InputSignal[] Inputs { get; protected set; }
		public InputProxy[] AttachedProxies { get; protected set; }
		public Vector3 LookDirection { get; protected set; }

		public abstract void UpdateInputs();

		public void Update()
		{
			UpdateInputs();

			var inputs = Inputs;

			var proxies = AttachedProxies;
			if(proxies!=null) {
				for(int i = 0;i<proxies.Length;i++) {
					proxies[i].CopyInputs(inputs,LookDirection);
				}
			}
		}

		public void AttachProxy(InputProxy proxy)
		{
			var proxies = AttachedProxies;
			ArrayUtils.Add(ref proxies,proxy);
			AttachedProxies = proxies;
		}
	}
}*/
