using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

namespace AStar {
    public class PathRequestManager : MonoBehaviour
    {
        Queue<PathResult> results = new Queue<PathResult>();
        public static PathRequestManager instance;
        Pathfinding pathfinding;

        void Awake()
        {
            if(instance != null)
            {
                Debug.LogError("More than one PathRequestManager in scene!");
            }
            else {
                instance = this;
            }
            pathfinding = GetComponent<Pathfinding>();
        }

        CancellationTokenSource cts;

        void Start () {

        }

        void Update () {
            if (results.Count > 0)
            {
                int itemsInQueue = results.Count;
                lock (results)
                {
                    for (int i = 0; i < itemsInQueue; i++)
                    {
                        PathResult result = results.Dequeue();
                        result.callback(result.path, result.success);
                    }
                }
            }
        }

        public static async void RequestPathAsync(PathRequest request){
            if (instance.cts != null) {
                instance.cts.Cancel();
            }
            instance.cts = new CancellationTokenSource();
            await Task.Run(() => {
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                instance.pathfinding.FindPathAsync(request, instance.FinishedProcessingPath, instance.cts);
                sw.Stop();
                Debug.Log("Path found in: " + sw.ElapsedMilliseconds + " ms");
            }, instance.cts.Token);
        }

        public void FinishedProcessingPath(PathResult result) {
            lock (results)
            {
                results.Enqueue(result);
            }
        }  
    }

    public struct PathResult {
            public Vector3[] path;
            public bool success;
             public Action<Vector3[], bool> callback;

            public PathResult(Vector3[] path, bool success, Action<Vector3[], bool> callback){
                this.path = path;
                this.success = success;
                this.callback = callback;
            }
        }

    public struct PathRequest{
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;

        public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback)
        {
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
        }
    }
}

