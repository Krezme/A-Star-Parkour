using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AStar {
    public class Pathfinding : MonoBehaviour
    {
        public static Pathfinding instance;

        Grid grid;

        void Awake() {
            if (instance != null) {
                Debug.LogError("More than one Pathfinding in scene!");
            }else {
                instance = this;
            }

            grid = GetComponent<Grid>();
        }

        void Update () {

        }

        public void FindPath (PathRequest request, Action<PathResult> callback) {
            Vector3[] waypoints = new Vector3[0];
            bool pathSuccess = false;

            Node startNode = Grid.instance.NodeFromWorldPoint(request.pathStart);
            Node targetNode = Grid.instance.NodeFromWorldPoint(request.pathEnd);

            if (startNode.walkable && targetNode.walkable) {
                Heap<Node> openSet = new Heap<Node>(Grid.instance.MaxSize);
                HashSet<Node> closedSet = new HashSet<Node>();

                openSet.Add(startNode);

                while(openSet.Count > 0) {
                    Node currentNode = openSet.RemoveFirst();
                    closedSet.Add(currentNode);

                    if (currentNode == targetNode) {
                        pathSuccess = true;
                        
                        break;
                    }

                    foreach (Node neighbour in Grid.instance.GetNeighbours(currentNode)) {
                        if (!neighbour.walkable || closedSet.Contains(neighbour)) {
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
            }
            if (pathSuccess) {
                waypoints = RetracePath(startNode, targetNode);
                pathSuccess = waypoints.Length > 0;
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
            Vector3[] waypoints = SimplifyPath(path);
            Array.Reverse(waypoints);
            return waypoints;
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