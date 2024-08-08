using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//[RequireComponent(typeof(PlayerMovement))]
public class PlayerInputHandler : EntityInputHandler
{
    // trigger events instead of requiring a check every frame of Update?
    //public delegate void OnJump(bool val);
    //public event OnJump Jump;

    //public delegate void OnMove(Vector2 xz);
    //public event OnMove Move;

    public override void OnMoveInput(InputAction.CallbackContext context)
    {
        // read in planar movement information
        currentInput.planeMove = context.ReadValue<Vector2>().normalized;
    }

    public override void OnJumpInput(InputAction.CallbackContext context)
    {
        // set jump info
        currentInput.jumpMove = context.performed; 
    }
}
