using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Player class
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : EntityMovement
{
    // scriptableObject for movement

    public override void Jump()
    {
        Debug.Log("Player will jump");
    }

    public override void Move(Vector3 move)
    {
        Debug.Log("Player will move in this" + move + " direction");
    }

    protected override void ApplyGravity()
    {
        // apply gravity
    }
}
