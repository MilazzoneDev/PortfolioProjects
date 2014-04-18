//Carl Milazzo
package  {
	
	public class Character extends SteeringVehicle{
		
		public static var boundsWeight: Number = 2;
		public static var sepWeight:Number = 10;
		public static var cohWeight:Number = 10;
		public static var goWeight: Number = 2;
		public static var findWeight: Number = 30;
		public static var avoidWeight:Number = 1.75;
		public var type: Number = 0; // used to determine what character type this is
		public static var wanderCircleRadius:Number = 40; // radius of wander circle
		private var _wanderAngle:Number = 0; // angle of the wander circle
		public static var wanderCircleDistance:Number = 50; // wander circle distance
		public var wandering:Boolean = true; // am I wandering?
		public var foundMate:Boolean = false; // have I found a mate?
		public var MateNum:Number = Number.NaN; // who is my mate?
		
		
		public function get wanderAngle( ) : Number 		{return _wanderAngle; }
		public function set wanderAngle(angle: Number) : void 
		{
			if(angle > 360){angle-=360;}
			if(angle < 360){angle+= 360;}
			_wanderAngle = angle;
		}
		
		// creates a character (manager, starting X, starting Y, MaxSpeed, type(boy/girl/bug))
		public function Character(aMan:Manager, aX:Number=0, aY:Number=0, aSpeed:Number=0, characterType:Number = 0) {
			// constructor code
			super(aMan, aX, aY, aSpeed);
			type = characterType;
		}
		
		// calculate where I am trying to go and return a force to get me there
		override protected function calcSteeringForce( ):Vector2
		{
			var steeringForce:Vector2 = new Vector2( );
			
			//avoid obsticles
			for(var i = 0; i < _manager._avoidArray.length; i++)
			{
				var avoidF:Vector2 = avoid(_manager._avoidArray[i].position,_manager._avoidArray[i].radius,_manager._avoidArray[i].radius*1.5)
				steeringForce.plusEquals(Vector2.multiply(avoidF, avoidWeight));
			}
			
			//go forward
			steeringForce.plusEquals(Vector2.multiply(fullSpeedAhead(),goWeight));
			
			//I have not found a mate
			if(!foundMate)
			{
				//wander around
				steeringForce.plusEquals(wander());
				
				//stay on stage
				steeringForce.plusEquals(Vector2.multiply(stayOnStage(), boundsWeight));
			}
			//I have found a mate
			else
			{
				//Don't wander anymore
				wandering = false;
				//Go with my mate to the house
				steeringForce.plusEquals(Vector2.multiply(seek(_manager._characterArray[MateNum].position),cohWeight));
				steeringForce.plusEquals(Vector2.multiply(separation(_manager._characterArray[MateNum].position),sepWeight));
				var houseloc:Vector2 = new Vector2(_manager.house.x+30,_manager.house.y);
				steeringForce.plusEquals(Vector2.multiply(seek(houseloc),findWeight));
			}
			
			return steeringForce;
		}
		
		// Wander function
		public function wander():Vector2
		{
			var steeringForce:Vector2 = new Vector2( );
			//steeringForce.plusEquals(Vector2.multiply(fullSpeedAhead(), goWeight));
			//move angle
			wanderAngle += ((Math.random()*30)-15);
			var angleDir:Vector2 = new Vector2(this.fwd.x,this.fwd.y);
			angleDir.rotate(wanderAngle);
			angleDir.timesEquals(wanderCircleRadius);
			//compute the seek point
			var circleCenter:Vector2 = Vector2.add(this.position,Vector2.multiply(this.fwd,wanderCircleRadius+wanderCircleDistance));
			var seekPoint:Vector2 = Vector2.add(circleCenter,angleDir);
			
			steeringForce.plusEquals(Vector2.multiply(seek(seekPoint),goWeight));
			
			return steeringForce;
		}
		
		// Used to keep mates separted
		protected function separation(where:Vector2):Vector2
		{
			var vec:Vector2 = new Vector2();
			var away:Vector2;
			var dist:Number;
			
			away = Vector2.subtract(position, where);
			dist = Vector2.distanceSqr(position, where);
			away.normalize();
			vec.plusEquals(Vector2.multiply(away,1/ dist));
			
			vec.normalize();
			vec.timesEquals(this.maxSpeed);
			
			return vec;
		}
		
		// Used to keep mates together
		protected function cohesion(where:Vector2):Vector2
		{
			var desVel:Vector2;
			var steeringForce:Vector2 = new Vector2();
			desVel = Vector2.subtract(where, position);
			var dist:Number = Vector2.distanceSqr(position, where)/100;
			
			// scale desired velocity so its magnitude equals max speed
			desVel = desVel.normalized( ).timesEquals(maxSpeed);
			
			// to get steerinForce subtract current velocity from desired velocity
			//steeringForce = Vector2.subtract(desVel, velocity)
			
			steeringForce.plusEquals(Vector2.multiply(desVel, dist));
			
			return steeringForce;
		}
		
		// Stub used for override
		public function checkMate(j:Number)
		{
			
		}
		

	}
	
}
