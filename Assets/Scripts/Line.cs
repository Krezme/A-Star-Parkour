using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AStar {
    public struct Line
    {
        const float verticalLineGradient = 1e5f;

        float gradient;
        float z_intercept;

        Vector3 pointOnLine_1;
        Vector3 pointOnLine_2;
        
        float lineHeight;

        float gradientPerpendicular;

        bool approachSide;

        public Line(Vector3 pointOnLine, Vector3 pointPerpendicularToLine)
        {
            float dx = pointOnLine.x - pointPerpendicularToLine.x;
            lineHeight = pointOnLine.y;
            float dz = pointOnLine.z - pointPerpendicularToLine.z;

            if (dx == 0)
            {
                gradientPerpendicular = verticalLineGradient;
            }
            else
            {
                gradientPerpendicular = dz / dx;
            }

            if (gradientPerpendicular == 0)
            {
                gradient = verticalLineGradient;
            }
            else
            {
                gradient = -1 / gradientPerpendicular;
            }

            z_intercept = pointOnLine.z - gradient * pointOnLine.x;
            pointOnLine_1 = pointOnLine;
            pointOnLine_2 = pointOnLine + new Vector3(1, 0, gradient);

            approachSide = false;
            approachSide = GetSide(pointPerpendicularToLine);
        }

        bool GetSide(Vector3 p)
        {
            return (p.x - pointOnLine_1.x) * (pointOnLine_2.z - pointOnLine_1.z) > (p.z - pointOnLine_1.z) * (pointOnLine_2.x - pointOnLine_1.x);
        }

        public bool HasCrossedLine(Vector3 p)
        {
            return GetSide(p) != approachSide;
        }

        /// <summary>
        /// Used to find the distance to target
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public float DistanceFromPoint(Vector3 p){
            float z_interceptPerpendicular = p.z - gradientPerpendicular * p.x;
            float intersectX = (z_interceptPerpendicular - z_intercept) / (gradient - gradientPerpendicular);
            float intersectZ = gradient * intersectX + z_intercept;

            return Vector3.Distance(p, new Vector3(intersectX, lineHeight, intersectZ)); //! This "p.y" is a place holder. It will cause issues down the line if it is not changed. Please Change it!!!
        }

        public void DrawWithGizmos(float length){
            Vector3 lineDir = new Vector3(1, 0, gradient).normalized;
            Vector3 lineCenter = new Vector3(pointOnLine_1.x, lineHeight, pointOnLine_1.z) + Vector3.up;
            Gizmos.DrawLine(lineCenter - lineDir * length / 2f, lineCenter + lineDir * length / 2f);
        }
    }
}