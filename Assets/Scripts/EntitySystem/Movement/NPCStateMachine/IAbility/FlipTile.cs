using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipTile : IAbility
{
    private float castTime;
    private string abilityName;
    private EntityMovement source;

    GridManager grid;

    public float CastTime { get => castTime; set => castTime = value; }
    public string AbilityName { get => abilityName; set => abilityName = value; }
    public EntityMovement Source { get => source; set => source = value; }

    public FlipTile(float time, string name, EntityMovement src)
    {
        castTime = time;
        abilityName = name;
        source = src;

        // find grid, CHECK SINGLETON ISNT MESSING UP
        grid = GameObject.Find("Grid").GetComponent<GridManager>();
    }

    public void CastAbility(Transform caster)
    {
        // request flip
        grid.RequestTileFlip(caster.position, source);
    }
}
