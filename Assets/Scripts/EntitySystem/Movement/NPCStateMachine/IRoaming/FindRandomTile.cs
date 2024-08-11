using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindRandomTile : IRoaming
{
    private float maxDistanceAway;
    private float minDistanceAway;

    public FindRandomTile(float distMin, float distMax)
    {
        minDistanceAway = distMin;
        maxDistanceAway = distMax;
    }

    // produces a vector in local space, have to convert to world based on NPC's rotation and position
    public Vector2 FindNextPosition(Transform currentNPCPos)
    {
        // generate a random direction in local space for NPC, convert it to world space, normalize it
        Vector3 localRight = currentNPCPos.right;
        Vector3 localForward = currentNPCPos.forward;
        float randomWeightX = Random.Range(-1.0f, 0.25f); // emphasize finding a "newer" direction by making the random range favor negatives
        float randomWeightZ = Random.Range(-1.0f, 0.25f);
        Vector3 direction = currentNPCPos.TransformPoint(localRight * randomWeightX + localForward * randomWeightZ).normalized;

        // get a random distance away
        float randomDist = Random.Range(minDistanceAway, maxDistanceAway);

        // scale our direction the randomDist to yield new target
        Vector3 choice = direction * randomDist;
        return new Vector2(choice.x, choice.z);
    }
}
