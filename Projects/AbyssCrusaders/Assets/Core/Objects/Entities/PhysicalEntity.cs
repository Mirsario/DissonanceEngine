using System;
using AbyssCrusaders.Utilities;
using GameEngine;

namespace AbyssCrusaders.Core
{
	public class PhysicalEntity : Entity
	{
		public Vector2 velocity;
		public Vector2 prevVelocity;
		public CollisionInfo collisions;
		public CollisionInfo collisionsOld;

		public float width = 1;
		public float height = 1;
		public float friction = 2f;
		public float airFriction = 1f;
		public float stopSpeed = 0.1f;
		public float maxSpeed = 10f;
		public float gravity = 20f;
		public bool dropDownSlopes;
		public bool autoStepUp = true;
		public bool autoStepDown = true;
		public Vector2 tempSpriteOffset;

		public Vector2 Size => new Vector2(width,height);
		public Vector2 SizeHalf => new Vector2(width*0.5f,height*0.5f);

		public override void FixedUpdate()
		{
			base.FixedUpdate();

			prevVelocity = velocity;

			velocity.y += gravity*Time.FixedDeltaTime;

			Position = PhysicsStep(Position,ref velocity);
		}

		protected Vector2 PhysicsStep(Vector2 position,ref Vector2 velocity)
		{
			Vector2 deltaVelocity = velocity*Time.FixedDeltaTime;

			int ceilWidth = Mathf.CeilToInt(width);
			int ceilHeight = Mathf.CeilToInt(height);

			float widthHalf = width*0.5f;
			float heightHalf = height*0.5f;

			int ceilHalfWidth = Mathf.CeilToInt(widthHalf);
			//int ceilHalfHeight = Mathf.CeilToInt(heightHalf);

			float left = position.x-widthHalf;
			float right = position.x+widthHalf;
			float top = position.y-heightHalf;
			float bottom = position.y+heightHalf;

			float leftNext = left+deltaVelocity.x;
			float rightNext = right+deltaVelocity.x;
			float topNext = top+deltaVelocity.y;
			float bottomNext = bottom+deltaVelocity.y;

			int x1 = world.ClampX(Mathf.FloorToInt(leftNext));
			int x2 = world.ClampX(Mathf.FloorToInt(rightNext));
			int y1 = world.ClampY(Mathf.FloorToInt(topNext));
			int y2 = world.ClampY(Mathf.FloorToInt(bottomNext));

			collisionsOld = collisions;
			collisions = default;

			for(int y = y1;y<=y2;y++) {
				for(int x = x1;x<=x2;x++) {
					ref var tile = ref world[x,y];

					if(tile.type==0) {
						continue;
					}

					var preset = tile.TilePreset;

					//Step-up
					bool TryClimb(int direction,ref Vector2 vel)
					{
						if(!autoStepUp || !preset.collision.up || bottom<y || bottom>y+1) { //vel.y<0f
							return false;
						}

						int xx1 = direction>0 ? x-ceilWidth : x;    //direction>0 ? x : x-ceilWidth-1;
						int yy1 = y-ceilHeight;                     //y-ceilHeight-1;
						int xx2 = xx1+ceilWidth;                    //xx1+ceilWidth;
						int yy2 = yy1+ceilHeight-1;                 //yy1+ceilHeight;

						for(int yy = yy1;yy<=yy2;yy++) {
							for(int xx = xx1;xx<=xx2;xx++) {
								var checkTile = world[xx,yy];

								if(checkTile.type>0 && checkTile.TilePreset.collision.down) {
									return false;
								}
							}
						}

						vel.y = Math.Min(vel.y,0f);
						deltaVelocity.y = Math.Min(deltaVelocity.y,0f);

						position.x = direction>0 ? x-widthHalf : x+1+widthHalf;
						position.y = y-heightHalf;

						float yOffset = 1f;
						if(Mathf.Abs(yOffset)>Mathf.Abs(tempSpriteOffset.y)) {
							tempSpriteOffset.y = yOffset;
						}

						collisions.down = true;

						return true;
					}

					//Vertical collisions
					if((preset.collision.up || preset.collision.down) && right>x && left<x+1) {
						if(preset.collision.up && (!dropDownSlopes || !preset.allowDroppingThrough) && velocity.y>0f && bottom<=y && bottomNext>y && bottomNext<y+1) {
							velocity.y = deltaVelocity.y = 0f;
							position.y = y-heightHalf;

							collisions.down = true;
						} else if(preset.collision.down && velocity.y<0f && top>=y+1 && topNext<y+1 && topNext>y) {
							velocity.y = deltaVelocity.y = 0f;
							position.y = y+1+heightHalf;

							collisions.up = true;
						}
					}

					//Horizontal collisions
					if((preset.collision.left || preset.collision.right) && bottom>y && top<y+1) {
						if(preset.collision.left && velocity.x>0f && right<=x && rightNext>x && rightNext<x+1) {
							if(!TryClimb(1,ref velocity)) {
								velocity.x = deltaVelocity.x = 0f;
								position.x = x-widthHalf;

								collisions.right = true;
							}
						} else if(preset.collision.right && velocity.x<0f && left>=x+1 && leftNext<x+1 && leftNext>x) {
							if(!TryClimb(-1,ref velocity)) {
								velocity.x = deltaVelocity.x = 0f;
								position.x = x+1+widthHalf;

								collisions.left = true;
							}
						}
					}
				}
			}

			position += deltaVelocity;

			//Step-down
			if(autoStepDown && deltaVelocity.x!=0f && deltaVelocity.y>=0f) {
				int direction = Math.Sign(deltaVelocity.x);

				bool goingRight = direction>0;

				int x = (int)(position.x-widthHalf);
				int y = (int)(position.y-heightHalf);

				if(goingRight ? x>(int)left : (int)(position.x+widthHalf)<(int)right) {
					int xx1 = x;
					int yy1 = y+1;
					int xx2 = xx1+ceilHalfWidth;
					int yy2 = yy1+ceilHeight-1;

					var stepTile = world[goingRight ? xx1 : xx1+1,yy2+1];

					if(stepTile.type>0 && stepTile.TilePreset.collision.up) {
						bool hasSpace = true;

						for(int yy = yy1;yy<=yy2;yy++) {
							for(int xx = xx1;xx<=xx2;xx++) {
								ref var checkTile = ref world[xx,yy];

								if(checkTile.type>0 && checkTile.TilePreset.collision.up) {
									hasSpace = false;
									break;
								}
							}
						}

						if(hasSpace) {
							position.y = Mathf.Floor(position.y+heightHalf)+1f-heightHalf;

							float yOffset = -1f;
							if(Mathf.Abs(yOffset)>Mathf.Abs(tempSpriteOffset.y)) {
								tempSpriteOffset.y = yOffset;
							}

							velocity.y = Math.Max(velocity.y,0f);
							deltaVelocity.y = Math.Max(deltaVelocity.y,0f);

							collisions.down = true;
						}
					}
				}
			}

			position.x = Mathf.Repeat(position.x,world.width);

			return position;
		}
		protected void ApplyFriction()
		{
			float xSpeed = Mathf.Abs(velocity.x);
			float drop = (xSpeed<stopSpeed ? stopSpeed : xSpeed)*(collisions.down ? friction : airFriction)*Time.FixedDeltaTime;
			float newSpeed = xSpeed-drop;

			if(newSpeed<0f) {
				newSpeed = 0f;
			}

			if(newSpeed!=0f) {
				newSpeed /= xSpeed;
			}

			velocity.x *= newSpeed;
		}
		protected void ApplyAcceleration(Vector2 acceleration,float? maxSpeed = null)
		{
			if(acceleration.x==0f && acceleration.y==0f) {
				return;
			}

			float speed = acceleration.Magnitude;
			Vector2 direction = acceleration.Normalized;
			float maxSpeedValue = maxSpeed ?? this.maxSpeed;
			float currentSpeed = Vector2.Dot(velocity,direction);
			float addSpeed = maxSpeedValue-currentSpeed;

			if(addSpeed<=0f) {
				return;
			}

			float accelSpeed = speed*maxSpeedValue*Time.FixedDeltaTime;
			if(accelSpeed>addSpeed) {
				accelSpeed = addSpeed;
			}

			velocity += direction*accelSpeed;
		}
	}
}
