using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AStar {
    public class Unit : MonoBehaviour
    {
        const float minPathUpdateTime = .2f;
        const float pathUpdateMoveThreshold = 0.5f;

        public Transform target;

        public PhysicsAIController physicsAIController;

        public Animator animator;

        public bool displayPathGizmos = true;

        Path path;

        void Start() {
            StartCoroutine(UpdatePath());
        }

        public void OnPathFound(Vector3[] waypoints, bool pathSuccessful){
            if (pathSuccessful)
            {
                path = new Path(waypoints, transform.position, physicsAIController.stats.turnDst, physicsAIController.stats.stoppingDst);
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
            PathRequestManager.RequestPathAsync(new PathRequest (transform.position, target.position, OnPathFound));

            float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
            Vector3 targetPosOld = target.position;

            while (true){
                yield return new WaitForSeconds(minPathUpdateTime);
                if ((target.position - targetPosOld).sqrMagnitude > sqrMoveThreshold)
                {
                    PathRequestManager.RequestPathAsync(new PathRequest(transform.position, target.position, OnPathFound));
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
                    if (pathIndex >= path.slowDownIndex && physicsAIController.stats.stoppingDst > 0) {
                        speedPercent = Mathf.Clamp01(path.turnBoundaries[path.finishLineIndex].DistanceFromPoint(pos3D) / physicsAIController.stats.stoppingDst);
                        if (speedPercent < 0.01f) {
                            followingPath = false;
                        }
                    }

                    // Moves and Rotates the AI
                    physicsAIController.RotatePlayer(new Vector3((path.lookPoints[pathIndex] - transform.position).x, 0, (path.lookPoints[pathIndex] - transform.position).z));
                    //Debug.Log((Vector3.Distance(new Vector3 (0, path.lookPoints[pathIndex].y, 0), new Vector3 (0, transform.position.y, 0)) >= physicsAIController.jumpThreshold) + " jeje") ;
                    //physicsAIController.jump = Vector3.Distance(new Vector3 (0, path.lookPoints[pathIndex].y, 0), new Vector3 (0, transform.position.y, 0)) >= physicsAIController.jumpThreshold && physicsAIController.isGrounded;
                    //transform.Translate(Vector3.forward * Time.deltaTime * speed * speedPercent, Space.Self);
                    //physicsAIController.Move(new Vector3(transform.forward.x, transform.forward.y, transform.forward.z) * Time.deltaTime * physicsAIController.stats.speed * speedPercent);
                    animator.SetFloat("Speed", physicsAIController.stats.speed * speedPercent);
                }
                yield return null;
            }
            if (!followingPath) {
                animator.SetFloat("Speed", 0);
            }
        }

        public void OnDrawGizmos() {
            if (displayPathGizmos) {
                if (path != null)
                {
                    path.DrawWithGizmos();
                }
            }
        }
    }
}