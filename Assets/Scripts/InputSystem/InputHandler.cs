using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public struct EntityInput
{
    public bool moving;
    public Vector3 moveDirection;
    public Vector3 smoothMoveDirection;
    public Quaternion facingDirection;

    public Vector2 planeMovement; // { x --> player X, y --> player Z }, grid-aligned movement
    public bool jumping; // y input, jumping
}

public struct MenuInput
{
    public bool enterPress; // true when selecting and moving into menu object
    public bool exitPress; // true when backing out of given menu
    public int scrollMenu; // { if == 1, go up | if == -1, go down }
}

// Abstract class for receiving & processing Input for any inheriting GameObject (Player, Menu, etc.)
public abstract class InputHandler : MonoBehaviour
{
    
}


public class MenuInputHandler : InputHandler
{
    protected MenuInput currentInput;

    public void OnBackCommandInput(InputAction.CallbackContext context)
    {

    }

    public void OnForwardCommandInput(InputAction.CallbackContext context)
    {

    }

    public void OnScrollCommandInput(InputAction.CallbackContext context)
    {

    }
}

public abstract class EntityInputHandler : InputHandler
{
    protected EntityInput currentInput;

    // public abstract void Move()
    // public abstract void Jump()
    // public abstract void ApplyGravity() maybe better in an Entity object
}

//[RequireComponent(typeof(PlayerMovement))]
public class PlayerInputHandler : EntityInputHandler
{
    public void OnMoveInput(InputAction.CallbackContext context)
    {
        // set some info
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        // set some info
    }
}
