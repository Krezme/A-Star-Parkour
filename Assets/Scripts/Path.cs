using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AStar {
    public class Path
    {
        public readonly Vector3[] lookPoints;

        public readonly Line[] turnBoundaries;

        public readonly int finishLineIndex;
        public readonly int slowDownIndex;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="waypoints"></param>
        /// <param name="startPos"></param>
        /// <param name="turnDst"></param>
        /// <param name="stoppingDst">AI stopping distance</param>
        public Path(Vector3[] waypoints, Vector3 startPos, float turnDst, float stoppingDst)
        {
            lookPoints = waypoints;
            turnBoundaries = new Line[lookPoints.Length];
            finishLineIndex = turnBoundaries.Length - 1;

            Vector3 previousPoint = startPos;
            for (int i = 0; i < lookPoints.Length; i++)
            {
                Vector3 currentPoint = lookPoints[i];
                Vector3 dirToCurrentPoint = (currentPoint - previousPoint).normalized;
                Vector3 turnBoundaryPoint = (i == finishLineIndex) ? currentPoint : currentPoint - dirToCurrentPoint * turnDst;
                turnBoundaries[i] = new Line(turnBoundaryPoint, previousPoint - dirToCurrentPoint * turnDst);
                previousPoint = turnBoundaryPoint;
            }

            // Slows down the AI when it gets close to the target
            float dstFromEndPoint = 0;
            for (int i = lookPoints.Length - 1; i > 0; i--)
            {
                dstFromEndPoint += Vector3.Distance(lookPoints[i], lookPoints[i - 1]);
                if (dstFromEndPoint > stoppingDst)
                {
                    slowDownIndex = i;
                    break;
                }
            }
        }

        Vector2 V3ToV2(Vector3 v3)
        {
            return new Vector2(v3.x, v3.z);
        }

        public void DrawWithGizmos()
        {
            Gizmos.color = Color.black;
            foreach (Vector3 p in lookPoints)
            {
                Gizmos.DrawCube(p, Vector3.one);
            }
            Gizmos.color = Color.blue;
            foreach (Line l in turnBoundaries)
            {
                l.DrawWithGizmos(10);
            }
        }
    }
}