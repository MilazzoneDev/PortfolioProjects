package  {
	
	public class Boys extends Character{
		
		public function Boys(aMan:Manager, aX:Number=0, aY:Number=0, aSpeed:Number=0) {
			// constructor code
			super(aMan, aX, aY, aSpeed,1);
			aMan.addChildAt(this,1);
		}
		
		override protected function calcSteeringForce( ):Vector2
		{
			var steeringForce:Vector2 = new Vector2( );
			
			wandering = false;
			var yPos:Number = (int)(this.position.y/_manager.pheroW);
			if (yPos < 0)
			{
				yPos = 0;
			}
			else if(yPos > stage.stageHeight/_manager.pheroW)
			{
				yPos = stage.stageHeight/_manager.pheroW;
			}
			
			var xPos:Number = (int)(this.position.x/_manager.pheroW);
			if (xPos < 0)
			{
				xPos = 0;
			}
			
			steeringForce.plusEquals(Vector2.multiply(fullSpeedAhead(),goWeight));
			
			// have they found a mate, if not
			if(!foundMate)
			{
				steeringForce.plusEquals(Vector2.multiply(stayOnStage(), boundsWeight));
				
				var fieldStrength:Number =_manager._fieldStrength[yPos*_manager.stageW+xPos];
				var sourcePhero:Vector2 = Vector2.add(_manager._fieldDirection[yPos*_manager.stageW+xPos],this.position);
				//follow female?
				if(fieldStrength>0)
				{
					steeringForce.plusEquals(Vector2.multiply(seek(sourcePhero),goWeight));
					wanderAngle = 0;
				}
				//run from bug?
				else if(fieldStrength<0)
				{
					steeringForce.plusEquals(Vector2.multiply(flee(sourcePhero),goWeight));
					wanderAngle = 0;
				}
				// can't see anything
				else
				{
					steeringForce.plusEquals(wander());
					wandering = true;
					
					for(var i = 0; i < _manager._avoidArray.length; i++)
					{
						var avoidF:Vector2 = avoid(_manager._avoidArray[i].position,_manager._avoidArray[i].radius,_manager._avoidArray[i].radius*1.5)
						steeringForce.plusEquals(Vector2.multiply(avoidF, avoidWeight));
					}
				}
			}
			// if they have found a mate
			else
			{
				for(var h = 0; h < _manager._avoidArray.length; h++)
				{
					var avoidForce:Vector2 = avoid(_manager._avoidArray[h].position,_manager._avoidArray[h].radius,_manager._avoidArray[h].radius*1.5)
					steeringForce.plusEquals(Vector2.multiply(avoidForce, avoidWeight));
				}
				
				steeringForce.plusEquals(Vector2.multiply(seek(_manager._characterArray[MateNum].position),cohWeight));
				steeringForce.plusEquals(Vector2.multiply(separation(_manager._characterArray[MateNum].position),sepWeight));
				var houseloc:Vector2 = new Vector2(_manager.house.x+30,_manager.house.y);
				steeringForce.plusEquals(Vector2.multiply(seek(houseloc),findWeight));
				
			}
			
			
			return steeringForce;
			
			
		}
		
		//is this mate a match for me and am I close enough
		override public function checkMate(j:Number)
		{
			if(_manager._characterArray[j].type == 2)
			{
				if(Vector2.distance(position, _manager._characterArray[j].position)<30)
				{
					MateNum = j;
					foundMate = true;
					maxSpeed = _manager._characterArray[j].maxSpeed;
				}
			}
		}
		
		// stump used for drawing better sprites
		override protected function drawing()
		{
			
		}

	}
	
}
