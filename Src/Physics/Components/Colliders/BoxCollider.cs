namespace Dissonance.Engine.Physics
{
	public struct BoxCollider : IComponent
	{
		public static readonly BoxCollider Default = new BoxCollider(Vector3.One);

		public Vector3 Size { get; set; }

		public BoxCollider(Vector3 size)
		{
			Size = size;
		}
	}
}
