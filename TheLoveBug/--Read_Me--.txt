Carl Milazzo 2D world
Instructor: Steve Kurtz
Date: 1/15/13

Love connection

*to exit debug mode and watch without flow fields and wander circles click the box in the lower left hand corner

-2 advanced methods
	
	-pheromones (flow fields)
		-both the girls (pink circles) and bugs (black circles) leave behind a field
		(found in character.update && girls/bugs.leavePheromones)
		-boys follow it in their calcSteeringForce (overriden in Boys)

	-wander
		-everything wanders (unless they are following a pheromone trail)
		(a class in Character used in update)

		
- what happens

Characters and trees are placed randomly

when a boy finds a girl they will make their way to the house and then respawn.