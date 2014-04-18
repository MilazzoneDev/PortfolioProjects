//Carl Milazzo
package 
{
	import flash.display.*;
	import flash.events.*;
	import flash.ui.*;
	import flash.utils.*;
	
	
	
	public class Manager extends MovieClip
	{
		public var _characterArray		: Array;			// all the flockers
		public var _avoidArray			: Array;			// all the obstacles
		private var _dt					: Number;			// clock time since last update 
		private var _lastTime			: Number;			// for calculating dt
		private var _curTime			: Number;			// for calculating dt
		public var _fieldStrength		: Array;			// array for the pheromone strength
		public var _fieldDirection		: Array;			// array for the direction of pheromone
		private var _pheromone			: Shape;			// used to draw pheromones
		public var pheroW				: Number = 15;		// width of pheromones
		public var stageW				: Number;           // = stage.stageWidth/30;
		private var debuger				: Boolean = true;	// used for debuging
		public var house				: House
		
		
		public function Manager( )
		{
			_characterArray = new Array();
			_avoidArray = new Array();
			_fieldStrength = new Array();
			_fieldDirection = new Array();
			_pheromone = new Shape();
			stageW = stage.stageWidth/pheroW;
			addChildAt(_pheromone,0);
			
			this.buildWorld( );
			var e:Event;
			
			
			//event listener for to drive frame loop
			debug_Box.addEventListener(Event.CHANGE, changeDebug);
			addEventListener(Event.ENTER_FRAME, frameLoop);
			
		}
		
		// make the characters
		private function buildWorld( ): void
		{
			setUpField();
			
			//add the trees
			for (var j: int = 1; j <= 5; j++)
			{
				
				var obsticale:Obstacle = new Obstacle((Math.random()*(stage.stageWidth-180))+90,(Math.random()*(stage.stageHeight-180))+90,90)
				stage.addChild(obsticale);
				
				_avoidArray.push(obsticale);
			}
			
			//add the girls
			_characterArray.push(newGirls());
			_characterArray.push(newGirls());
			_characterArray.push(newGirls());
			//add the boys
			_characterArray.push(newBoys());
			_characterArray.push(newBoys());
			_characterArray.push(newBoys());
			//add the bugs
			_characterArray.push(newBugs());
			_characterArray.push(newBugs());
			
			
			house = new House(1020,460);
			addChild(house);
			
			_lastTime = getTimer( );
		}
		
		//This frameloop sends an update message to each turtle in the turtleArray
		private function frameLoop(e: Event ):void
		{
			// do update based on clock time
			_curTime = getTimer( );
			_dt = (_curTime - _lastTime)/1000;
			_lastTime = _curTime;
			graphics.clear( );
			
			//tell the characters to do their update
			for(var i:Number = 0; i < _characterArray.length; i++)
			{
				//_flockerArray[i].sightArray = closeflock(_flockerArray[i]);
				_characterArray[i].update(_dt);
				for(var j:Number = 0; j <_characterArray.length; j++)
				{
					if(!_characterArray[i].foundMate&&!_characterArray[j].foundMate)
					{
						_characterArray[i].checkMate(j);
						_characterArray[j].checkMate(i);
					}
				}
				
				if(_characterArray[i].foundMate)
				{
					
					// have they entered the house?
					if( _characterArray[i].position.x > house.x+15)
					{
						if((_characterArray[i].position.y >= house.y -30) || (_characterArray[i].position.y < house.y +30))
						{
							// new characters on the screen
							removeChild(_characterArray[i]);
							if(_characterArray[i].type == 1)
							{
								_characterArray[i] = newBoys();
							}
							else
							{
								_characterArray[i] = newGirls();
							}
						}
					}
				}
			}
			
			//decrements pheromones on field
			decField();
			
			graphics.clear();
			_pheromone.graphics.clear();
			// if debuging
			if(debuger)
			{
				// shows wander
				showWander();
				// shows pheromones
				showField();
			}
		}
		
		
		public function drawLine (startX:Number, startY:Number, endX:Number, endY:Number):void
		{
			with (graphics)
			{
				lineStyle(1);
				moveTo(startX, startY);
				lineTo(endX, endY);
			}
		}
		
		//sets up the grid for pheromones
		public function setUpField()
		{
			for(var i:int=0;i<=stage.stageHeight/pheroW;i++)
			{
				for(var j:int=0;j<=stageW;j++)
				{
					_fieldStrength[i*stageW+j]=0;
					_fieldDirection[i*stageW+j] = new Vector2(0);
				}
			}
		}
		
		//shows wander for each thing if being used
		public function showWander()
		{
			graphics.clear();
			for(var i:int=0;i<_characterArray.length;i++)
			{
				if(_characterArray[i].wandering)
				{
					var circleCenter:Vector2;
					circleCenter = Vector2.add(_characterArray[i].position,Vector2.multiply(_characterArray[i].fwd,Character.wanderCircleRadius+Character.wanderCircleDistance));
					with (graphics)
					{
						lineStyle(1);
						beginFill(0x000000, 0);
						drawCircle(circleCenter.x, circleCenter.y, Character.wanderCircleRadius);
						endFill();
						
						beginFill(0x000000,1);
						var angleDir:Vector2 = new Vector2(_characterArray[i].fwd.x,_characterArray[i].fwd.y);
						angleDir.rotate(_characterArray[i].wanderAngle);
						angleDir.timesEquals(Character.wanderCircleRadius);
						var seekPoint:Vector2 = new Vector2();
						seekPoint = Vector2.add(circleCenter,angleDir);
						drawCircle(seekPoint.x,seekPoint.y,5);
						endFill();
					}
				}
				drawLine(_characterArray[i].position.x,_characterArray[i].position.y,_characterArray[i].velocity.x + _characterArray[i].position.x,_characterArray[i].velocity.y +_characterArray[i].position.y);
			}
		}
		
		// decreases pheromones on the field
		public function decField()
		{
			var strength:Number;
			for(var i:int=0;i<stage.stageHeight/pheroW;i++)
			{
				for(var j:int=0;j<stage.stageWidth/pheroW;j++)
				{
					strength = _fieldStrength[i*stageW+j];
					_fieldDirection[i*stageW+j].normalize();
					_fieldDirection[i*stageW+j].timesEquals(10);
					if (strength > 0)
					{
						_fieldStrength[i*stageW+j] -= .25;
						if (strength < 0)
						{
							_fieldStrength[i*stageW+j]=0;
							_fieldDirection[i*stageW+j] = new Vector2();
						}
					}
					else if(strength < 0)
					{
						_fieldStrength[i*stageW+j] += .5;
						if (strength > 0)
						{
							_fieldStrength[i*stageW+j]=0;
							_fieldDirection[i*stageW+j] = new Vector2();
						}
					}
					if(strength>50)
					{
						_fieldStrength[i*stageW+j] = 50;
					}
					else if(strength<-50)
					{
						_fieldStrength[i*stageW+j] = -50;
					}
				}
				
			}
		}
		
		// shows the pheromones
		public function showField()
		{
			_pheromone.graphics.lineStyle(0, 0, 0);
			var strength:Number;
			var dir:Vector2;
			
			for(var i:int=0;i<stage.stageHeight/pheroW;i++)
			{
				for(var j:int=0;j<stage.stageWidth/pheroW;j++)
				{
					strength = _fieldStrength[i*stageW+j];
					_fieldDirection[i*stageW+j].normalize();
					_fieldDirection[i*stageW+j].timesEquals(10);
					dir = _fieldDirection[i*stageW+j];
					if (strength > 0)
					{
						_pheromone.graphics.beginFill(0xff0000, strength/75);
						_pheromone.graphics.drawRect(j*pheroW, i*pheroW, pheroW,pheroW);
						drawLine((j*pheroW)+pheroW/2,(i*pheroW)+pheroW/2,(j*pheroW)+dir.x+pheroW/2,(i*pheroW)+dir.y+pheroW/2);
						
						
						_pheromone.graphics.endFill();
					}
					else if(strength < 0)
					{
						_pheromone.graphics.beginFill(0x000000, (strength/75)*-1);
						_pheromone.graphics.drawRect(j*pheroW, i*pheroW, pheroW,pheroW);
						drawLine((j*pheroW)+pheroW/2,(i*pheroW)+pheroW/2,(j*pheroW)+dir.x+pheroW/2,(i*pheroW)+dir.y+pheroW/2);
						
						
						_pheromone.graphics.endFill();
					}
					
				}
			}
			
		}
		
		
		// creates a new boy
		public function newBoys():Boys
		{
			var rand:int = (int)(Math.random()*4);
			var tempx:Number;
			var tempy:Number;
			if (rand == 0)
			{
				tempx = -15;
				tempy = (Math.random()*stage.stageHeight-60)+30;
			}
			if (rand == 1)
			{
				tempx = stage.stageWidth+15;
				tempy = (Math.random()*stage.stageHeight-60)+30;
			}
			if (rand == 2)
			{
				tempx = (Math.random()*stage.stageWidth-60)+30;
				tempy = -15
			}
			if (rand == 3)
			{
				tempx = (Math.random()*stage.stageWidth-60)+30;
				tempy = 15
			}
			
			var temp:Boys = new Boys(this,tempx,tempy,90);
			
			return temp;
		}
		
		// creates a new girl
		public function newGirls():Girls
		{
			var rand:int = (int)(Math.random()*4);
			var tempx:Number;
			var tempy:Number;
			if (rand == 0)
			{
				tempx = -15;
				tempy = (Math.random()*stage.stageHeight-60)+30;
			}
			if (rand == 1)
			{
				tempx = stage.stageWidth+15;
				tempy = (Math.random()*stage.stageHeight-60)+30;
			}
			if (rand == 2)
			{
				tempx = (Math.random()*stage.stageWidth-60)+30;
				tempy = -15
			}
			if (rand == 3)
			{
				tempx = (Math.random()*stage.stageWidth-60)+30;
				tempy = 15
			}
			
			var temp:Girls = new Girls(this,tempx,tempy,75);
			
			return temp;
		}
		
		// creates a new bug
		public function newBugs():Bugs
		{
			var rand:int = (int)(Math.random()*4);
			var tempx:Number;
			var tempy:Number;
			if (rand == 0)
			{
				tempx = -15;
				tempy = (Math.random()*stage.stageHeight-60)+30;
			}
			if (rand == 1)
			{
				tempx = stage.stageWidth+15;
				tempy = (Math.random()*stage.stageHeight-60)+30;
			}
			if (rand == 2)
			{
				tempx = (Math.random()*stage.stageWidth-60)+30;
				tempy = -15
			}
			if (rand == 3)
			{
				tempx = (Math.random()*stage.stageWidth-60)+30;
				tempy = 15
			}
			
			var temp:Bugs = new Bugs(this,tempx,tempy,70);
			
			return temp;
		}
		
		//debuger
		public function changeDebug(e:Event)
		{
			debuger = !debuger;
		}
	}
}




