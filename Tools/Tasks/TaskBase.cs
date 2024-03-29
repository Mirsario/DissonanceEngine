﻿using System;
using Microsoft.Build.Utilities;

namespace Dissonance.BuildTools.Tasks
{
	public abstract class TaskBase : Task
	{
		public sealed override bool Execute()
		{
			try {
				Run();
			}
			catch (Exception e) {
				Log.LogErrorFromException(e, true);
			}

			return !Log.HasLoggedErrors;
		}

		protected abstract void Run();
	}
}
