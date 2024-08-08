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
    public Vector3 movementRightVector; // x plane movement
    public Vector3 movementForwardVector; // z plane movement
    public Vector3 movementUpVector; // how do we actually keep this in world space???

    [SerializeField] private BoxCollider footCollider; // for grounded check & enemy head collider check

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
        // apply movement to workingdir
        Move(playerInput.currentInput.planeMove);
        // combine all into workingDirection
        SetWorkingDirectionVector();
        // set rotation based on currentDir
        HandleRotation();
        // set currentdir
        currentDirection = workingDirection; // update our current direction to the calculated magnitude
        // move
        character.Move(currentDirection * Time.deltaTime); // EXPECTING WORLD SPACE VECTOR
    }
    #endregion


    public override void Jump()
    {
        if (playerInput.currentInput.jumpMove && grounded)
        {
            movementUpVector = Vector3.up * constants.jumpSpeed;
        }
    }

    // SOMETHING WRONG HERE... NEED TO FIGURE OUT WHETHER FINAL VECTOR IS IN WORLD OR LOCAL BEFORE CALLING CharacterController.Move()
    public override void Move(Vector2 move)
    {
        // create movement basis vectors from camera's coordinate space
        Vector3 cameraRight = thirdPersonCamera.transform.right;
        Vector3 cameraForward = thirdPersonCamera.transform.forward;
        // eliminate y components & re-normalize
        cameraRight.y = 0;
        cameraForward.y = 0;
        cameraRight = cameraRight.normalized;
        cameraForward = cameraForward.normalized;
        // translate our movement inputs into camera space
        movementRightVector = move.x * constants.moveSpeed * cameraRight;
        movementForwardVector = move.y * constants.moveSpeed * cameraForward;
    }

    /*
    private Vector3 ConvertToCameraSpace(Vector3 vectorToRotate)
    {
        Vector3 cameraRight = thirdPersonCamera.transform.right;//Camera.main.transform.right;
        Vector3 cameraForward = thirdPersonCamera.transform.forward;

        cameraRight.y = 0;
        cameraForward.y = 0;

        cameraRight = cameraRight.normalized;
        cameraForward = cameraForward.normalized;

        // rotate to camera space
        Vector3 cameraForwardX = vectorToRotate.x * cameraRight;
        Vector3 cameraForwardZ = vectorToRotate.z * cameraForward;

        return cameraForwardX + cameraForwardZ;
    }
    */

    protected override void ApplyGravity()
    {
        if (!grounded)
        {
            float newVel = workingDirection.y;
            // apply ascent or descent gravity
            if (newVel >= 0)
            {
                newVel += (constants.ascentGravity * Time.deltaTime);
            }
            else
            {
                newVel += (constants.descentGravity * Time.deltaTime);
            }
            newVel = Mathf.Max(newVel, -20.0f);
            movementUpVector = Vector3.up * newVel;
        }
        else
        {
            movementUpVector = Vector3.up * constants.groundedGravity;
        }
    }

    public override bool GroundedCheck()
    {
        // returns true if grounded
        return Physics.CheckSphere(transform.position + (Vector3.down * 0.2f), character.radius * 0.9f, groundLayer);
    }


    public void HandleRotation()
    {
        // set a look direction
        if (playerInput.currentInput.planeMove.magnitude >= 0.05f)
        {
            // make desired position to look at
            Vector3 lookAtDir;
            lookAtDir.x = workingDirection.x;
            lookAtDir.y = 0f;
            lookAtDir.z = workingDirection.z;
            // current rotation
            Quaternion facingDir = transform.rotation;
            // target rotation
            Quaternion target = Quaternion.LookRotation(lookAtDir);
            // slerp
            transform.rotation = Quaternion.Slerp(facingDir, target, constants.turnSpeed * Time.deltaTime);
        }
        // set facing direction
        facingDir = transform.rotation;
    }


    private void SetWorkingDirectionVector()
    {
        workingDirection = movementRightVector + movementForwardVector + movementUpVector;
    }
}
