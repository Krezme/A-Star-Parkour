using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AStar{
    public class Node : IHeapItem<Node>
    {
        // Is the node walkable?
        public bool walkable;
        // Is the node air?
        public bool isAir;

        public bool isOneUnitHeight;

        // Position of the node in the world
        public Vector3 worldPosition;
        // Position of the node in the grid on the X axis
        public int gridX;
        // Position of the node in the grid on the X axis
        public int gridY;
        // Position of the node in the grid on the Z axis
        public int gridZ;
        // The penalty for moving through this node
        public int movementPenalty;

        // The cost of moving from the start node to this node
        public int gCost;
        // The cost of moving from this node to the target node
        public int hCost;
        // The parent node (The previous node that was used to reach this node)
        public Node parent;

        //? If There is time use these "current" variables to control the AI's movement trough the nodes And remove the RayCasts. 
        //? This can be made trough functions 
        //! HOWEVER BE AWARE OF HOW MULTIPLE AI WILL WORK as all will be using the same nodes
        // The current vertical air time used for moving the AI trough air nodes
        public int currentVerticalAirTime;

        public int initialPossibleAirMovement;
        // The current slide length used for moving the AI trough low spaces
        public int currentSlideLength;

        // The index of the node in the heap
        int heapIndex;

        /// <summary>
        /// Setting node's information
        /// </summary>
        /// <param name="_walkable"> If it is walkable</param>
        /// <param name="_worldPos"> Its world position</param>
        /// <param name="_gridX"> Grid position on the X axis</param>
        /// <param name="_gridZ"> Grid position on the Y axis</param>
        /// <param name="_penalty"> Movement penalty</param>
        public Node(bool _walkable, bool _isAir, Vector3 _worldPos, int _gridX, int _gridY, int _gridZ, int _penalty) {
            walkable = _walkable;
            isAir = _isAir;
            worldPosition = _worldPos;
            gridX = _gridX;
            gridY = _gridY;
            gridZ = _gridZ;
            movementPenalty = _penalty;
        }

        /// <summary>
        /// Calculating the F cost of the node (G cost + H cost) How far it is from the start node and how far it is from the target node
        /// </summary>
        /// <value>G cost + H cost (How far it is from the start node and how far it is from the target node)</value>
        public int fCost {
            get {
                return gCost + hCost;
            }
        }

        /// <summary>
        /// The index of the node in the heap
        /// </summary>
        /// <value></value>
        public int HeapIndex {
            get {
                return heapIndex;
            }
            set {
                heapIndex = value;
            }
        }

        /// <summary>
        /// Comparing the F cost or the H cost of the nodes
        /// </summary>
        /// <param name="nodeToCompare">specific node to compare</param>
        /// <returns></returns>
        public int CompareTo(Node nodeToCompare) {
            int compare = fCost.CompareTo(nodeToCompare.fCost);
            if (compare == 0) {
                compare = hCost.CompareTo(nodeToCompare.hCost);
            }
            return -compare;
        }
    }
}