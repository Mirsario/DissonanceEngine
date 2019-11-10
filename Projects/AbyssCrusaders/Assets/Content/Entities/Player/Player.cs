using AbyssCrusaders.Content.Common.Effects;
using AbyssCrusaders.Content.Tiles.Building;
using AbyssCrusaders.Core;
using AbyssCrusaders.Core.Test;
using GameEngine;
using GameEngine.Graphics;

namespace AbyssCrusaders.Content.Entities
{
	public partial class Player : PhysicalEntity
	{
		public const int FrameWidth = 40;
		public const int FrameHeight = 40;

		public Sprite sprite;
		public ushort currentTile;
		public float defaultWidth;
		public float defaultHeight;
		public Vector2 moveInput;
		public int animationFrame;
		public int animationFrameCount;

		protected float rotationOffset;

		private Light2D light;
		private sbyte direction = 1;
		private float jumpKeyBuffer;
		private float lastFootstepTime;
		private float lastDigTime;

		public int Direction {
			get => direction;
			set {
				direction = (sbyte)value;
				sprite.spriteEffects = direction==-1 ? Sprite.SpriteEffects.FlipHorizontally : 0;
				//Transform.EulerRot = new Vector3(value==1 ? 0f : 180f,0f,rotation);
			}
		}

		public override void OnInit()
		{
			base.OnInit();

			defaultWidth = width = Utils.MetersToUnits(0.75f);
			defaultHeight = height = Utils.MetersToUnits(1.7f);
			friction = 9f;
			airFriction = 0.75f;
			stopSpeed = 0.1f;
			maxSpeed = 12f;

			animationFrameCount = 18;

			currentTile = TilePreset.GetTypeId<Wood>();

			sprite = AddComponent<Sprite>(c => {
				c.Material = Resources.Find<Material>("Player").Clone();
				c.FrameSize = new Vector2(FrameWidth,FrameHeight);
			});

			UpdateSprites();

			light = AddComponent<Light2D>(c => {
				c.color = new Vector3(1f,0.75f,0.25f);
				c.range = 10f;
			});

			Instantiate<CursorSoundObj>();
		}
		public override void FixedUpdate()
		{
			moveInput = new Vector2(
				GameInput.moveX.Value,
				GameInput.moveY.Value
			);

			UpdateDodgerolls(false);

			if(moveInput.x!=0f) {
				Direction = moveInput.x>0f ? 1 : -1;

				const float Acceleration = 2.5f;
				const float AirAcceleration = 1f;

				bool slowWalk = GameInput.walk.IsPressed;
				float usedMaxSpeed = slowWalk ? maxSpeed*0.5f : maxSpeed;
				float acceleration = collisions.down ? Acceleration : AirAcceleration;

				ApplyAcceleration(new Vector2(moveInput.x*acceleration,0f),usedMaxSpeed);
			}else{
				ApplyFriction();
			}
			
			if(GameInput.jump.JustPressed) {
				jumpKeyBuffer = 0.12f;
			}else{
				jumpKeyBuffer -= Time.FixedDeltaTime;
			}

			if(collisions.down) {
				if(jumpKeyBuffer>0f) {
					velocity.y = -26f;

					jumpKeyBuffer = 0f;

					Debug.Log("Jump");

					Footstep("Run",0.4f);

					lastFootstepTime = Time.GameTime;
				} else if(!collisionsOld.down && prevVelocity.y>10f) {
					Footstep("Land",0.6f);

					lastFootstepTime = Time.GameTime;
				} else if(moveInput.x!=0f) {
					float currentTime = Time.GameTime;

					if(currentTime-lastFootstepTime>0.25f) {
						Footstep(GameInput.walk.IsPressed ? "Walk" : "Run",0.4f);

						lastFootstepTime = Time.GameTime;
					}
				}
			}

			if(Input.GetKeyDown(Keys.Z)) {
				currentTile--;

				if(currentTile==ushort.MaxValue) {
					currentTile = (ushort)(TilePreset.typeCount-1);
				}
			}else if(Input.GetKeyDown(Keys.X)) {
				currentTile++;

				if(currentTile>=TilePreset.typeCount) {
					currentTile = 0;
				}
			}

			if(Input.GetKeyDown(Keys.U)) {
				Instantiate<Cloud>(position:Main.camera.mousePosition);
			}
			if(Input.GetKeyDown(Keys.I)) {
				var pos = new Vector2Int((int)Position.x,(int)Position.y+2);

				world.PlaceTileEntity<TileEntities.Nature.Trees.Spruce>(pos.x,pos.y);
			}
			if(Input.GetKeyDown(Keys.O)) {
				var pos = new Vector2Int((int)Position.x,(int)Position.y-1);

				world.PlaceTileEntity<TileEntities.Crafting.RepairTable>(pos.x,pos.y);
			}

			if(Input.GetKeyDown(Keys.G)) {
				light.Enabled = !light.Enabled;
			}

			bool primaryUse = GameInput.primaryUse.IsPressed;
			bool secondaryUse = GameInput.secondaryUse.IsPressed;
			bool teleport = GameInput.mmb.JustPressed;
			bool placeLight = Input.GetKeyDown(Keys.E);
			bool damageTiles = Input.GetKey(Keys.R);

			if(primaryUse || secondaryUse || teleport || placeLight) {
				Vector2 mouseWorld = Main.camera.mousePosition;
				Vector2Int mouseTile = (Vector2Int)mouseWorld;
				bool control = Input.GetKey(Keys.LControl);

				if(primaryUse) {
					if(damageTiles) {
						if(Time.GameTime-lastDigTime>0.333f) {
							world.DamageTile(mouseTile.x,mouseTile.y,128);

							lastDigTime = Time.GameTime;
						}
					}else if(control) {
						world.RemoveTile(mouseTile.x,mouseTile.y);
					}else{
						world.PlaceTile(mouseTile.x,mouseTile.y,currentTile);
					}
				}else if(secondaryUse) {
					if(control) {
						world.RemoveWall(mouseTile.x,mouseTile.y);
					}else{
						world.PlaceWall(mouseTile.x,mouseTile.y,currentTile);
					}
				}else if(teleport) {
					Position = mouseWorld;
					velocity = Vector2.Zero;

					SoundInstance.Create("Audio/Teleport.ogg",Position,0.25f,is2D:true);
				} else if(placeLight) {
					Instantiate<LightObj>(position:mouseWorld);
				}
			}

			if(Input.GetKeyDown(Keys.Plus)) {
				light.range += 2f;
			} else if(Input.GetKeyDown(Keys.Minus)) {
				light.range -= 2f;
			}

			dropDownSlopes = moveInput.y>0f;
			autoStepUp = moveInput.y<=0f;

			base.FixedUpdate();

			width = defaultWidth;
			height = defaultHeight;
			autoStepDown = true;
		}
		public override void RenderUpdate()
		{
			rotationOffset = 0f;

			UpdateDodgerolls(true);

			rotationOffset += Mathf.Clamp(velocity.x*0.5f,-5f,5f);

			Rotation = rotationOffset;

			if(tempSpriteOffset!=default) {
				tempSpriteOffset = Vector2.StepTowards(tempSpriteOffset,default,Time.RenderDeltaTime*Mathf.Max(8f,Mathf.Abs(velocity.x)));
			}

			UpdateSprites();
		}
		public override void OnGUI()
		{
			float y = 6.5f;

			Vector2 pos = Position;

			GUI.DrawText(new RectFloat(8,y++*16,128,8),$"Player Position: {pos.x}, {pos.y}");
			GUI.DrawText(new RectFloat(8,y++*16,128,8),$"Player Velocity: {velocity.x}, {velocity.y}");

			var texture = TilePreset.byId[currentTile].Texture;
			if(texture!=null) {
				float size = Main.UnitSizeInPixels*16f;
				GUI.DrawTexture(new RectFloat(8,Screen.Height-8-size,size,size),texture);
			}
		}

		public void UpdateSprites()
		{
			if(isDodging) {
				animationFrame = Mathf.Min(17,5+Mathf.FloorToInt(Mathf.Min(1f,dodgerollProgress)*14f));
			} else if(!collisions.down) {
				animationFrame = velocity.y<0f ? 2 : 3;
			} else if(GameInput.moveX.Value!=0f || !collisions.down) {
				animationFrame = 1+(int)Time.FixedUpdateCount/5%3;
			} else {
				animationFrame = GameInput.moveY.Value<0f ? 17 : (GameInput.moveY.Value>0f ? 15 : 0);
			}

			sprite.SourceRectangle = new RectFloat(
				animationFrame/(float)animationFrameCount,
				0f,
				1f/(float)animationFrameCount,
				1f
			);

			var usedOffset = -tempSpriteOffset*Main.UnitSizeInPixels;
			usedOffset.y += (FrameHeight-(height*Main.UnitSizeInPixels))*0.5f;

			sprite.Origin = new Vector2(
				0.5f+usedOffset.x/FrameWidth,
				0.5f+usedOffset.y/FrameHeight
			);
		}

		public void Footstep(string actionType,float volume = 1f)
		{
			if(!collisions.down) {
				return;
			}

			var position = Position;

			var stepPosition = new Vector2(position.x,position.y+height*0.5f);

			int left = (int)(position.x-width*0.5f);
			int right = (int)(position.x+width*0.5f);
			int bottom = (int)(position.y+height*0.5f);

			for(int x = left;x<=right;x++) {
				for(int y = bottom;y<=bottom+1;y++) {
					var tile = world[x,y];

					if(tile.type==0) {
						continue;
					}

					var tilePreset = TilePreset.byId[tile.type];
					if(!(tilePreset is IHasMaterial materialProvaider)) {
						continue;
					}

					var material = materialProvaider.GetMaterial(stepPosition);
					if(material==null) {
						continue;
					}

					material.GetFootstepInfo(stepPosition,out string surfaceType,ref actionType,out int numSoundVariants);

					SoundInstance.Create($"Footstep{surfaceType}{actionType}{(numSoundVariants>0 ? Rand.Range(1,numSoundVariants+1).ToString() : null)}.ogg",Position,volume);

					return;
				}
			}
		}
	}
}