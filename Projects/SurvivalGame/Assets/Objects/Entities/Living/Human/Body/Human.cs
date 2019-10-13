//#define STRAFE_JUMPING

using System;
using System.Linq;
using GameEngine;
using GameEngine.Graphics;
using GameEngine.Extensions;
using GameEngine.Physics;
using ImmersionFramework;

namespace SurvivalGame
{
	public class Human : LivingEntity
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
		public float walkIntensity;
		public float jumpPress;
		public float screenFlash;
		public Vector2 moveInput;
		public Vector3 size = Vector3.One;
		public AudioSource audioSource;
		public bool isSprinting;
		public bool onGround;
		public Vector3 groundNormal;

		public float MoveSpeed => (isSprinting && onGround) ? 12f : 7.5f;
		public float Acceleration => 4.25f;

		public override Type CameraControllerType => Main.forceThirdPerson ? typeof(BasicThirdPersonCamera) : typeof(FirstPersonCamera);

		public override void OnInit()
		{
			base.OnInit();

			size = new Vector3(1f,1.95f,1f); //JoJo height.

			audioSource = AddComponent<AudioSource>();
		}
		public override void SetupRenderers(ref Renderer[] renderers)
		{
			renderers = new[] {
				AddComponent<MeshRenderer>(c => {
					c.Mesh = Resources.Get<Mesh>("Robot.obj");
					c.Material = Resources.Get<Material>("Robot.material");
				})
			};
		}
		public override void SetupPhysicsComponents(ref Collider[] colliders,ref Rigidbody rigidbody)
		{
			colliders = new[] {
				AddComponent<CylinderCollider>(c => {
					c.Size = size;
					c.Offset = new Vector3(0f,size.y/2f,0f);
				})
			};

			rigidbody = AddComponent<Rigidbody>(c => {
				c.Mass = 1f;
				c.AngularFactor = Vector3.Zero;
				c.UseGravity = false;
				c.Friction = 0f;
				c.Drag = 0f;
			});
		}
		public override void FixedUpdate()
		{
			/*if(IsLocalPlayer) {
				renderer.Enabled = Main.forceThirdPerson;
			}*/

			PreMovement();

			moveInput = this.Value<Inputs.MoveX,Inputs.MoveY>();

			if(moveInput.x==0f && moveInput.y<=0f) {
				isSprinting = false;
			}else if(this.JustActivated<Inputs.Sprint>()) {
				isSprinting = true;
			}

			base.FixedUpdate();

			Transform.EulerRot = new Vector3(0f,Mathf.LerpAngle(Transform.EulerRot.y,LookRotation.y,Time.FixedDeltaTime*(onGround ? 12f : 4f)),0f);

			/*if(this.JustActivated<Inputs.PrimaryUse>()) {
				Vector3 direction = brain?.LookDirection ?? Vector3.Forward;
				Rocket rocket = Instantiate<Rocket>(world,position:camera.Transform.Position+direction);
				rocket.velocity = direction*25f;
				rocket.owner = this;
				SoundInstance.Create("Sounds/rocketFire.wav",camera.Transform.Position);
			}*/

			if(this.JustActivated<Inputs.Jump>()) {
				jumpPress = 0.25f;
			} else {
				jumpPress = Math.Max(0f,jumpPress-Time.FixedDeltaTime);
			}

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

			PostMovement();
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

#if STRAFE_JUMPING
			const string StrafeJumpingState = "Enabled";
#else
			const string StrafeJumpingState = "Disabled";
#endif

			int i = 5;
			GUI.DrawText(new RectFloat(8,8+(i++*16),128,8),$"Player Speed - XZ:{new Vector3(velocity.x,0f,velocity.z).Magnitude:0.00} Y: {velocity.y:0.00}");
			GUI.DrawText(new RectFloat(8,8+(i++*16),128,8),$"Player Position - {Transform.Position}");
			GUI.DrawText(new RectFloat(8,8+(i++*16),128,8),$"Quake 3 Acceleration: {StrafeJumpingState}");
			GUI.DrawText(new RectFloat(8,8+(i++*16),128,8),$"Input: {moveInput.x:0.00},{moveInput.y:0.00}");
			GUI.DrawText(new RectFloat(8,8+(i*16),128,8),$"Sprinting: {isSprinting}");

			if(screenFlash>0f) {
				GUI.DrawTexture(Screen.Rectangle,Main.whiteTexture,new Vector4(0.75f,0f,0f,screenFlash));
			}
		}
		public override void UpdateIsPlayer(bool isPlayer)
		{
			base.UpdateIsPlayer(isPlayer);

			/*if(!isPlayer) {
				renderer.Enabled = true;
			}*/

			audioSource.Is2D = isPlayer;
		}

		//Movement
		public void PreMovement()
		{
			velocity = rigidbody.Velocity;

			var firstColl = rigidbody.Collisions.FirstOrDefault(c => c.Contacts.Any(p => p.point.y>=Transform.Position.y-0.1f));

			float lowY = Transform.Position.y-0.1f;

			onGround = false;

			foreach(var collision in rigidbody.Collisions) {
				foreach(var contact in collision.Contacts) {
					if(contact.point.y>=lowY) {
						onGround = true;
						groundNormal = contact.normal;
						break;
					}
				}

				if(onGround) {
					break;
				}
			}
		}
		public void Movement()
		{
			var forwardUnlimited = Transform.Forward;
			var rightUnlimited = Transform.Right;
			var forward = forwardUnlimited;
			var right = rightUnlimited;
			forwardUnlimited.Normalize();
			forward.y = 0f;
			right.y = 0f;
			forward.Normalize();
			right.Normalize();

			bool inWater = false; //position.y+1.5f<=world.GetWaterLevelAt(position);

			void UpdateWalk(bool walking)
			{
				if(walking) {
					float prevWalkTime = walkTime;
					walkTime += Time.FixedDeltaTime;

					float repeatTime = isSprinting ? 0.3f : 0.5f;

					if(Mathf.Repeat(walkTime,repeatTime)<Mathf.Repeat(prevWalkTime,repeatTime)) {
						Footstep("Walk",0.3f);
					}

					walkIntensity = Mathf.StepTowards(walkIntensity,1f,Time.FixedDeltaTime);
				} else if(walkIntensity>0f) {
					walkIntensity -= Time.FixedDeltaTime*0.5f;

					if(walkIntensity<=0f) {
						walkTime = 0f;
						walkIntensity = 0f;
					}
				}
			}

			if(onGround && forceAirMove<=0f && !inWater) {
				UpdateWalk(moveInput!=default);

				if(Movement_CheckJump()) {
					PlayVoiceClip(Resources.Get<AudioClip>("Jump.ogg"));
					forceAirMove = 0.1f;
					Movement_Move(AirAcceleration,MoveSpeed,Friction,forward,right);
				} else {
					Movement_Move(Acceleration,MoveSpeed,Friction,forward,right);

					if(!wasOnGround) {
						if((Time.GameTime-lastLand)>=0.5f) {
							Footstep("Land",0.8f);
						}

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

				lastLand = Time.GameTime;
				wasOnGround = true;
			} else {
				UpdateWalk(false);

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
		}
		public void PostMovement()
		{
			rigidbody.Velocity = velocity;
			prevVelocity = velocity;
		}
		public void Movement_Move(float acceleration,float moveSpeed,float friction,Vector3 forward,Vector3 right)
		{
			Movement_Friction(friction);

			var wishDirection = (forward*moveInput.y)+(right*moveInput.x);

			if(wishDirection!=Vector3.Zero) {
				float angleDiff = Vector3.Angle(wishDirection,groundNormal)-90f;

				Debug.Log(angleDiff);
			}

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

			if(onGround) {
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
#if STRAFE_JUMPING
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
#else
			var wishVelocity = wishDirection*wishSpeed;
			var pushDirection = wishVelocity-velocity;
			pushDirection.Normalize(out float pushLength);

			float accelSpeed = acceleration*wishSpeed*Time.FixedDeltaTime;
			if(accelSpeed>pushLength) {
				accelSpeed = pushLength;
			}

			velocity += pushDirection*accelSpeed;
#endif
		}

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
			var providers = rigidbody.Collisions.SelectIgnoreNull(c => c.GameObject as IFootstepProvider ?? ((c.GameObject as IHasMaterial)?.GetMaterial(atPoint))).ToArray();

			if(providers.Length>0) {
				providers[0].GetFootstepInfo(atPoint,out string surfaceType,ref actionType,out int numSoundVariants);

				SoundInstance.Create($"Footstep{surfaceType}{actionType}{(numSoundVariants>0 ? Rand.Range(1,numSoundVariants+1).ToString() : null)}.ogg",atPoint+(velocity*Time.DeltaTime),volume,Transform,IsLocalPlayer);
			}
		}
	}
}