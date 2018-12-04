using System;
using System.Linq;
using System.Collections.Generic;
using GameEngine;

namespace Game
{
	public class Human : Mob
	{
		//TODO: Make this not bad.

		public const float airAcceleration = 0.4f;
		public const float jumpSpeed = 7.5f;
		public const float stopSpeed = 1f;
		public const float friction = 4.5f;
		
		public bool wasOnGround;
		public float forceAirMove;
		public float lastLand;
		public float walkTime;
		public float jumpPress;
		public bool enableStrafeJumping;
		public AudioSource audioSource;
		public float screenFlash;

		public bool IsSprinting => Input.GetKey(Keys.LShift);
		public float MoveSpeed => IsSprinting ? 12f : 7.5f;
		public float Acceleration => 4.25f;

		public override Type CameraControllerType => typeof(FirstPersonCamera);

		public override void OnInit()
		{
			size = new Vector3(1f,1.95f,1f); //JoJo height.

			base.OnInit();

			rigidbody.UseGravity = false;
			rigidbody.Friction = 0f;
			rigidbody.Drag = 0f;

			//var box = AddComponent<MeshRenderer>();
			//box.Mesh = PrimitiveMeshes.Cube;
			//box.Material = Resources.Get<Material>("TestCube.material");

			audioSource = AddComponent<AudioSource>();
		}
		public override void FixedUpdate()
		{
			var camera = Main.camera;
			if(Input.GetKeyDown(Keys.C)) {	//Sound test
				Debug.Log("HONK!");
				PlayVoiceClip(Resources.Get<AudioClip>("Sounds/honk.wav"));
			}
			if(Input.GetKeyDown(Keys.K)) {  //Add light
				Instantiate<LightObj>(world,position:camera.Transform.Position);
			}

			base.FixedUpdate();
			if(Input.GetMouseButtonDown(0)) {
				Rocket rocket = Instantiate<Rocket>(world,position:camera.Transform.Position+camera.Transform.Forward);
				rocket.velocity = camera.Transform.Forward*25f;
				rocket.owner = this;
				SoundInstance.Create("Sounds/rocketFire.wav",camera.Transform.Position);
			}

			if(Input.GetKeyDown(Keys.Space) || Input.GetMouseButton(1)) {
				jumpPress = 0.25f;
			} else {
				jumpPress = Math.Max(0f,jumpPress-Time.DeltaTime);
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

			if(Physics.Raycast(camera.Transform.Position,camera.Transform.Forward,out var hit,customFilter:obj => obj==this ? false : (bool?)null)) {
				if(hit.gameObject is Entity entity && Input.GetMouseButtonDown(2)) {
					Main.LocalEntity = entity;
					screenFlash = 0.5f;
					SoundInstance.Create($"Magic.ogg",entity.Transform.Position);
				}
				if(Input.GetKeyDown(Keys.X)) { //Teleport
					Transform.Position = hit.point+Vector3.up;
				}
				if(Input.GetKeyDown(Keys.J)) {
					Instantiate<RaisingPlatform>(world,position:hit.point);
				}
				if(Input.GetKeyDown(Keys.V)) {
					Instantiate<Robot>(world,position:hit.point);
				}
				if(Input.GetKeyDown(Keys.B)) {
					Instantiate<StoneHatchet>(world,position:hit.point+new Vector3(0f,15f,0f));
				}
				if(Input.GetKeyDown(Keys.N)) {
					Instantiate<TexTest>(world,position:hit.point+new Vector3(0f,15f,0f));
				}
				if(Input.GetKeyDown(Keys.U)) {
					enableStrafeJumping = !enableStrafeJumping;
				}
			}
		}
		public override void RenderUpdate()
		{
			base.RenderUpdate();
			if(screenFlash>0f) {
				screenFlash = Mathf.StepTowards(screenFlash,0f,Time.DeltaTime);
			}
		}
		public override void OnGUI()
		{
			int i = 5;
			GUI.DrawText(new Rect(8,8+(i++*16),128,8),$"Player Speed - XZ:{new Vector3(velocity.x,0f,velocity.z).Magnitude.ToString("0.00")} Y: {velocity.y.ToString("0.00")}");
			GUI.DrawText(new Rect(8,8+(i*16),128,8),$"Quake 3 Acceleration: {(enableStrafeJumping ? "Enabled" : "Disabled")} ([U] - Toggle)");

			if(screenFlash>0f) {
				GUI.DrawTexture(Graphics.ScreenRect,Main.whiteTexture,new Vector4(0.75f,0f,0f,screenFlash));
			}
			//GUI.DrawTexture(new Rect(32,32,64,64),TileEntity.tileTexture);	//This was... weirdly moving???
		}

		public override void UpdateIsPlayer(bool isPlayer)
		{
			/*Camera cameraTop = new GameObject().AddComponent<Camera>();
			cameraTop.view = new Rect(0.5f,0.5f,0.5f,0.5f);
			cameraTop.transform.position = transform.position+(Vector3.up*128f);
			cameraTop.transform.eulerRot = new Vector3(-90f,0f,0f);
			cameraTop.transform.parent = transform;
			Debug.Log("spawned new camera");*/
		}

		#region Movement
		public void Movement()
		{
			velocity = rigidbody.Velocity;

			//var direction = ((Transform.Right*Main.MoveInput.x)+(Transform.Forward*Main.MoveInput.y)).RotatedBy(0f,Main.camera.Transform.EulerRot.y,0f).Normalized;

			if(OnGround && forceAirMove<=0f) {
				if(Input.GetDirection(Keys.W,Keys.S,Keys.A,Keys.D)!=Vector2.zero) {
					walkTime += Time.DeltaTime;
					if(walkTime>=(IsSprinting ? 0.3f : 0.5f)) {
						Footstep("Walk",0.3f);
						walkTime = 0f;
					}
				} else {
					walkTime = 0f;
				}

				Movement_WalkMove();

				if(!wasOnGround) {
					Footstep("Land",0.8f);
					float magnitude = prevVelocity.Magnitude-velocity.Magnitude;
					const float minSpeed = 8.5f;
					if(magnitude>minSpeed) {
						float power = Mathf.Lerp(0f,1f,(magnitude-minSpeed)*0.2f);
						screenFlash = power*0.5f;
						SoundInstance.Create($"FallBig{Rand.Range(1,6)}.ogg",Transform.Position+(velocity*Time.DeltaTime),power);
						screenShake += power*0.4f;
					}
				}

				lastLand = Time.DeltaTime;
				wasOnGround = true;
			} else {
				walkTime = 0f;

				if(forceAirMove>0f) {
					forceAirMove = Math.Max(0f,forceAirMove-Time.DeltaTime);
				}

				Movement_AirMove();
				wasOnGround = false;
			}

			velocity.y -= 18f*Time.DeltaTime; //Temporary implementation, doing gravity through the physics engine would probably be better. 
			rigidbody.Velocity = velocity;
			prevVelocity = velocity;
		}
		public void Movement_WalkMove()
		{
			if(Movement_CheckJump()) {
				//PlayVoiceClip(Resources.Get<AudioClip>("Jump.ogg"));
				//SoundInstance.Create("Land.ogg",transform.position);
				Movement_AirMove();
				forceAirMove = 0.1f;
				return;
			}

			Movement_Friction();

			var moveInput = Input.GetDirection(Keys.W,Keys.S,Keys.A,Keys.D);
			var cameraTransform = Main.camera.Transform;
			var forward = cameraTransform.Forward;
			var right = cameraTransform.Right;
			forward.y = 0f;
			right.y = 0f;
			forward.Normalize();
			right.Normalize();

			var wishDirection = (forward*moveInput.y)+(right*moveInput.x);
			float wishSpeed = MoveSpeed;
			float acceleration = Acceleration;

			Movement_Acceleration(wishDirection,wishSpeed,acceleration);
		}
		public void Movement_AirMove()
		{
			Movement_Friction();

			var moveInput = Input.GetDirection(Keys.W,Keys.S,Keys.A,Keys.D);
			var cameraTransform = Main.camera.Transform;
			var forward = cameraTransform.Forward;
			var right = cameraTransform.Right;
			forward.y = 0f;
			right.y = 0f;
			forward.Normalize();
			right.Normalize();

			var wishDirection = (forward*moveInput.y)+(right*moveInput.x);
			float wishSpeed = MoveSpeed;
			float acceleration = airAcceleration;

			Movement_Acceleration(wishDirection,wishSpeed,acceleration);
		}
		public bool Movement_CheckJump()
		{
			if(jumpPress<=0f) {
				return false;
			}
			if(velocity.y<0f) {
				velocity.y = jumpSpeed;
			}else{
				velocity.y += jumpSpeed;
			}
			return true;
		}
		public void Movement_Friction()
		{
			var tempVec = velocity;
			tempVec.y = 0f;
			float speed = tempVec.Magnitude;
			float drop = 0f;
			if(OnGround) {
				drop = (speed<stopSpeed ? stopSpeed : speed)*friction*Time.DeltaTime;
			}
			float newSpeed = speed-drop;
			if(newSpeed<0) {
				newSpeed = 0;
			}
			if(newSpeed!=0f) {
				newSpeed /= speed;
			}
			//velocity *= newSpeed;
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
				float accelSpeed = acceleration*wishSpeed*Time.DeltaTime;
				if(accelSpeed>addSpeed) {
					accelSpeed = addSpeed;
				}
				velocity += wishDirection*accelSpeed;
			}else{
				var wishVelocity = wishDirection*wishSpeed;
				var pushDirection = wishVelocity-velocity;
				pushDirection.Normalize(out float pushLength);
				float accelSpeed = acceleration*wishSpeed*Time.DeltaTime;
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

		//No strafejumping acceleration
		/*//proper way (avoids strafe jump maxspeed bug),but feels bad
		vec3_t		wishVelocity;
		vec3_t		pushDir;
		float		pushLen;
		float		canPush;
		VectorScale( wishdir,wishspeed,wishVelocity );
		VectorSubtract( wishVelocity,pm->ps->velocity,pushDir );
		pushLen = VectorNormalize( pushDir );
		canPush = accel*pml.frametime*wishspeed;
		if (canPush > pushLen) {
			canPush = pushLen;
		}
		VectorMA( pm->ps->velocity,canPush,pushDir,pm->ps->velocity );*/
	}
}