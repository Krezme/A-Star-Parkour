# **A-Star-Parkour**

A Star Parkour is a prototype that is meant to achieve a smooth and realistic movement in all axis for an AI character, in this case a humanoid. In the case of a lot of pathfinding systems that already exist like Unity's build in NavMesh system, they do not have realistic jumping, climbing ledges, falling, sliding animation interaction with the environment.

## **How to test the project**

The project was made with Unity 2021.3.4f1 version.

The preferable Unity version to test the project out in is 2021.3.4f1. However, the project should work fine with any newer versions as well, but this is not guaranteed.

Open up the project using Unity and open the scene located: Assets > Scenes > Game.unity

To test the project and its functionality you can just press play and observe how the AI's find a path and traverse the terrain to reach that path.

### **How to test different routes**

To test different routes either by starting at a different point and/or ending at a different point. 
The target and/or the AI (Seeker) can be moved around.

#### **How to position the AI (Seeker)**

The seeker has to be on the ground and standing on a surface.

Preferably it should be placed so the coordinates on the X and Z values have a .5 in the number. 
This in the large majority of cases is not required for this version of the pathfinding system.
However, if any difficulties arise it is best to position the AI (Seeker) properly.

#### **How to position the Target**

The target is the pink cube in the scene and it is also named "Target" in the Hierarchy.

The best way to position the Target is on the ground, touching the floor with it's bottom face but keeping the positioning to be .5 on every axis.
For the X and Z coordinates this is not required, but it is strongly recommended for the Y axis.

### **Inspect the Grid and See the Path**

Warning: Drawing the Grid is a very Heavy operation for a system. On a less powerful machine the project may crash Unity and/or lock up a system. 
Please be aware that because this is a prototype of a huge project, small things like drawing the grid would not be optimized as they are a quality of life functions and they are not used inside of a build game.

#### **Grid**

To be able to inspect the Grid that the system has build:
- Navigate to the Hierarchy > Game Manager > Grid (C# Script Component)
  - Display Grid Gizmos
  - Display Only Ground Gizmos
- Play the project and Pause (Pause is not required)

Display Grid Gismos draws all of the nodes in the grid. If the default settings, that are not changed, are tried to be drawn it will draw 1,000,000 nodes. The default size for the grid is 100 X 100 X 100.

Display Only Ground Gizmos does exactly what the name suggests. It only draws the nodes that have a surface underneath them which are able to be walked on by the seeker.

The differently coloured nodes are colour coded:

<div style="display: flex; gap: 1rem; align-items: center;">
    <div style="display: flex; gap: 1rem;">
        <div style="width: 2rem; height: 2rem; background: red; border-radius: 15%; margin-left: 1rem; margin-bottom: 1rem;"></div>
    </div>
    <p> Obstacle</p>
</div>
<div style="display: flex; gap: 1rem; align-items: center;">
    <div style="display: flex; gap: 1rem;">
        <div style="width: 2rem; height: 2rem; background: cyan; border-radius: 15%; margin-left: 1rem; margin-bottom: 1rem;"></div>
        </div>
    <p> Air Nodes (Empty)</p>
</div>
<div style="display: flex; gap: 1rem; align-items: center;">
    <div style="display: flex; gap: 1rem;">
        <div style="width: 2rem; height: 2rem; background: white; border-radius: 15%; margin-left: 1rem; margin-bottom: 1rem;"></div>
        <div style="width: 2rem; height: 2rem; background: gray; border-radius: 15%; margin-bottom: 1rem;"></div> 
        <div style="width: 2rem; height: 2rem; background: black; border-radius: 15%; margin-bottom: 1rem;"></div>
    </div> 
    <p> Terrain Difficulty: Easy > Hard (Gradient)</p>
</div>
<div style="display: flex; gap: 1rem; align-items: center;">
    <div style="display: flex; gap: 1rem;">
        <div style="width: 2rem; height: 2rem; background: lime; border-radius: 15%; margin-left: 1rem; margin-bottom: 1rem;"></div>
    </div>
    <p> Slidable</p>
</div>

#### **Path**

To be able to inspect the calculated path from the Seeker to the Target:
- Navigate to the Hierarchy > Select the desired "Seeker" Game Object > Unit (C# Script Component)
- Tick Display Path Gizmos

This will show the path for that Seeker. This can be done for as many seekers as desired at once.

The path will be drawn in Black Squares.
The also will be blue perpendicular lines that represent the turning boundaries for each node.

