using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AStar {
    public class Unit : MonoBehaviour
    {
        const float minPathUpdateTime = .2f;
        const float pathUpdateMoveThreshold = 0.5f;

        public Transform target;

        public float speed = 5;
        public float turnSpeed = 3;
        public float turnDst = 5;
        public float stoppingDst = 10;
        public CharacterController controller;

        Path path;

        void Start() {
            StartCoroutine(UpdatePath());
        }

        public void OnPathFound(Vector3[] waypoints, bool pathSuccessful){
            if (pathSuccessful)
            {
                path = new Path(waypoints, transform.position, turnDst, stoppingDst);
                StopCoroutine("FollowPath");
                StartCoroutine("FollowPath");
            }
        }


        /// <summary>
        /// Updates the path if the target pos has changed
        /// </summary>
        IEnumerator UpdatePath() {

            if (Time.timeSinceLevelLoad < .3f) {
                yield return new WaitForSeconds(.3f);
            }
            PathRequestManager.RequestPath(new PathRequest (transform.position, target.position, OnPathFound));

            float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
            Vector3 targetPosOld = target.position;

            while (true){
                yield return new WaitForSeconds(minPathUpdateTime);
                if ((target.position - targetPosOld).sqrMagnitude > sqrMoveThreshold)
                {
                    PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));
                    targetPosOld = target.position;
                }
            }
        }

        IEnumerator FollowPath() {

            bool followingPath = true;
            int pathIndex = 0;

            transform.LookAt(path.lookPoints[0]);

            float speedPercent = 1;

            while (followingPath) {
                Vector3 pos3D = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                while (path.turnBoundaries[pathIndex].HasCrossedLine(pos3D)) {
                    if (pathIndex == path.finishLineIndex) {
                        followingPath = false;
                        break;
                    }
                    else {
                        pathIndex++;
                    }
                }

                if (followingPath) {

                    // Calculates the speed percentage based on the distance to the target
                    if (pathIndex >= path.slowDownIndex && stoppingDst > 0) {
                        speedPercent = Mathf.Clamp01(path.turnBoundaries[path.finishLineIndex].DistanceFromPoint(pos3D) / stoppingDst);
                        if (speedPercent < 0.01f) {
                            followingPath = false;
                        }
                    }

                    // Moves and Rotates the AI
                    Quaternion targetRotation = Quaternion.LookRotation(new Vector3((path.lookPoints[pathIndex] - transform.position).x, 0, (path.lookPoints[pathIndex] - transform.position).z));
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                    //transform.Translate(Vector3.forward * Time.deltaTime * speed * speedPercent, Space.Self);
                    controller.Move(new Vector3(transform.forward.x, 0, transform.forward.z) * Time.deltaTime * speed * speedPercent);
                }
                yield return null;
            }
        }

        public void OnDrawGizmos() {
            if (path != null)
            {
                path.DrawWithGizmos();
            }
        }
    }
}