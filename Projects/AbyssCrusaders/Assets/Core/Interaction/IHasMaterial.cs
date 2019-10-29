using GameEngine;

namespace AbyssCrusaders
{
	public interface IHasMaterial
	{
		PhysicMaterial GetMaterial(Vector2? atPoint = null);
	}
}
