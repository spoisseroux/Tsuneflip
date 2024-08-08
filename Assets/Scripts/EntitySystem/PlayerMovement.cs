using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Player class
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : EntityMovement
{
    [SerializeField] public PlayerInputHandler playerInput; // receive input from InputSystem
    [SerializeField] public CharacterController character; // character controller

    [SerializeField] public Camera thirdPersonCamera; // for camera-relative movement

    [SerializeField] public Vector3 currentDirection; // for storing last frame's info
    [SerializeField] public Vector3 workingDirection; // edits new frame's info
    public Vector3 relativeRightVector;
    public Vector3 relativeForwardVector;
    public Vector3 movementUpVector; // how do we actually keep this in world space???

    [SerializeField] private BoxCollider footCollider; // for grounded check & enemy head collider check

    private float turnSmoothVelocity;

    // raycast below for things

    #region Monobehaviour Functions
    protected override void Start()
    {
        playerInput = GetComponent<PlayerInputHandler>();
        character = GetComponent<CharacterController>();
        grounded = GroundedCheck();
        
    }

    protected override void Update()
    {
        base.Update();
        grounded = GroundedCheck();
        workingDirection = currentDirection; // update our working direction to hold last frame's info

        // apply gravity to workingdir
        ApplyGravity();
        // apply jump to workingdir
        Jump();
        // set rotation
        SetRotation();
        // apply movement to workingdir
        Move(playerInput.currentInput.planeMove);
        // combine all into workingDirection
        SetWorkingDirectionVector();
        // set currentdir
        currentDirection = workingDirection; // update our current direction to the calculated magnitude
        // move
        character.Move(currentDirection * Time.deltaTime);
    }
    #endregion


    public override void Jump()
    {
        if (playerInput.currentInput.jumpMove && grounded)
        {
            //workingDirection.Set(workingDirection.x, constants.jumpSpeed, workingDirection.z);

            movementUpVector = Vector3.up * constants.jumpSpeed;
        }
    }

    // SOMETHING WRONG HERE... NEED TO FIGURE OUT WHETHER FINAL VECTOR IS IN WORLD OR LOCAL BEFORE CALLING CharacterController.Move()
    public override void Move(Vector2 move)
    {
        // create movement basis vectors from camera's coordinate space
        Vector3 right = transform.InverseTransformVector(thirdPersonCamera.transform.right);
        Vector3 forward = transform.InverseTransformVector(thirdPersonCamera.transform.forward);
        right.y = 0;
        forward.y = 0;
        right = right.normalized;
        forward = forward.normalized;

        relativeRightVector = move.x * constants.moveSpeed * right;
        relativeForwardVector = move.y * constants.moveSpeed * forward;
        

        //SetWorkingX(move.x * constants.moveSpeed);
        //SetWorkingZ(move.y * constants.moveSpeed);
    }

    protected override void ApplyGravity()
    {
        if (!grounded)
        {
            // apply ascent or descent gravity
            float newVel = workingDirection.y;
            // ascent
            if (newVel >= 0)
            {
                newVel += (constants.ascentGravity * Time.deltaTime);
            }
            // descent
            else
            {
                newVel += (constants.descentGravity * Time.deltaTime);
            }
            newVel = Mathf.Max(newVel, -20.0f);
            //workingDirection.Set(workingDirection.x, newVel, workingDirection.z);
            movementUpVector = Vector3.up * newVel;
        }
        else
        {
            //workingDirection.Set(workingDirection.x, constants.groundedGravity, workingDirection.z);
            movementUpVector = Vector3.up * constants.groundedGravity;
        }
    }

    public override bool GroundedCheck()
    {
        // returns true if grounded
        return Physics.CheckSphere(transform.position + (Vector3.down * 0.2f), character.radius * 0.9f, groundLayer);
    }


    public void SetRotation()
    {
        // set a look direction
        if (playerInput.currentInput.planeMove.magnitude >= 0.05f)
        {
            Debug.Log("Update facing angle");
            // smooth damp to target angle
            float targetAngle = Mathf.Atan2(currentDirection.x, currentDirection.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, constants.turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);
        }
        // set facing direction
        facingDir = transform.rotation;
    }


    private void SetWorkingDirectionVector()
    {
        workingDirection = relativeRightVector + relativeForwardVector + movementUpVector;
    }







    private void SetWorkingX(float x)
    {
        // right + up and left vectors
        Vector3 xDir = thirdPersonCamera.transform.right * x;
        workingDirection.Set(x, workingDirection.y, workingDirection.z);
    }

    private void SetWorkingZ(float z)
    {
        workingDirection.Set(workingDirection.x, workingDirection.y, z);
    }

}
