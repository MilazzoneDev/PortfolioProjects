Carl Milazzo 2D world
Date: 1/15/13

Can be viewed at people.rit.edu/cxm7805

This project was a solo project
written in ActionScript

Love Bug

*to exit debug mode and watch without flow fields and wander circles click the box in the lower left hand corner

-2 advanced methods
	
	-pheromones (flow fields)
		-both the girls (pink circles) and bugs (black circles) leave behind a field
		(found in character.update && girls/bugs.leavePheromones)
		-boys follow it in their calcSteeringForce (overridden in Boys)

	-wander
		-everything wanders (unless they are following a pheromone trail)
		(a class in Character used in update)

		
- what happens

Characters and trees are placed randomly

when a boy finds a girl they will make their way to the house and then respawn.