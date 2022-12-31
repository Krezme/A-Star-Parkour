# A-Star-Parkour

A Star Parkour is a prototype that is meant to achieve a smooth and realistic movement in all axis for an AI character, in this case a humanoid. In the case of a lot of pathfinding systems that already exist like Unity's build in NavMesh system, they do not have realistic jumping, climbing ledges, falling, sliding animation interaction with the environment.

## How to test the project

The project was made with Unity 2021.3.4f1 version.

The preferable Unity version to test the project out in is 2021.3.4f1. However, the project should work fine with any newer versions as well, but this is not guaranteed.

Open up the project using Unity and open the scene located: Assets > Scenes > Game.unity

To test the project and its functionality you can just press play and observe how the AI's find a path and traverse the terrain to reach that path.

### How to test different routes 

To test different routes either by starting at a different point and/or ending at a different point. 
The target and/or the AI (Seeker) can be moved around.

#### How to position the AI (Seeker)

The seeker has to be on the ground and standing on a surface.

Preferably it should be placed so the coordinates on the X and Z values have a .5 in the number. 
This in the large majority of cases is not required for this version of the pathfinding system.
However, if any difficulties arise it is best to position the AI (Seeker) properly.

#### How to position the Target

The target is the pink cube in the scene and it is also named "Target" in the Hierarchy.

The best way to position the Target is on the ground, touching the floor with it's bottom face but keeping the positioning to be .5 on every axis.
For the X and Z coordinates this is not required, but it is strongly recommended for the Y axis.

