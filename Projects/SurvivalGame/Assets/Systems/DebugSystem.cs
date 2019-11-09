using GameEngine;
using GameEngine.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame
{
	public class DebugSystem : GameObject
	{
		public override void FixedUpdate()
		{
			var player = Player.localPlayers[0];
			var entity = player.Entity;
			var camera = player.Camera;
			var world = entity?.world;

			if(Input.GetKeyDown(Keys.C)) { //Sound test
				if(entity==null) {
					Debug.Log("No honk...");
				} else {
					Debug.Log("HONK!");

					const string Clip = "Sounds/honk.wav";

					var instance = SoundInstance.Create(Clip,entity.Transform.Position);

					instance.source.Clip?.Dispose();
				}
			}

			if(world!=null) {
				if(Input.GetKeyDown(Keys.K) && camera!=null) { //Add light
					Entity.Instantiate<LightObj>(world,position: camera.Transform.Position);
				}

				if(PhysicsEngine.Raycast(camera.Transform.Position,camera.Transform.Forward,out var hit,customFilter: obj => (obj is Entity e && e.IsPlayer) ? false : (bool?)null)) {
					if(Input.GetMouseButtonDown(MouseButton.Middle) && hit.gameObject is PhysicalEntity e) {
						player.Entity = e;
						//screenFlash = 0.5f;
						SoundInstance.Create($"Magic.ogg",e.Transform.Position);
					}

					if(Input.GetKeyDown(Keys.X)) { //Teleport
						Transform.Position = hit.point+Vector3.Up;
					}

					if(Input.GetKeyDown(Keys.J)) {
						Entity.Instantiate<RaisingPlatform>(world,position: hit.point);
					}

					if(Input.GetKeyDown(Keys.V)) {
						Entity.Instantiate<Robot>(world,position: hit.point);
					}

					if(Input.GetKey(Keys.B)) {
						Entity.Instantiate<StoneHatchet>(world,position: hit.point+new Vector3(0f,15f,0f));
					}

					if(Input.GetKeyDown(Keys.N)) {
						Entity.Instantiate<CubeObj>(world,position: hit.point+new Vector3(0f,5f,0f));
					}

					if(Input.GetKeyDown(Keys.M)) {
						Entity.Instantiate<CubeObj2>(world,position: hit.point + new Vector3(0f,5f,0f));
					}

					if(Input.GetKeyDown(Keys.H)) {
						Entity.Instantiate<GiantPlatform>(world,position: hit.point);
					}
				}
			}
		}
	}
}
