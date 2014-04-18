//Carl Milazzo
package  {
	
	public class Girls extends Character{

		public function Girls(aMan:Manager, aX:Number=0, aY:Number=0, aSpeed:Number=0) {
			// constructor code
			super(aMan, aX, aY, aSpeed,2);
			aMan.addChildAt(this,1);
		}
		
		//leaves pheromones
		override protected function leavePheromones()
		{
			//makes a 5 by 5 grid
			for(var i:int = -2; i < 3; i++)
			{
				for(var j:int = -2; j < 3; j++)
				{
					var yPos:Number = (int)(this.position.y/_manager.pheroW)+i;
					if (yPos < 0)
					{
						yPos = 0;
					}
					else if(yPos > stage.stageHeight/_manager.pheroW)
					{
						yPos = (stage.stageHeight/_manager.pheroW)-1;
					}
					
					var xPos:Number = (int)(this.position.x/_manager.pheroW)+j;
					if (xPos < 0)
					{
						xPos = 0;
					}
					else if(xPos > stage.stageWidth/_manager.pheroW)
					{
						xPos = (stage.stageWidth/_manager.pheroW)-1;
					}
					
					var spot:Vector2 = new Vector2((xPos*_manager.pheroW)+_manager.pheroW/2,(yPos*_manager.pheroW)+_manager.pheroW/2);
					
					if(i==0 && j==0)
					{
						_manager._fieldStrength[yPos*_manager.stageW+xPos]+=5;
						_manager._fieldDirection[yPos*_manager.stageW+xPos].plusEquals(Vector2.subtract(this.position,spot));
					}
					else if(i==0 ||j ==0)
					{
						_manager._fieldStrength[yPos*_manager.stageW+xPos]+=4;
						_manager._fieldDirection[yPos*_manager.stageW+xPos].plusEquals(Vector2.subtract(this.position,spot));
					}
					else if((i==-1||i==1)||(j==-1||j==1))
					{
						_manager._fieldStrength[yPos*_manager.stageW+xPos]+=3;
						_manager._fieldDirection[yPos*_manager.stageW+xPos].plusEquals(Vector2.subtract(this.position,spot));
					}
					else if((i==-2||i==2)&&(j==-2||j==2))
					{
						
					}
					else
					{
						_manager._fieldStrength[yPos*_manager.stageW+xPos]+=2;
						_manager._fieldDirection[yPos*_manager.stageW+xPos].plusEquals(Vector2.subtract(this.position,spot));
						//_manager._fieldDirection[(int)((this.position.y/_manager.pheroW)+i)*_manager.stageW+(int)((this.position.x/_manager.pheroW)+j)]=this.fwd;
					}
				}
			}
		}
		
		//is this mate a match for me and am I close enough
		override public function checkMate(j:Number)
		{
			if(_manager._characterArray[j].type == 1)
			{
				if(Vector2.distance(position, _manager._characterArray[j].position)<30)
				{
					MateNum = j;
					foundMate = true;
				}
			}
		}
		// stump used for drawing better sprites
		override protected function drawing()
		{
			
		}

	}
	
}
