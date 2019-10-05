using GameEngine;
using GameEngine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImmersionFramework
{
	public abstract class InputProxy : IInputProvaider, IDisposable
	{
		protected InputSignal[] inputs;

		private Vector3 lookDirection;
		private Vector3 lookRotation;

		public InputSignal[] Inputs => inputs;

		public InputProxy Parent { get; protected set; }
		public InputProxy Child { get; protected set; }

		public Vector3 LookDirection {
			get => lookDirection;
			protected set {
				lookDirection = value;
				lookRotation = Vector3.DirectionToEuler(lookDirection);
			}
		}
		public Vector3 LookRotation {
			get => lookRotation;
			protected set {
				lookRotation = value;
				lookDirection = Vector3.EulerToDirection(lookRotation);
			}
		}

		public InputProxy()
		{
			var triggers = Input.Triggers;

			inputs = new InputSignal[InputTrigger.Count];
			for(int i = 0;i<InputTrigger.Count;i++) {
				inputs[i] = new InputSignal(triggers[i]);
			}
		}

		public virtual void ReceiveInputs(InputProxy source)
		{
			var sourceInputs = source.Inputs;

			LookRotation = source.LookRotation; //LookDirection = source.LookDirection;

			int length = sourceInputs.Length;

			if(inputs==null || inputs.Length!=length) {
				inputs = new InputSignal[sourceInputs.Length];
			}

			for(int i = 0;i<length;i++) {
				ref var dest = ref inputs[i];

				dest.prevValue = dest.value;
				dest.value = sourceInputs[i].value;
			}

			CopyInputs();
		}
		public virtual void CopyInputs()
		{
			Child?.ReceiveInputs(this);
		}

		public virtual void Dispose()
		{
			inputs = null;
			Child = null;
		}

		public T AttachProxy<T>(T proxy) where T : InputProxy
		{
			Child = proxy;
			proxy.Parent = this;
			return proxy;
		}
		public void DetachProxy()
		{
			if(Child!=null) {
				Child.Parent = null;
				Child = null;
			}
		}
	}
}
