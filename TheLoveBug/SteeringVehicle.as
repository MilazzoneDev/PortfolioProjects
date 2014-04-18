//Carl Milazzo
package  
{
	public class SteeringVehicle extends VectorTurtle
	{
		protected var _maxSpeed: Number = 100;
		protected var _maxForce: Number = 150;
		protected var _mass: Number = 1.0;
		protected var _radius:Number = 0;
	
		public function SteeringVehicle(aMan:Manager , aX:Number = 0, 
								aY:Number = 0, aSpeed:Number = 0) 
		{
			super(aMan, aX, aY);
			// initialize velocity to zero so movement results from applied force
			_maxSpeed = aSpeed;
			_velocity = new Vector2( );
		}
		
		public function set maxSpeed(s:Number)		{_maxSpeed = s;	}
		public function set maxForce(f:Number)		{_maxForce = f;	}
		public function get maxSpeed( )		{ return _maxSpeed;	}
		public function get maxForce( )		{ return _maxForce; }
		//public static function set maxSpeed(s:Number)		{_maxSpeed = s;	}
		//public static function set maxForce(f:Number)		{_maxForce = f;	}
		//public static function get maxSpeed( )		{ return _maxSpeed;	}
		//public static function get maxForce( )		{ return _maxForce; }
		public function get right( )		{ return fwd.perpRight( ); }
		public function get radius():Number
		{
			if ((_radius == 0))
			{
				var rot:Number = rotation;
				rotation = 0;
				_radius = Math.sqrt(((width * width) + height * height)) / 2;
				rotation = rot;
			}
			return _radius;
		}



		override public function update(dt:Number): void
		{
			//call calcSteeringForce (override in subclass) to get steering force
			var steeringForce:Vector2 = calcSteeringForce( ); 
			
			// clamp steering force to max force
			clampSteeringForce(steeringForce);
	
			// calculate acceleration: force/mass
			var acceleration:Vector2 = Vector2.divide(steeringForce, _mass);
			// add acceleration for time step to velocity
			_velocity.plusEquals(Vector2.multiply(acceleration, dt));
			//_velocity = _velocity.plusEquals(acceleration.timesEquals(dt));
			// update speed to reflect new velocity
			_speed = _velocity.magnitude( );
			// update fwd to reflect new velocity 
			fwd = _velocity;
			// clamp speed and velocity to max speed
			if (_speed > _maxSpeed)
			{
				_speed = _maxSpeed;
				_velocity = Vector2.multiply(fwd, _speed);
			}
			// call move with velocity adjusted for time step
			move( Vector2.multiply(_velocity, dt));
			
			leavePheromones();
			
		}
		
		protected function leavePheromones()
		{
			
			
		}
		
				
		protected function calcSteeringForce( ):Vector2
		{
			var steeringForce : Vector2 = new Vector2( );
			
			// override this function in subclasses by adding steering forces
			// using syntax like below (assuming target is a position vector)
			
			// steeringForce.plusEquals(seek(target));
			// steeringForce.plusEquals(stayOnStage( ));
			
			// multiple steering forces can be added to produce complex behavior
			
			return steeringForce;
		}
		
		protected function drawing()
		{
			
		}
			
		private function clampSteeringForce(force: Vector2 ): void
		{
			var mag:Number = force.magnitude();
			if(mag > _maxForce)
			{
				force.divideEquals(mag);
				force.timesEquals(_maxForce);
			}
		}
		
			
		protected function seek(targPos : Vector2) : Vector2
		{
			// set desiredVelocity equal to a vector from position to targPos
			var desVel:Vector2;
			var steeringForce:Vector2;
			desVel = Vector2.subtract(targPos, position);
			
			// scale desired velocity so its magnitude equals max speed
			desVel = desVel.normalized( ).timesEquals(maxSpeed);
			
			
			// to get steerinForce subtract current velocity from desired velocity
			steeringForce = Vector2.subtract(desVel, velocity)
	
			return steeringForce;
		}
		
		protected function flee(targPos : Vector2) : Vector2
		{
			// Set desiredVelocity equal to a vector AWAY from targPos
			// otherwise the function should be the same as seek 
			var desVel:Vector2;
			var steeringForce:Vector2;
			desVel = Vector2.subtract(position, targPos);
			
			desVel = desVel.normalized( ).timesEquals(maxSpeed);
			
			steeringForce = Vector2.subtract(desVel, velocity);
	
			return steeringForce;
		}
		
		protected function avoid(obstaclePos:Vector2, 
								 obstacleRadius:Number, 
								 safeDistance:Number): Vector2 
		{
			var desVel: Vector2; //desired velocity
			var steeringForce:Vector2;
			
			var vectorToObstacleCenter:Vector2 = Vector2.subtract(obstaclePos, position);
			var distance: Number = vectorToObstacleCenter.magnitude();
			
			//if vectorToCenter - obstacleRadius longer than safe return zero vector
			if (((distance - obstacleRadius) - radius) > safeDistance)
			{
				return new Vector2();
			}
			
			// if object behind me return zero vector
			if ( vectorToObstacleCenter.dot (fwd) < 0)
			{
				return new Vector2();
			}
			
			var rightDotVTOC:Number = vectorToObstacleCenter.dot(right);
			
			// if sum of radii < dot of vectorToCenter with right return zero vector
			if ((obstacleRadius + radius) < Math.abs(rightDotVTOC))
			{
				return new Vector2();
			}
			
			//desired velocity is to right or left depending on 
			// sign of  dot of vectorToCenter with right 
			
			if ( rightDotVTOC < 0)
			{
				desVel = Vector2.multiply(right, maxSpeed);
			}
			else
			{
				desVel = Vector2.multiply(right, -maxSpeed);
			}

			//subtract current velocity from desired velocity to get steering force
			steeringForce= Vector2.subtract(desVel, _velocity);
			
			//option: increase magnitude when obstacle is close
			steeringForce.timesEquals(safeDistance / distance);
			
			return steeringForce;
		}
		
		//  additional functions you might find usefull
		
		protected function stayOnStage( ):Vector2
		{
			//we could try this in class
			var steeringForce : Vector2 = new Vector2( );
			
			if(position.x > stage.stageWidth-75)
				steeringForce.plusEquals(flee(new Vector2(position.x+15,position.y)));
			else if(position.x < 75)
				steeringForce.plusEquals(flee(new Vector2(position.x-15,position.y)));
			if(position.y > stage.stageHeight-75)
				steeringForce.plusEquals(flee(new Vector2(position.x,position.y+15)));
			else if(position.y < 75)
				steeringForce.plusEquals(flee(new Vector2(position.x,position.y-15)));
			
			return steeringForce;
		}
		protected function fullSpeedAhead( ):Vector2
		{			
			var steeringForce:Vector2;
			steeringForce = Vector2.subtract(Vector2.multiply(fwd, maxSpeed), velocity);
			return steeringForce;
		}

	}
}



