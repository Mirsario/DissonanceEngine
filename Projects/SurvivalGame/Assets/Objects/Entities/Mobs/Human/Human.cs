using System;
using System.Linq;
using GameEngine;
using GameEngine.Graphics;

namespace SurvivalGame
{
	public class Human : Mob
	{
		//TODO: Make this not bad.

		public const float AirAcceleration = 0.4f;
		public const float JumpSpeed = 7.5f;
		public const float StopSpeed = 1f;
		public const float Friction = 4.5f;
		
		public bool wasOnGround;
		public float forceAirMove;
		public float lastLand;
		public float walkTime;
		public float jumpPress;
		public bool enableStrafeJumping;
		public float screenFlash;
		public Vector2 moveInput;
		public Vector3 size = Vector3.one;
		public Vector3 prevVelocity;
		public MeshRenderer renderer;
		public CylinderCollider collider;
		public Rigidbody rigidbody;
		public AudioSource audioSource;
		public bool isSprinting;
		public bool onGroundCached;

		public bool OnGround => rigidbody.Collisions.Any(c => c.contacts.Any(p => p.point.y>=Transform.Position.y-0.1f));
		public float MoveSpeed => (isSprinting && onGroundCached) ? 12f : 7.5f;
		public float Acceleration => 4.25f;

		public override Type CameraControllerType => Main.forceThirdPerson ? typeof(BasicThirdPersonCamera) : typeof(FirstPersonCamera);

		public override void OnInit()
		{
			size = new Vector3(1f,1.95f,1f); //JoJo height.

			collider = AddComponent<CylinderCollider>(false);
			collider.size = size;
			collider.offset = new Vector3(0f,size.y/2f,0f);
			collider.Enabled = true;

			rigidbody = AddComponent<Rigidbody>();
			rigidbody.Mass = 1f;
			rigidbody.AngularFactor = Vector3.zero;
			
			renderer = AddComponent<MeshRenderer>();
			renderer.Mesh = Resources.Get<Mesh>("Robot.mesh");
			renderer.Material = Resources.Get<Material>("Robot.material");

			rigidbody.UseGravity = false;
			rigidbody.Friction = 0f;
			rigidbody.Drag = 0f;

			audioSource = AddComponent<AudioSource>();
		}
		public override void FixedUpdate()
		{
			if(IsLocal) {
				renderer.Enabled = Main.forceThirdPerson;
			}

			var camera = Main.camera;
			if(Input.GetKeyDown(Keys.C)) { //Sound test
				Debug.Log("HONK!");
				PlayVoiceClip(Resources.Get<AudioClip>("Sounds/honk.wav"));
			}

			if(Input.GetKeyDown(Keys.K)) { //Add light
				Instantiate<LightObj>(world,position:camera.Transform.Position);
			}

			moveInput = brain.Signal2(GameInput.moveX,GameInput.moveY);

			if(moveInput.x==0f && moveInput.y<=0f) {
				isSprinting = false;
			}else if(brain.JustActivated(GameInput.sprint)) {
				isSprinting = true;
			}

			base.FixedUpdate();

			var euler = Transform.EulerRot;
			Transform.EulerRot = new Vector3(euler.x,brain.Transform.EulerRot.y,euler.z);
			
			if(brain.JustActivated(GameInput.primaryUse)) {
				Vector3 direction = brain?.LookDirection ?? Vector3.forward;
				Rocket rocket = Instantiate<Rocket>(world,position:camera.Transform.Position+direction);
				rocket.velocity = direction*25f;
				rocket.owner = this;
				SoundInstance.Create("Sounds/rocketFire.wav",camera.Transform.Position);
			}

			jumpPress = brain.JustActivated(GameInput.jump) ? 0.25f : Math.Max(0f,jumpPress-Time.FixedDeltaTime);
			Movement();

			var tempPos = Transform.Position;
			tempPos.x = Mathf.Repeat(tempPos.x,Main.world.xSizeInUnits);
			tempPos.z = Mathf.Repeat(tempPos.z,Main.world.ySizeInUnits);
			if(tempPos.y+2f<Main.world.HeightAt(Transform.Position.XZ,false)) {
				tempPos.y += 5f;
			}

			if(Transform.Position!=tempPos) {
				Transform.Position = tempPos;
			}

			if(Physics.Raycast(camera.Transform.Position,camera.Transform.Forward,out var hit,customFilter:obj => Main.LocalEntity==obj ? false : (bool?)null)) {
                if(hit.gameObject is Entity entity && Input.GetMouseButtonDown(MouseButton.Middle)) {
                    Main.LocalEntity = entity;
                    screenFlash = 0.5f;
                    SoundInstance.Create($"Magic.ogg",entity.Transform.Position);
                }
                if(Input.GetKeyDown(Keys.X)) { //Teleport
                    Transform.Position = hit.point+Vector3.up;
                }
                if(Input.GetKeyDown(Keys.J)) {
                    Instantiate<RaisingPlatform>(world,position: hit.point);
                }
                if(Input.GetKeyDown(Keys.V)) {
                    Instantiate<Robot>(world,position: hit.point);
                }
                if(Input.GetKeyDown(Keys.B)) {
                    Instantiate<StoneHatchet>(world,position: hit.point+new Vector3(0f,15f,0f));
                }
                if(Input.GetKeyDown(Keys.N)) {
                    Instantiate<TexTest>(world,position: hit.point+new Vector3(0f,15f,0f));
                }
                if(Input.GetKeyDown(Keys.M)) {
                    Instantiate<TestSphere>(world,position: hit.point + new Vector3(0f,15f,0f));
                }
                if(Input.GetKeyDown(Keys.H)) {
                    Instantiate<GiantPlatform>(world,position: hit.point);
                }
                if(Input.GetKeyDown(Keys.U)) {
                    enableStrafeJumping = !enableStrafeJumping;
                }
            }
		}
		public override void RenderUpdate()
		{
			if(screenFlash>0f) {
				screenFlash = Mathf.StepTowards(screenFlash,0f,Time.RenderDeltaTime);
			}
		}
		public override void OnGUI()
		{
			if(Main.hideUI) {
				return;
			}
			
			int i = 5;
			GUI.DrawText(new RectFloat(8,8+(i++*16),128,8),$"Player Speed - XZ:{new Vector3(velocity.x,0f,velocity.z).Magnitude:0.00} Y: {velocity.y:0.00}");
			GUI.DrawText(new RectFloat(8,8+(i++*16),128,8),$"Quake 3 Acceleration: {(enableStrafeJumping ? "Enabled" : "Disabled")} ([U] - Toggle)");
			GUI.DrawText(new RectFloat(8,8+(i++*16),128,8),$"Input: {moveInput.x:0.00},{moveInput.y:0.00}");
			GUI.DrawText(new RectFloat(8,8+(i*16),128,8),$"Sprinting: {isSprinting}");

			if(screenFlash>0f) {
				GUI.DrawTexture(Screen.Rectangle,Main.whiteTexture,new Vector4(0.75f,0f,0f,screenFlash));
			}
		}
		public override void UpdateIsPlayer(bool isPlayer)
		{
			base.UpdateIsPlayer(isPlayer);

			if(!isPlayer) {
				renderer.Enabled = true;
			}
		}

		#region Movement
		public void Movement()
		{
			velocity = rigidbody.Velocity;

			onGroundCached = OnGround;

			var forward = brain?.Transform.Forward ?? Vector3.forward;
			var right = brain?.Transform.Right ?? Vector3.right;
			forward.y = 0f;
			right.y = 0f;
			forward.Normalize();
			right.Normalize();

			if(onGroundCached && forceAirMove<=0f) {
				if(moveInput!=default) {
					walkTime += Time.FixedDeltaTime;
					if(walkTime>=(isSprinting ? 0.3f : 0.5f)) {
						Footstep("Walk",0.3f);
						walkTime = 0f;
					}
				} else {
					walkTime = 0f;
				}

				Movement_WalkMove(forward,right);

				if(!wasOnGround) {
					Footstep("Land",0.8f);
					float magnitude = prevVelocity.Magnitude-velocity.Magnitude;
					const float minSpeed = 8.5f;
					if(magnitude>minSpeed) {
						float power = Mathf.Lerp(0f,1f,(magnitude-minSpeed)*0.2f);
						screenFlash = power*0.5f;
						SoundInstance.Create($"FallBig{Rand.Range(1,6)}.ogg",Transform.Position+(velocity*Time.FixedDeltaTime),power);
						ScreenShake.New(power*0.5f,1f,5f,Transform.Position);
					}
				}

				lastLand = Time.FixedDeltaTime;
				wasOnGround = true;
			} else {
				walkTime = 0f;

				if(forceAirMove>0f) {
					forceAirMove = Math.Max(0f,forceAirMove-Time.FixedDeltaTime);
				}

				Movement_AirMove(forward,right);

				wasOnGround = false;
			}

			velocity.y -= 18f*Time.FixedDeltaTime; //Temporary implementation, doing gravity through the physics engine would probably be better. 
			rigidbody.Velocity = velocity;
			prevVelocity = velocity;
		}
		public void Movement_WalkMove(Vector3 forward,Vector3 right)
		{
			if(Movement_CheckJump()) {
				PlayVoiceClip(Resources.Get<AudioClip>("Jump.ogg"));
				//SoundInstance.Create("Land.ogg",transform.position);
				Movement_AirMove(forward,right);

				forceAirMove = 0.1f;
				return;
			}

			Movement_Friction();

			var wishDirection = (forward*moveInput.y)+(right*moveInput.x);
			float wishSpeed = MoveSpeed;
			float acceleration = Acceleration;

			Movement_Acceleration(wishDirection,wishSpeed,acceleration);
		}
		public void Movement_AirMove(Vector3 forward,Vector3 right)
		{
			Movement_Friction();

			var wishDirection = (forward*moveInput.y)+(right*moveInput.x);
			float wishSpeed = MoveSpeed;
			float acceleration = AirAcceleration;

			Movement_Acceleration(wishDirection,wishSpeed,acceleration);
		}
		public bool Movement_CheckJump()
		{
			if(jumpPress<=0f) {
				return false;
			}
			if(velocity.y<0f) {
				velocity.y = JumpSpeed;
			}else{
				velocity.y += JumpSpeed;
			}
			return true;
		}
		public void Movement_Friction()
		{
			var tempVec = velocity;
			tempVec.y = 0f;
			float speed = tempVec.Magnitude;
			float drop = 0f;
			if(onGroundCached) {
				drop = (speed<StopSpeed ? StopSpeed : speed)*Friction*Time.FixedDeltaTime;
			}
			float newSpeed = speed-drop;
			if(newSpeed<0) {
				newSpeed = 0;
			}
			if(newSpeed!=0f) {
				newSpeed /= speed;
			}
			velocity.x *= newSpeed; //Temporary! Do an air friction method.
			velocity.z *= newSpeed;
		}
		public void Movement_Acceleration(Vector3 wishDirection,float wishSpeed,float acceleration)
		{
			//Acceleration with strafejumping
			if(enableStrafeJumping) {
				float currentSpeed = Vector3.Dot(velocity,wishDirection);
				float addSpeed = wishSpeed-currentSpeed;
				if(addSpeed<=0f) {
					return;
				}
				float accelSpeed = acceleration*wishSpeed*Time.FixedDeltaTime;
				if(accelSpeed>addSpeed) {
					accelSpeed = addSpeed;
				}
				velocity += wishDirection*accelSpeed;
			}else{
				var wishVelocity = wishDirection*wishSpeed;
				var pushDirection = wishVelocity-velocity;
				pushDirection.Normalize(out float pushLength);
				float accelSpeed = acceleration*wishSpeed*Time.FixedDeltaTime;
				if(accelSpeed>pushLength) {
					accelSpeed = pushLength;
				}
				velocity += pushDirection*accelSpeed;
			}
		}
		#endregion

		public void PlayVoiceClip(AudioClip clip)
		{
			if(audioSource.IsPlaying) {
				audioSource.Stop();
			}
			audioSource.Clip = clip;
			audioSource.Play();
		}
		public void Footstep(string actionType,float volume)
		{
			var atPoint = Transform.Position;
			// ReSharper disable once SuspiciousTypeConversion.Global
			var providers = rigidbody.Collisions.SelectIgnoreNull(c => c.gameObject as IFootstepProvider ?? ((c.gameObject as IHasMaterial)?.GetMaterial(atPoint))).ToArray();
			if(providers.Length>0) {
				var provider = providers[0];
				provider.GetFootstepInfo(atPoint,out string surfaceType,ref actionType,out int numSoundVariants);
				SoundInstance.Create($"Footstep{surfaceType}{actionType}{(numSoundVariants>0 ? Rand.Range(1,numSoundVariants+1).ToString() : null)}.ogg",atPoint+(velocity*Time.DeltaTime),volume,Transform);
			}
		}
	}
}