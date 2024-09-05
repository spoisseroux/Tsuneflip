using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// strategy pattern interface for Roaming method of a given NPC unit
public interface IRoaming
{
    //public abstract void UseRoamingStrategy();
    public abstract Vector2 FindNextPosition(Transform currentNPCPos);
}

// possible:
//      FindFlippedTile (YES FOR NOW)
//      FindRandomTile
