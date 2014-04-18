package  
{
	import flash.display.MovieClip;
	import flash.events.*;
	import flash.ui.*;
	import flash.utils.*;
	
	public class VectorTurtle extends MovieClip
	{
		protected var _position : Vector2; //vector from origin - components correspond to coordinates
		protected var _fwd : Vector2; // Unit vector indicating forward direction
		protected var _velocity : Vector2; // vector in forward direction scaled by speed
		protected var _speed : Number; // speed in units per second
		protected var _manager: Manager; // reference to manager
		
		//constructor initializes properties
		public function VectorTurtle(aMan:Manager, anX:Number = 0, 
								aY:Number = 0) 
		{
			_manager = aMan;
			x= anX;
			y = aY;
			_position = new Vector2(x, y);
			_speed = 0; 
			_fwd = new Vector2(1, 0);
			_velocity = Vector2.multiply(_fwd, _speed);
		}

		//accessors and mutators -- getters and setters
		public function get position( )		: Vector2				{ return  _position;	}
		public function get fwd( )			: Vector2				{ return _fwd;			}
		public function get speed( )		: Number				{ return _speed;		}
		public function get velocity()		: Vector2 				{ return _velocity;		}

		//-----------------SETTING THE POSITION AND FWD VECTORS-----------------//

		
		public function set position(pos : Vector2): void
		{
			_position.x = pos.x;
			_position.y = pos.y;
			x = pos.x;
			y = pos.y;
		}
		
		// assigning a new value to the fwd vector requires adjusting rotation
		public function set fwd(vec: Vector2): void
		{
			if (vec.x != 0 && vec.y != 0)
			{
				_fwd.x = vec.x;
				_fwd.y = vec.y;
				_fwd.normalize( );
				rotation = _fwd.angle;
			}
		}			

		// set speed in pixels per second
		public function set speed(amt:Number):void		{_speed = amt; }

		//-----------------MOVING-----------------//

		
		// Update is called every frame by the manager class, TurtleAPP
		// dt is the elapsed time in seconds since the last update
		// Update calculates velocity and calls move to move the turtle
		public function update( dt : Number): void
		{
			// calculate velocity
			_velocity = Vector2.multiply(_fwd, _speed);
			// call move to update position
			move(Vector2.multiply(_velocity, dt));
		}
		
		
		// Alters the position vector and movieClip properties to
		// relect the change indicated by the moveVector
		public function move( moveVector:Vector2): void
		{
			// calculate new position by adding the moveVector to the old position
			// update MovieClip's coordinates 
			_position.plusEquals(moveVector);
			x = _position.x;
			y = _position.y;
		}

		
		//-----------------ROTATION-----------------//
		
		// rotate the turtle to face an absolute angle
		// rotation  here is a property inherited from MovieClip
		// degToVector returns a unit vector whose direction
		// corresponds to the set rotation
		public function turnAbs(ang: Number): void
		{
			rotation = ang;
			_fwd = Vector2.degToVector(rotation);
		}

	
		// TurnRight and turnLeft are relative commands altering the
		//  turtle's heading or and forward vector by ang degrees
		public function turnRight(ang: Number): void
		{
			// change rotation
			rotation += ang;
			// change forward vector to reflect new orientation
			_fwd = Vector2.degToVector(rotation);
		}
		
		public function turnLeft(ang: Number): void
		{
			// change rotation
			rotation -= ang;
			// change forward vector to reflect new orientation
			_fwd = Vector2.degToVector(rotation);
		}
		
		//Checks if the turtle is colliding with the given turtle
		public function checkCollision(turtle2:VectorTurtle):Boolean
		{
			if(Vector2.distance(position, turtle2.position)<30)
				return true;
			return false;
		}
		
		
		//-----------------INFO-----------------//
		
		override public function toString( ): String
		{
			
			return "x:" + x + ", y:" + y;;
		}
		
		
		
	}
}
