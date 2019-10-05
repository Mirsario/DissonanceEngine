using GameEngine;
using ImmersionFramework;
using SurvivalGame.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame
{
	public class LocalPlayer : Player, ILocalPlayer
	{
		protected CameraController camera;
		protected Type prevCameraControllerType;

		public CameraController Camera {
			get => camera;
			set => camera = value;
		}
		public int LocalId { get; set; }

		public LocalPlayer(int id,int localId) : base(id)
		{
			LocalId = localId;
		}

		public override void FixedUpdate()
		{
			//Camera
			CheckCamera();

			if(Input.GetKey(Keys.Z) && LocalId!=0) {
				return;
			}

			//Inputs
			var inputs = Inputs;
			for(int i = 0;i<inputs.Length;i++) {
				ref var input = ref inputs[i];
				input.value = input.inputTrigger.Value;
			}

			base.FixedUpdate();
		}
		public override void RenderUpdate()
		{
			base.RenderUpdate();

			//Looks bad
			Vector2 delta = new Vector2(
				-inputs[SingletonInputTrigger.Info<LookX>.Id].inputTrigger.Value,
				-inputs[SingletonInputTrigger.Info<LookY>.Id].inputTrigger.Value
			);

			var rotation = LookRotation;
			rotation.x = Mathf.Clamp(rotation.x+delta.y,CameraController.MinLockedPitch,CameraController.MaxLockedPitch);
			rotation.y = Mathf.Repeat(rotation.y+delta.x,360f);
			LookRotation = rotation;

			//Debug.Log($"\r\nRotation:\r\n{rotation}\r\nDirection:\r\n{LookDirection}");
		}

		protected void CheckCamera(bool forceRecreation = false)
		{
			if(entity==null) {
				camera?.Dispose();
				camera = null;
				return;
			}

			var controllerType = entity.CameraControllerType;
			if(forceRecreation || controllerType!=prevCameraControllerType) {
				camera?.Dispose();

				if(controllerType==null || !typeof(CameraController).IsAssignableFrom(controllerType)) {
					throw new Exception($"Invalid CameraControllerType return value, '{controllerType?.ToString() ?? "null"}' does not derive from CameraController class.");
				}

				camera = (CameraController)GameObject.Instantiate(controllerType,init:false);
				camera.camera = InstantiateCamera(camera);
				camera.entity = entity;
				camera.Init();
			}

			prevCameraControllerType = controllerType;
		}
		private Camera InstantiateCamera(CameraController controller) //TODO: Move
		{
			controller.AddComponent<AudioListener>();

			return controller.AddComponent<Camera>(c => {
				c.fov = 110f;

				c.OnRenderStart = camera => {
					Skybox.OnRenderStart(camera);
					Sun.OnRenderStart(camera);
				};

				//Unepic
				c.view = LocalPlayerCount switch {
					1 => new RectFloat(0f,0f,1f,1f),
					2 => LocalId switch {
						0 => new RectFloat(0.0f,0.0f,0.5f,1.0f),
						1 => new RectFloat(0.5f,0.0f,0.5f,1.0f),
						_ => throw new IndexOutOfRangeException()
					},
					3 => LocalId switch {
						0 => new RectFloat(0.0f,0.5f,1.0f,0.5f),
						1 => new RectFloat(0.0f,0.0f,0.5f,0.5f),
						2 => new RectFloat(0.5f,0.0f,0.5f,0.5f),
						_ => throw new IndexOutOfRangeException()
					},
					4 => LocalId switch {
						0 => new RectFloat(0.0f,0.0f,0.5f,0.5f),
						1 => new RectFloat(0.5f,0.0f,0.5f,0.5f),
						2 => new RectFloat(0.0f,0.5f,0.5f,0.5f),
						3 => new RectFloat(0.5f,0.5f,0.5f,0.5f),
						_ => throw new IndexOutOfRangeException()
					},
					_ => throw new ArgumentException()
				};
			});
		}
	}
}
