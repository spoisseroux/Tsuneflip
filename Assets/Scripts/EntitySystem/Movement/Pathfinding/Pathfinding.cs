using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attach this script as a component to the GameObject holding the GridManager
// TODO: redo to add a jumpable component, height difference component
     //  basically, can i get up this high??? if not, not reachable... configurable to the NPC's stats)
public class Pathfinding : MonoBehaviour
{
    PathRequestManager requestManager;
    GridManager grid;

    #region Monobehaviors
    void Awake()
    {
        grid = GetComponent<GridManager>();
        requestManager = GetComponent<PathRequestManager>();
    }
    #endregion

    #region Pathfinding Functions using A*

    public void StartFindPath(Vector3 startPos, Vector3 targetPos)
    {
        StartCoroutine(FindPath(startPos, targetPos));
    }

    IEnumerator FindPath(Vector3 startPosition, Vector3 targetPosition)
    {
        // initialize starting data
        Tile startNode = grid.GetTileFromWorldSpace(startPosition);
        Tile targetNode = grid.GetTileFromWorldSpace(targetPosition);

        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        // if we can get to both nodes, begin pathfinding
        if (startNode.walkable && targetNode.walkable)
        {
            List<Tile> openSet = new List<Tile>();
            HashSet<Tile> closedSet = new HashSet<Tile>();
            openSet.Add(startNode);

            // while still unvisited nodes
            while (openSet.Count > 0)
            {
                // find neighboring nodes with lowest f-cost
                Tile node = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost)
                    {
                        // pick based on lower distance to target node
                        if (openSet[i].hCost < node.hCost)
                        {
                            node = openSet[i];
                        }
                    }
                }
                // remove current from open and move to close
                openSet.Remove(node);
                closedSet.Add(node);

                // check we reached target node
                if (node == targetNode)
                {
                    pathSuccess = true;
                    //RetracePath(startNode, targetNode);
                    break;
                }

                // add neighbors
                foreach (Tile neighbor in grid.GetNeighboringTiles(node))
                {
                    if (!neighbor.walkable || closedSet.Contains(neighbor))
                        continue;

                    int newCostToNeighbor = node.gCost + GetDistance(node, neighbor);
                    // finds new gCost to neighbor, from current node
                    // gCost current + distance (curr->neighbor) == gCost to neighbor
                    // if new gCost is less than before
                    if (newCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                    {
                        neighbor.gCost = newCostToNeighbor;
                        neighbor.hCost = GetDistance(neighbor, targetNode);
                        neighbor.parent = node;

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Add(neighbor);
                        }
                    }
                }
            }

            yield return null;
            if (pathSuccess)
            {
                waypoints = RetracePath(startNode, targetNode);
            }
            requestManager.FinishedProcessingPath(waypoints, pathSuccess);
        }
    }

    // retraces path from start to target
    Vector3[] RetracePath(Tile startNode, Tile endNode)
    {
        List<Tile> path = new List<Tile>();
        Tile currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;
    }

    // converts path of Tile nodes into Vector3 waypoints, trims out regions with no change in direction
    Vector3[] SimplifyPath(List<Tile> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 oldDir = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            // check for change in direction between last marked waypoint and next pathfinding node
            Vector2 newDir = new Vector2(path[i-1].gridX - path[i].gridX, path[i-1].gridY - path[i].gridY);
            if (newDir != oldDir)
            {
                waypoints.Add(path[i].nodePosition);
            }
            oldDir = newDir;
        }
        return waypoints.ToArray();
    }

    int GetDistance(Tile a, Tile b)
    {
        int dX = Mathf.Abs(a.gridX - b.gridX);
        int dY = Mathf.Abs(a.gridY - b.gridY);

        if (dX > dY)
            return 14 * dY + 10 * (dX - dY);
        return 14 * dX + 10 * (dY - dX);
    }

    #endregion
}
