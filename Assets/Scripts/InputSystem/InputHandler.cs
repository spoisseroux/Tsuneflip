using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public struct EntityInput
{
    public Vector2 planeMove; // { x --> player X, y --> player Z }, grid-aligned movement
    public float normalizedX, normalizedZ; // can we comment this out?
    public bool jumpMove; // y input, jumping
    // ability press here
}

/*
public struct MenuInput
{
    public bool enterPress; // true when selecting and moving into menu object
    public bool exitPress; // true when backing out of given menu
    public int scrollMenu; // { if == 1, go up | if == -1, go down }
}
*/

// Abstract class for receiving & processing Input for any inheriting GameObject (Player, Menu, etc.)
public abstract class InputHandler : MonoBehaviour
{
    
}

/*
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
*/

public abstract class EntityInputHandler : InputHandler
{
    // input data
    public EntityInput currentInput;

    // input system functions
    public abstract void OnMoveInput(InputAction.CallbackContext context);
    public abstract void OnJumpInput(InputAction.CallbackContext context);
}
