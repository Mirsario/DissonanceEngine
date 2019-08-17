using System;
using System.Linq;
using GameEngine;
using GameEngine.Graphics;
using GameEngine.Extensions;
using GameEngine.Physics;

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
		public Vector3 size = Vector3.One;
		public Vector3 prevVelocity;
		public MeshRenderer renderer;
		public CylinderCollider collider;
		public Rigidbody rigidbody;
		public AudioSource audioSource;
		public bool isSprinting;
		public bool onGroundCached;

		public bool OnGround => rigidbody.Collisions.Any(c => c.Contacts.Any(p => p.point.y>=Transform.Position.y-0.1f));
		public float MoveSpeed => (isSprinting && onGroundCached) ? 12f : 7.5f;
		public float Acceleration => 4.25f;

		public override Type CameraControllerType => Main.forceThirdPerson ? typeof(BasicThirdPersonCamera) : typeof(FirstPersonCamera);

		public override void OnInit()
		{
			size = new Vector3(1f,1.95f,1f); //JoJo height.

            renderer = AddComponent<MeshRenderer>();
            renderer.Mesh = Resources.Get<Mesh>("Robot.mesh");
            renderer.Material = Resources.Get<Material>("Robot.material");

            collider = AddComponent<CylinderCollider>(false);
			collider.Size = size;
			collider.Offset = new Vector3(0f,size.y/2f,0f);
			collider.Enabled = true;

			rigidbody = AddComponent<Rigidbody>();
			rigidbody.Mass = 1f;
			rigidbody.AngularFactor = Vector3.Zero;
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
				Vector3 direction = brain?.LookDirection ?? Vector3.Forward;
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

			if(PhysicsEngine.Raycast(camera.Transform.Position,camera.Transform.Forward,out var hit,customFilter:obj => Main.LocalEntity==obj ? false : (bool?)null)) {
                if(hit.gameObject is Entity entity && Input.GetMouseButtonDown(MouseButton.Middle)) {
                    Main.LocalEntity = entity;
                    screenFlash = 0.5f;
                    SoundInstance.Create($"Magic.ogg",entity.Transform.Position);
                }
                if(Input.GetKeyDown(Keys.X)) { //Teleport
                    Transform.Position = hit.point+Vector3.Up;
                }
                if(Input.GetKeyDown(Keys.J)) {
                    Instantiate<RaisingPlatform>(world,position: hit.point);
                }
                if(Input.GetKeyDown(Keys.V)) {
                    Instantiate<Robot>(world,position: hit.point);
                }
                if(Input.GetKey(Keys.B)) {
                    Instantiate<StoneHatchet>(world,position: hit.point+new Vector3(0f,15f,0f));
                }
                if(Input.GetKeyDown(Keys.N)) {
                    Instantiate<CubeObj>(world,position: hit.point+new Vector3(0f,5f,0f));
                }
                if(Input.GetKeyDown(Keys.M)) {
                    Instantiate<CubeObj2>(world,position: hit.point + new Vector3(0f,5f,0f));
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
			GUI.DrawText(new RectFloat(8,8+(i++*16),128,8),$"Player Position - {Transform.Position}");
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

			var forwardUnlimited = brain?.Transform.Forward ?? Vector3.Forward;
			var rightUnlimited = brain?.Transform.Right ?? Vector3.Right;
			var forward = forwardUnlimited;
			var right = rightUnlimited;
			forwardUnlimited.Normalize();
			forwardUnlimited.Normalize();
			forward.y = 0f;
			right.y = 0f;
			forward.Normalize();
			right.Normalize();

			var position = Transform.Position;

			bool inWater = position.y+1.5f<=world.GetWaterLevelAt(position);

			if(onGroundCached && forceAirMove<=0f && !inWater) {
				if(moveInput!=default) {
					walkTime += Time.FixedDeltaTime;

					if(walkTime>=(isSprinting ? 0.3f : 0.5f)) {
						Footstep("Walk",0.3f);
						walkTime = 0f;
					}
				} else {
					walkTime = 0f;
				}

				if(Movement_CheckJump()) {
					PlayVoiceClip(Resources.Get<AudioClip>("Jump.ogg"));
					forceAirMove = 0.1f;
					Movement_Move(AirAcceleration,MoveSpeed,Friction,forward,right);
				} else {
					Movement_Move(Acceleration,MoveSpeed,Friction,forward,right);

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
				}

				lastLand = Time.FixedDeltaTime;
				wasOnGround = true;
			} else {
				walkTime = 0f;

				forceAirMove = Mathf.StepTowards(forceAirMove,0f,Time.FixedDeltaTime);

				if(inWater) {
					Movement_Move(AirAcceleration,MoveSpeed,10f,forwardUnlimited,rightUnlimited);

					Movement_Acceleration(Vector3.Backward+Vector3.Left,4f,0.5f);
				} else {
					Movement_Move(AirAcceleration,MoveSpeed,Friction,forward,right);
				}

				wasOnGround = false;

				velocity.y += (inWater ? 2f : -18f)*Time.FixedDeltaTime;
			}

			rigidbody.Velocity = velocity;
			prevVelocity = velocity;
		}
		public void Movement_Move(float acceleration,float moveSpeed,float friction,Vector3 forward,Vector3 right)
		{
			Movement_Friction(friction);

			var wishDirection = (forward*moveInput.y)+(right*moveInput.x);

			Movement_Acceleration(wishDirection,moveSpeed,acceleration);
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
		public void Movement_Friction(float friction)
		{
			var tempVec = velocity;
			tempVec.y = 0f;
			float speed = tempVec.Magnitude;
			float drop = 0f;

			if(onGroundCached) {
				drop = (speed<StopSpeed ? StopSpeed : speed)*friction*Time.FixedDeltaTime;
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
			var providers = rigidbody.Collisions.SelectIgnoreNull(c => c.GameObject as IFootstepProvider ?? ((c.GameObject as IHasMaterial)?.GetMaterial(atPoint))).ToArray();
			if(providers.Length>0) {
				var provider = providers[0];
				provider.GetFootstepInfo(atPoint,out string surfaceType,ref actionType,out int numSoundVariants);
				SoundInstance.Create($"Footstep{surfaceType}{actionType}{(numSoundVariants>0 ? Rand.Range(1,numSoundVariants+1).ToString() : null)}.ogg",atPoint+(velocity*Time.DeltaTime),volume,Transform);
			}
		}
	}
}