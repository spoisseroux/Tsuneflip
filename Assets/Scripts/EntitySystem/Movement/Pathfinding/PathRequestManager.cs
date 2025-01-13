using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathRequestManager : MonoBehaviour
{
    // queue of requests
    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentRequest;

    // singleton instance
    static PathRequestManager instance;
    Pathfinding pathfinding;

    // status variable
    bool inProcess;

    void Awake()
    {
        instance = this;
        pathfinding = GetComponent<Pathfinding>();
    }

    // request a new Path
    public static void RequestPath(Vector3 start, Vector3 target, Action<Vector3[], bool> callback)
    {
        Debug.Log("path requested");
        PathRequest request = new PathRequest(start, target, callback);
        instance.pathRequestQueue.Enqueue(request);
        instance.TryNextPath();
    }

    // try to process next PathRequest
    void TryNextPath()
    {
        if (!inProcess && pathRequestQueue.Count > 0)
        {
            currentRequest = pathRequestQueue.Dequeue();
            inProcess = true;
            pathfinding.StartFindPath(currentRequest.start, currentRequest.target);
        }
    }


    // check path is finished processing
    public void FinishedProcessingPath(Vector3[] path, bool success)
    {
        Debug.Log("path finished");
        currentRequest.callback(path, success);
        inProcess = false;
        TryNextPath();
    }

    struct PathRequest
    {
        public Vector3 start;
        public Vector3 target;
        public Action<Vector3[], bool> callback;

        public PathRequest(Vector3 _start, Vector3 _target, Action<Vector3[], bool> _call)
        {
            start = _start;
            target = _target;
            callback = _call;
        }
    }
}
