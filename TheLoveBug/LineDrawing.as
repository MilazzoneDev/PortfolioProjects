package
{
	import flash.display.MovieClip;
	import flash.display.Shape;
	
	public class LineDrawing extends MovieClip
	{
		private var _line:Shape;
		
		public function LineDrawing()
		{
			_line = new Shape();
			addChild(_line);	
		}
		
		
		public function drawVector(v:Vector2):void
		{
			_line.graphics.clear();
			_line.graphics.lineStyle(2, 0, 1);
			_line.graphics.beginFill(0xff0000, 1);
			_line.graphics.drawCircle(0, 0, 5);
			_line.graphics.endFill();
			_line.graphics.moveTo(0,0);
			_line.graphics.lineTo(v.x,v.y);
			
		}
	}
}