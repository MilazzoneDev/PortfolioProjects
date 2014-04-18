//Carl Milazzo
package 
{
	
	public class Vector2
	{
		//members of vector class
		private var _x: Number;
		private var _y: Number;
		//constant for conversion from degrees to radians
		public static const DEGRAD: Number = Math.PI/180;
		
		//constructor
		public function Vector2(x:Number=0, y:Number=0)
		{
			_x = x;
			_y = y;
		}
		
		//accessors and mutators -- getters and setters
		public function get x( ) : Number 			{return _x; }
		public function get y( ) : Number 			{return _y; }
		public function set x(x: Number) : void 	{ _x = x; }
		public function set y(y: Number) : void 	{_y = y; }
		
		// public static methods
		
		public static function add(v1:Vector2,  v2:Vector2):Vector2
		{
			return new Vector2(v1.x + v2.x, v1.y + v2.y);
		}

		//****************complete these functions*****************
		public static function subtract(v1:Vector2,  v2:Vector2):Vector2
		{
			return new Vector2(v1.x - v2.x, v1.y - v2.y ); // replace with valid vector value
		}
		
		public static function multiply(v1:Vector2,  num:Number):Vector2
		{
			return new Vector2(v1.x * num, v1.y * num ); // replace with valid vector value			
		}

		public static function divide(v1:Vector2,  num:Number):Vector2
		{
			return new Vector2(v1.x / num, v1.y / num ); // replace with valid vector value			
		}
		
		// returns distance between this vector and v2
		public static function distance(v1 : Vector2, v2 : Vector2): Number
		{
			var deltaX = v1.x - v2.x;
			var deltaY = v1.y - v2.y;
			return Math.sqrt(deltaX * deltaX + deltaY * deltaY);
		}
		
		// returns squared distance between this vector and v2  
		public static function distanceSqr(v1 : Vector2, v2 : Vector2): Number
		{
			var deltaX = v1.x - v2.x;
			var deltaY = v1.y - v2.y;
			return (deltaX * deltaX + deltaY * deltaY);
		}	
		
		//----------------Instance Methods-----------------------

		public function timesEquals(num:Number):Vector2
		{
			_x *= num;
			_y *= num;
			return this;
		}
		
		//****************complete these functions*****************
		
		public function plusEquals(v2:Vector2):Vector2
		{
			_x += v2.x;
			_y += v2.y;
			return this; // replace with valid vector value
		}
		
		public function divideEquals(num:Number):Vector2
		{
			_x /= num;
			_y /= num;
			return this; // replace with valid vector value
		}
		
		public function minusEquals(v2:Vector2):Vector2
		{
			_x -= v2.x;
			_y -= v2.y;
			return this; // replace with valid vector value
		}

		
		// returns length or magnitude of this vector
		public function magnitude( ) : Number
		{
			return Math.sqrt(_x * _x + _y * _y);
		}
		
		
		
		// makes this a unit vector (magnitude = 1)
		public function normalize( ) : void
		{
			var mag : Number = Math.sqrt(_x * _x + _y * _y);
			if (mag == 0)
			{
				_x = 0;
				_y = 0;
			}
			else
			{
				_x = _x / mag;
				_y = _y / mag;
			}
		}
		
		// returns a unit vector in direction of this vector
		public function normalized( ) : Vector2
		{
			var mag : Number = Math.sqrt(_x * _x + _y * _y);
			if(mag == 0) 
				return new Vector2( );
			else
				return new Vector2(_x / mag, _y / mag);
		}		
		
		// returns angle of this vector in degrees
		public function get angle( ) : Number
		{
			return Math.atan2(_y, _x) * 180 / Math.PI;
		}
		
		// returns a unit vector in direction angle supplied in degrees
		public static function degToVector(deg: Number): Vector2
		{
			var rad = deg * DEGRAD;
			return new Vector2(Math.cos(rad), Math.sin(rad));
		}
		
		// returns a unit vector in direction angle supplied in radians
		public static function radToVector(rad: Number): Vector2
		{
			return new Vector2(Math.cos(rad), Math.sin(rad));
		}
		
		// -----------------------additional vector methods---------------
		
		// returns the dot product of this vector and and v2
		public function dot(v2 : Vector2): Number
		{
			return (_x * v2.x + _y * v2.y);
		}
		
		// rotates this vector by deg degrees
		public function rotate( deg: Number): void
		{
			var rad: Number = deg * DEGRAD;
			var cos: Number = Math.cos(rad);
			var sin : Number= Math.sin(rad);
			var tempX: Number = _x;
			_x = _x * cos -  _y * sin;
			_y = _y * cos +  tempX * sin;
		}
		
		//returns the angle between v1 and v2 in degrees
		public static function angleBetween(v1:Vector2, v2:Vector2):Number
		{
			var ang1, ang2: Number;
			ang1 = Math.atan2(v1.y, v1.x);
			ang2 = Math.atan2(v2.y, v2.x);
			return (ang2 - ang1)/DEGRAD;
		}
		
		// returns a copy of this vector rotated 90 degrees cw (Flash)
		public function perpRight( ): Vector2
		{
			return new Vector2(-_y, _x);
		}
		
		public function toString( ): String
		{
			return("x: " + int(_x*100)/100 + ", \ty: " + int(_y*100)/100);
		}
		

	}
}