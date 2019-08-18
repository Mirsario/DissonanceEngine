using GameEngine;
using GameEngine.Graphics;

namespace AbyssCrusaders
{
	public class Player : PhysicalEntity
	{
		public SpriteObject spriteObj;
		public float jumpKeyBuffer;
		public ushort currentTile;
		
		private sbyte direction;
		public int Direction {
			get => direction;
			set {
				direction = (sbyte)value;
				spriteObj.sprite.spriteEffects = direction==-1 ? Sprite.SpriteEffects.FlipHorizontally : 0;
				//Transform.EulerRot = new Vector3(value==1 ? 0f : 180f,0f,rotation);
			}
		}

		public override void OnInit()
		{
			base.OnInit();
			
			width = Utils.MetersToUnits(0.75f);
			height = Utils.MetersToUnits(1.8f);
			friction = 7f;
			airFriction = 0.5f;
			stopSpeed = 0.1f;
			maxSpeed = 12f;

			currentTile = TilePreset.GetTypeId<Tiles.Wood>();

			var texture = Resources.Get<Texture>("Player.png");

			spriteObj = Instantiate<SpriteObject>();
			spriteObj.sprite.Material = new Material("Player",Resources.Find<Shader>("Game/SpriteColor"));
			spriteObj.sprite.Material.SetTexture("mainTex",texture);
			spriteObj.Transform.parent = Transform;
			spriteObj.Transform.LocalScale = new Vector3(texture.Width*Main.PixelSizeInUnits,texture.Height*Main.PixelSizeInUnits,1f);
			spriteObj.Position = Vector2.Zero;

			var light = AddComponent<Light2D>(c => {
				c.color = new Vector3(1f,0.75f,0.5f);
				c.range = 32f;
			});

			Instantiate<CursorLightObj>().light.color = Vector3.One;
		}
		public override void FixedUpdate()
		{
			float moveX = GameInput.moveX.Value;
			float moveY = GameInput.moveY.Value;

			if(moveX!=0f) {
				Direction = moveX>0f ? 1 : -1;
				if(collisions.down) {
					if(GameInput.sprint.IsPressed) {
						ApplyAcceleration(new Vector2(moveX*5f,0f),maxSpeed*1.5f);
					}else{
						ApplyAcceleration(new Vector2(moveX*5f,0f));
					}
				}else{
					ApplyAcceleration(new Vector2(moveX*2.5f,0f));
				}
			}else{
				ApplyFriction();
			}
			
			if(GameInput.jump.JustPressed) {
				jumpKeyBuffer = 0.12f;
			}else{
				jumpKeyBuffer -= Time.FixedDeltaTime;
			}

			if(collisions.down && jumpKeyBuffer>0f) {
				velocity.y = -26f;
				if(moveX!=0f) {
					velocity.x += moveX*2f;
				}
				jumpKeyBuffer = 0f;
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

			bool primaryUse = GameInput.primaryUse.IsPressed;
			bool secondaryUse = GameInput.secondaryUse.IsPressed;
			bool teleport = GameInput.mmb.JustPressed;

			if(primaryUse || secondaryUse || teleport) {
				Vector2 mouseWorld = Main.camera.mousePosition;
				Vector2Int mouseTile = (Vector2Int)mouseWorld;
				bool control = Input.GetKey(Keys.LControl);

				if(primaryUse) {
					if(control) {
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
					//Position = mouseWorld;
					Instantiate<LightObj>(position:mouseWorld);
				}
			}

			Rotation = Mathf.Clamp(velocity.x*0.5f,-5f,5f);

			dropDownSlopes = moveY>0f;
			autoClimb = moveY<=0f;

			base.FixedUpdate();
		}
		public override void RenderUpdate()
		{
			if(tempSpriteOffset!=default) {
				tempSpriteOffset = Vector2.StepTowards(tempSpriteOffset,default,Time.RenderDeltaTime*Mathf.Max(8f,Mathf.Abs(velocity.x)));
			}
			spriteObj.Position = tempSpriteOffset;
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
	}
}