package  {
	
	public class Obstacle extends LineDrawing{

		public var radius:int;
		public var position:Vector2;
		
		public function Obstacle(aX:Number=0, aY:Number=0, radius:int = 90) {
			// constructor code
			super();
			this.radius = radius;
			this.position = new Vector2(aX,aY);
			x=aX;
			y=aY;
		}
		
		public function display()
		{
			/*graphics.clear();
			//_line.graphics.lineStyle(2, 0, 1);
			graphics.beginFill(0xFFFFFF, 1);
			graphics.drawCircle(position.x, position.y, radius);
			graphics.endFill();*/
		}

	}
	
}
