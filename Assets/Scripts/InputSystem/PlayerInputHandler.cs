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
    bool paused = false; // if true, game is paused
    bool noMove = false; // if true, player cannot move
    public delegate void HandlePause(bool pauseValue);
    public event HandlePause OnPauseInput;

    public override void OnMoveInput(InputAction.CallbackContext context)
    {
        if (paused || noMove)
        {
            return;
        }
        // read in planar movement information
        currentInput.planeMove = context.ReadValue<Vector2>().normalized;
    }

    public override void OnJumpInput(InputAction.CallbackContext context)
    {
        if (paused || noMove)
        {
            return;
        }
        // set jump info
        currentInput.jumpMove = context.performed;
    }

    public void OnTogglePauseInput(InputAction.CallbackContext context)
    {
        // since this is on the player, can we maybe just trigger an event for this
        paused = !paused;
        OnPauseInput?.Invoke(paused);
        Debug.Log("pause variable: " + paused);
    }
    public void ToggleMovementInput(bool value)
    {
        noMove = value;
    }
}
