using GameEngine;
using GameEngine.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImmersionFramework
{
	public class CustomInputProxy : InputProxy
	{
		public delegate void ModifyInputsDelegate(InputSignal[] signals);

		protected ModifyInputsDelegate modifyInputs;

		public CustomInputProxy(ModifyInputsDelegate modifyInputs) : base()
		{
			this.modifyInputs = modifyInputs;
		}

		public override void ReceiveInputs(InputProxy source)
		{
			base.ReceiveInputs(source);

			modifyInputs?.Invoke(inputs);
		}

		public override void Dispose()
		{
			base.Dispose();

			modifyInputs = null;
		}
	}
}
