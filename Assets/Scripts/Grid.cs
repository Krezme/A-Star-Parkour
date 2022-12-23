using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System.Diagnostics;
using System.Threading;

namespace AStar{
    public class Grid : MonoBehaviour
    {

        public static Grid instance;

        //create a singleton
        void Awake() {
            if (instance != null) {
                UnityEngine.Debug.LogError("More than one Grid in scene!");
            }else {
                instance = this;
            }

            nodeDiameter = nodeRadius * 2;
            gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
            gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
            gridSizeZ = Mathf.RoundToInt(gridWorldSize.z / nodeDiameter);

            foreach (TerrainType region in walkableRegions) {
                walkableMask.value |= region.terrainMask.value; //bitwise OR operator
                walkableRegionsDictionary.Add(Mathf.RoundToInt(Mathf.Log(region.terrainMask.value, 2)), region.terrainPenalty);
            }

            CreateGrid();
        }

        [Tooltip("Should the grid be displayed in the scene view?")]
        public bool displayGridGizmos;
        public bool displayOnlyGroundGridGizmos;
        [Tooltip("Layers that the enemy cannot walk on")]
        public LayerMask unwalkableMask;
        public Vector3 gridWorldSize;
        [Tooltip("The radius of the nodes in world units. The diameter is calculated automatically. (2x the radius)")]
        public float nodeRadius;
        [Tooltip("The layers that the enemy can walk on and their movement penalty")]
        public TerrainType[] walkableRegions;
        [Tooltip("The Penalty for moving close to Obstacles")]
        public int obstacleProximityPenalty = 10;
        public int airPenalty = 30;
        [Tooltip("Leaving gaps between the drawn gizmo grid squares (Does not effect the actual grid)")]
        public float gizmosGridGap = 0.1f;

        // All walkableLayers combined into one LayerMask
        LayerMask walkableMask;
        // Dictionary of all walkable layers and their movement penalty
        Dictionary<int, int> walkableRegionsDictionary = new Dictionary<int, int>();
        // An array of all the nodes in the grid
        Node[,,] grid;

        // The diameter of the nodes in world units
        float nodeDiameter;
        // The number of nodes in the grid on the x axis
        int gridSizeX;
        // The number of nodes in the grid on the y axis
        int gridSizeY;
        // The number of nodes in the grid on the z axis
        int gridSizeZ;

        // the smallest possible penalty for a node
        int penaltyMin = int.MinValue;
        // the largest possible penalty for a node
        int penaltyMax = int.MaxValue;
        // The Max size of the grid
        public int MaxSize{
            get{
                return gridSizeX * gridSizeY * gridSizeZ;
            }
        }

        /// <summary>
        /// Generating the grid
        /// </summary>
        void CreateGrid() {
            grid = new Node[gridSizeX, gridSizeY, gridSizeZ]; //create a new grid
            Vector3 worldBottomLeft = transform.position - (Vector3.right * gridWorldSize.x / 2) - (Vector3.forward * gridWorldSize.z / 2); //get the bottom left corner of the grid
            //loop through every node in the grid
            for (int x = 0; x < gridSizeX; x++) { //loop through the grid on the x axis
                for (int y = 0; y < gridSizeY; y++) { //loop through the grid on the y axis
                    for (int z = 0; z < gridSizeZ; z++) { //loop through the grid on the z axis
                        Vector3 worldPoint = worldBottomLeft + (Vector3.right * (x * nodeDiameter + nodeRadius)) + (Vector3.up * (y * nodeDiameter + nodeRadius)) + (Vector3.forward * (z * nodeDiameter + nodeRadius)); //get the world position of the next node depending on which node the loops are on and the additional node specifications
                        bool walkable = true;
                        
                        bool isAir = true; // Default value for the node is air. If the node is actually air it will stay true, but it will be calculated below

                        int movementPenalty = 0; // The movement penalty of the node

                        //raycast to check if the node is on a walkable layer and which walkable layer it is on
                        Ray ray = new Ray(worldPoint + Vector3.up * 0.1f, Vector3.down); // position of the raycast is the center of the node and the direction is down
                        RaycastHit hit; // the hit information of the raycast
                        Collider[] obstacles = Physics.OverlapSphere(worldPoint, nodeRadius * 0.2f, walkableMask); // Checking if there is obstacle inside of the node it will return true if there is no obstacle
                        if (Physics.Raycast(ray, out hit, nodeDiameter, walkableMask, QueryTriggerInteraction.UseGlobal)) { // if the raycast hits something with a walkable layer
                            walkableRegionsDictionary.TryGetValue(hit.collider.gameObject.layer, out movementPenalty); // get the movement penalty of the layer the node is on
                            isAir = false; // raycast has hit something with a walkable layer the node is not air
                        }
                        else {
                            walkable = false; // if the raycast does not hit anything with a walkable layer the node is not walkable
                            isAir = true; // raycast has not hit anything with a walkable layer the node is air
                        }

                        if (obstacles.Length > 0) { // There is Obstacle inside of the node
                            walkable = false;
                            isAir = false;
                            if (y > 0 && y < gridSizeY - 1) { // if the node is not on the bottom or top layer of the grid
                                grid[x, y -1, z].isOneUnitHeight = true;
                            }
                        }
                        else {
                            if (y > 0 && y < gridSizeY - 1) { // if the node is not on the bottom or top layer of the grid
                                grid[x, y -1, z].isOneUnitHeight = false;
                            }
                        }

                        /* if (isAir) {
                            movementPenalty += airPenalty;
                        } */

                        if (!walkable) { // if the node is not walkable
                            movementPenalty += obstacleProximityPenalty; // add the obstacle proximity penalty to the movement penalty
                        }

                        

                        grid[x, y, z] = new Node(walkable, isAir, worldPoint, x, y, z, movementPenalty); // create a new node in the grid and record all the information about it
                    }
                }
            }



            BlurPenaltyMap(3); // Blurring the surrounding 3 nodes of the obstacles
        }

        /// <summary>
        /// Blurring the penalty map. Calculating the surrounding nodes of the obstacles and averaging their penalty
        /// </summary>
        /// <param name="blurSize"> How many squares next to the obstacles are going to be affected</param>
        void BlurPenaltyMap (int blurSize) {
            int kernelSize = blurSize * 2 + 1;
            int kernelExtents = (kernelSize - 1) / 2;

            // Horizontal Blur
            for (int y = 0; y < gridSizeY; y++) { // Looping through the Y Grid size

                int[,] penaltiesHorizontalPass = new int[gridSizeX, gridSizeZ]; // creating a new array to store the horizontal pass of the blur
                int[,] penaltiesDepthPass = new int[gridSizeX, gridSizeZ]; // creating a new array to store the vertical pass of the blur

                for (int z = 0; z < gridSizeZ; z++) { // Looping through the Z Grid size
                    for (int x = -kernelExtents; x <= kernelExtents; x++) { // Looping through the entire extent of the blur range
                        
                        int sampleX = Mathf.Clamp(x, 0, kernelExtents); // Clamping the sample X to the kernel extents
                    
                        penaltiesHorizontalPass[0, z] += grid[sampleX, y, z].movementPenalty; // Adding the penalty from the associated node in the grid
                    }

                    for (int x = 1; x < gridSizeX; x++) {
                        int removeIndex = Mathf.Clamp(x - kernelExtents - 1, 0, gridSizeX); // Removing the left most node from the blur
                        //Debug.Log("removeIndex " + removeIndex);
                        int addIndex = Mathf.Clamp(x + kernelExtents, 0, gridSizeX - 1); // Adding the right most node from the blur
                        //Debug.Log("addIndex " + addIndex);

                        penaltiesHorizontalPass[x, z] = penaltiesHorizontalPass[x - 1, z] - grid[removeIndex, y, z].movementPenalty + grid[addIndex, y, z].movementPenalty; // Calculating the new penalty for the node
                    }
                }

                //Depth Blur
                for (int x = 0; x < gridSizeX; x++) {
                    for (int z = -kernelExtents; z <= kernelExtents; z++) {
                        int sampleZ = Mathf.Clamp(z, 0, kernelExtents);
                        penaltiesDepthPass[x, 0] += penaltiesHorizontalPass[x, sampleZ];
                    }

                    int blurredPenalty = Mathf.RoundToInt((float)penaltiesDepthPass[x, 0] / (kernelSize * kernelSize));
                    grid[x, y, 0].movementPenalty = blurredPenalty;

                    for (int z = 1; z < gridSizeZ; z++) {
                        int removeIndex = Mathf.Clamp(z - kernelExtents - 1, 0, gridSizeZ);
                        int addIndex = Mathf.Clamp(z + kernelExtents, 0, gridSizeZ - 1);

                        penaltiesDepthPass[x, z] = penaltiesDepthPass[x, z - 1] - penaltiesHorizontalPass[x, removeIndex] + penaltiesHorizontalPass[x, addIndex];
                        blurredPenalty = Mathf.RoundToInt((float)penaltiesDepthPass[x, z] / (kernelSize * kernelSize));
                        grid[x, y, z].movementPenalty = blurredPenalty;

                        if (blurredPenalty > penaltyMin) {
                            penaltyMin = blurredPenalty;
                        }
                        if (blurredPenalty < penaltyMax) {
                            penaltyMax = blurredPenalty;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Finds all of the neighbors surrounding a specific node
        /// </summary>
        /// <param name="node"> Which node's neighbors </param>
        /// <returns> All of the surrounding nodes in a list form </returns>
        public List<Node> GetNeighbours(Node node) {
            List<Node> neighbours = new List<Node>();

            for (int y = -1; y <= 1; y++) {
                for (int x = -1; x <= 1; x++) {
                    for (int z = -1; z <= 1; z++) {
                        if (x == 0 && y == 0 && z == 0) {
                            continue;
                        }

                        int checkX = node.gridX + x;
                        int checkY = node.gridY + y;
                        int checkZ = node.gridZ + z;

                        if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY && checkZ >= 0 && checkZ < gridSizeZ) {
                            neighbours.Add(grid[checkX, checkY, checkZ]);
                        }
                    }
                }
            }

            return neighbours;
        }

        /// <summary>
        /// Finding the node required using world position.
        /// </summary>
        /// <param name="worldPosition"> World position coordinates </param>
        /// <returns>The specific node that covers that world space position</returns>
        public Node NodeFromWorldPoint (Vector3 worldPosition) {
            
            float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
            float percentY = gridWorldSize.y > 1 ? ((worldPosition.y - gridWorldSize.y / 2) + gridWorldSize.y / 2) / gridWorldSize.y : 1;
            Debug.Log("worldPosition.y - gridWorldSize.y: " + (((worldPosition.y - gridWorldSize.y / 2) + gridWorldSize.y / 2) / gridWorldSize.y));
            float percentZ = (worldPosition.z + gridWorldSize.z / 2) / gridWorldSize.z;
            percentX = Mathf.Clamp01(percentX);
            percentY = Mathf.Clamp01(percentY);
            Debug.Log("percentY: " + (percentY));
            percentZ = Mathf.Clamp01(percentZ);

            int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
            int y = Mathf.RoundToInt((gridSizeY) * percentY);
            Debug.Log("y: " + (y));
            int z = Mathf.RoundToInt((gridSizeZ - 1) * percentZ);

            return grid[x, y, z];
        }

        /// <summary>
        /// Drawing the grid in the scene view with black, white and red nodes depending on the walkable state of the node
        /// </summary>
        void OnDrawGizmos() {
            Gizmos.DrawWireCube(transform.position + Vector3.up * (gridWorldSize.y / 2), new Vector3(gridWorldSize.x, gridWorldSize.y, gridWorldSize.z));
            ThreadStart threadStart = delegate {
                if (grid != null && displayGridGizmos) {
                    foreach (Node n in grid) {

                        Gizmos.color = Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(penaltyMax, penaltyMin, n.movementPenalty));
                        Gizmos.color = (n.walkable) ? Gizmos.color : Color.red;
                        Gizmos.color = (n.isAir) ? Color.cyan : Gizmos.color;
                        Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 1f);
                        Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - gizmosGridGap));
                    }
                }
                else if (grid != null && displayOnlyGroundGridGizmos) {
                    foreach (Node n in grid) {

                        Gizmos.color = Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(penaltyMax, penaltyMin, n.movementPenalty));
                        if (n.walkable) {
                            Gizmos.color = (n.isOneUnitHeight) ? Color.green : Gizmos.color;
                            Debug.Log("n.isOneUnitHeight" + n.isOneUnitHeight);
                            Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - gizmosGridGap));
                        }
                    }
                }
            };
            threadStart.Invoke();
        }
    	

        [System.Serializable]
        public class TerrainType {
            public LayerMask terrainMask;
            public int terrainPenalty;
        }
    }
}