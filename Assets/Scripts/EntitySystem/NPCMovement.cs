using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NPC class
public class NPCMovement : EntityMovement
{
    // different variables 

    public override void Jump()
    {
        Debug.Log("NPC will jump");
    }

    public override void Move(Vector2 move)
    {
        Debug.Log("NPC will move in this " + move + " direction");
    }

    protected override void ApplyGravity()
    {
        // apply gravity
    }

    public override bool GroundedCheck()
    {
        throw new System.NotImplementedException();
    }
}
