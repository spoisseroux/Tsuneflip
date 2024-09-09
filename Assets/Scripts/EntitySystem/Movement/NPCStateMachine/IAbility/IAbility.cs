using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Interface for defining Abilities for Player and NPC
public interface IAbility
{
    // cast time
    float CastTime
    {
        get;
        set;
    }

    // ability name, for setting animator true and false and what not
    string AbilityName
    {
        get;
        set;
    }

    // cast the ability
    public abstract void CastAbility();
}

// possible other things?
    // Hitboxes/hurtboxes?
    // Animation?
    // States for different stages?
    // Movement ability stuff? (how does the dragon from vividlope zipp around the map, how does NPC lunge forward off attack dash, etc)
    // Starting location, method for calculating end location?
