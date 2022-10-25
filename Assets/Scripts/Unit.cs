using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AStar {
    public class Unit : MonoBehaviour
    {
        public Transform target;

        public float speed = 5;
        public float turnDst = 5;

        Path path;

        void Start() {
            PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
        }

        public void OnPathFound(Vector3[] waypoints, bool pathSuccessful){
            if (pathSuccessful)
            {
                path = new Path(waypoints, transform.position, turnDst);
                StopCoroutine("FollowPath");
                StartCoroutine("FollowPath");
            }
        }

        IEnumerator FollowPath() {

            while (true)
            {
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