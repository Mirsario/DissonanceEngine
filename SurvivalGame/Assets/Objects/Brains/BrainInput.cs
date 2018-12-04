namespace Game
{
	public struct BrainInput
	{
		public bool active;
		public bool prevActive;

		public bool JustActivated => active && !prevActive;
		public bool JustDeactivated => !active && prevActive;
	}
}