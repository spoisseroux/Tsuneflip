using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FindRandomTile : IRoaming
{
    private float maxDistanceAway;
    private float minDistanceAway;

    private float minX, maxX, minZ, maxZ;

    public FindRandomTile(float distMin, float distMax, float Xmin, float Xmax, float Zmin, float Zmax)
    {
        minDistanceAway = distMin;
        maxDistanceAway = distMax;
        minX = Xmin;
        maxX = Xmax;
        minZ = Zmin;
        maxZ = Zmax;
    }

    // produce the next target position for roaming NPC
    public Vector2 FindNextPosition(Transform currentNPCPos)
    {
        Vector2 randomPos = RandomPos(currentNPCPos);
        return ResolveToGrid(randomPos);//ResolveToTile(ResolveToGrid(randomPos));
    }

    // generates a random position away from our given position
    private Vector2 RandomPos(Transform currentPos)
    {
        // generate a random point
        Vector2 v2 = new Vector3(Random.Range(minX, maxX), Random.Range(minZ, maxZ));

        // scale our direction by randomDist
        Vector3 target = new Vector3(v2.x, 1.0f, v2.y);

        // do a NavMesh hit to find nearest distance on NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(target, out hit, maxX * maxZ, NavMesh.AllAreas))
        {
            target = hit.position;
        }
        return new Vector2(target.x, target.z);
    }

    // resolves generated position to our Grid
    private Vector2 ResolveToGrid(Vector2 potentialMove)
    {
        potentialMove.x = Mathf.Clamp(potentialMove.x, minX, maxX);
        potentialMove.y = Mathf.Clamp(potentialMove.y, minZ, maxZ);
        return potentialMove;
    }

    // converts any arbitrary point inside our grid to the center of the Tile it's on
    private Vector2 ResolveToTile(Vector2 gridLocation)
    {
        return new Vector2(NearestHalf(gridLocation.x), NearestHalf(gridLocation.y));
    }

    // rounding function
    private float NearestHalf(float num)
    {
        return Mathf.Round(num * 2) / 2;
    }
}
