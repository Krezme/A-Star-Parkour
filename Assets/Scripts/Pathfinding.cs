using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace AStar {
    public class Pathfinding : MonoBehaviour
    {
        public static Pathfinding instance;

        public int jumpHeight;

        public int amountOfImmediateAirMovementPossible;
        public int slideLength;

        void Awake() {
            if (instance != null) {
                Debug.LogError("More than one Pathfinding in scene!");
            }else {
                instance = this;
            }
        }

        void Update () {

        }

        public void FindPathAsync (PathRequest request, Action<PathResult> callback, CancellationTokenSource cts, Node[,,] personalGrid) {
            Vector3[] waypoints = new Vector3[0];
            bool pathSuccess = false;

            Node startNode = Grid.instance.NodeFromWorldPoint(request.pathStart, personalGrid);
            Node targetNode = Grid.instance.NodeFromWorldPoint(request.pathEnd, personalGrid);

            if (startNode.walkable && targetNode.walkable) {
                Heap<Node> openSet = new Heap<Node>(Grid.instance.MaxSize);
                HashSet<Node> closedSet = new HashSet<Node>();

                openSet.Add(startNode);
                startNode.currentVerticalAirTime = 0;
                startNode.initialPossibleAirMovement = 0;
                
                while(openSet.Count > 0) {
                    Node currentNode = openSet.RemoveFirst();
                    closedSet.Add(currentNode);
                    if (cts.IsCancellationRequested) {
                        Debug.Log("Cancellation Token Requested");
                        return;
                    }

                    if (currentNode == targetNode) {
                        pathSuccess = true;
                        Debug.Log("break");
                        break;
                    }

                    //Condition for navigating through air Nodes
                    if (currentNode.isAir) {
                        if (currentNode.gridY < currentNode.parent.gridY) {
                            currentNode.currentVerticalAirTime = currentNode.parent.currentVerticalAirTime;
                        }
                        else {
                            currentNode.currentVerticalAirTime = currentNode.parent.currentVerticalAirTime + 1;
                        }

                        if (currentNode.initialPossibleAirMovement <= amountOfImmediateAirMovementPossible) {
                            currentNode.initialPossibleAirMovement = currentNode.parent.initialPossibleAirMovement + 1;
                        }
                    }

                    //Condition for sliding down
                    if (currentNode.isOneUnitHeight) {
                        if (currentNode.parent != null) {
                            if (currentNode.gridY == currentNode.parent.gridY) {
                                currentNode.currentSlideLength = currentNode.parent.currentSlideLength + 1;
                            }
                            else {
                                currentNode.currentSlideLength = jumpHeight + 1; //Setting to a value that is not allowed so it will not use it to slide 
                            }
                        }
                    }

                    //Resetting the air time when landed and slide when not on a isOneUnitHeight Node
                    if (!currentNode.isAir && currentNode.walkable) {
                        currentNode.currentVerticalAirTime = 0;
                        if (!currentNode.isOneUnitHeight) {
                            currentNode.currentSlideLength = 0;
                        }
                    }
                    
                    foreach (Node neighbour in Grid.instance.GetNeighbours(currentNode, personalGrid)) {
                        if (cts.IsCancellationRequested) {
                            Debug.Log("Cancellation Token Requested");
                            return;
                        }

                        if ((!neighbour.walkable && !neighbour.isAir) || closedSet.Contains(neighbour)) {
                            continue;
                        }

                        //Setting Node to not viable when it is using too much air time
                        if (currentNode.currentVerticalAirTime >= jumpHeight -1 && ((!neighbour.walkable && neighbour.isAir) && neighbour.gridY >= currentNode.gridY)) {//! Look into this condition and fix it
                            continue;
                        }

                        // The path will now allow it to move any other way than straight down after it has used up all of its initial air movement
                        if (currentNode.initialPossibleAirMovement > amountOfImmediateAirMovementPossible && ((Vector3.Distance(new Vector3(currentNode.gridX, 0, 0), new Vector3(neighbour.gridX, 0, 0)) > 0) || (Vector3.Distance(new Vector3(0, 0, currentNode.gridZ), new Vector3(0, 0, neighbour.gridZ)) > 0) || (neighbour.gridY > currentNode.gridY)) ) {
                            continue;
                        }

                        if (currentNode.currentSlideLength == slideLength && (neighbour.isOneUnitHeight || neighbour.gridY > currentNode.gridY || !neighbour.walkable)) {
                            continue;
                        }

                        int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.movementPenalty;
                        if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {

                            neighbour.gCost = newMovementCostToNeighbour;
                            neighbour.hCost = GetDistance(neighbour, targetNode);
                            neighbour.parent = currentNode;

                            if (!openSet.Contains(neighbour)) {
                                
                                openSet.Add(neighbour);
                            }
                        }
                    }
                }
                Debug.Log("!!!");
            }
            else {
                Debug.Log("Start or End node is not walkable!: " + startNode.walkable + " " + targetNode.walkable);
            }
            if (pathSuccess) {
                Debug.Log("RetracePath");
                waypoints = RetracePath(startNode, targetNode);
                pathSuccess = waypoints.Length > 0;
            }
            else {
                Debug.Log("Path not found!");
            }

            if (cts.IsCancellationRequested) {
                Debug.Log("Cancellation Token Requested");
                return;
            }

            callback(new PathResult(waypoints, pathSuccess, request.callback));
        }



        int GetDistance(Node nodeA, Node nodeB) {
            int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
            int dstY = Mathf.Abs(nodeA.gridZ - nodeB.gridZ);

            if (dstX > dstY) {
                return 14 * dstY + 10 * (dstX - dstY);
            }
            return 14 * dstX + 10 * (dstY - dstX);
        }

        Vector3[] RetracePath(Node startNode, Node endNode) {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while (currentNode != startNode) {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }
            Vector3[] waypoints = PathToWorldPoints(path);
            Array.Reverse(waypoints);
            return waypoints;
        }

        Vector3[] PathToWorldPoints(List<Node> path) {
            List<Vector3> waypoints = new List<Vector3>();
            Vector2 directionOld = Vector2.zero;

            for (int i = 1; i < path.Count; i++) {
                waypoints.Add(path[i].worldPosition);
            }
            return waypoints.ToArray();
        }

        Vector3[] SimplifyPath(List<Node> path) {
            List<Vector3> waypoints = new List<Vector3>();
            Vector2 directionOld = Vector2.zero;

            for (int i = 1; i < path.Count; i++) {
                Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridZ - path[i].gridZ);
                if (directionNew != directionOld) {
                    waypoints.Add(path[i].worldPosition);
                }
                directionOld = directionNew;
            }
            return waypoints.ToArray();
        }
    }
}