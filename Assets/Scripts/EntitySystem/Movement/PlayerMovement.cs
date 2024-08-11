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

    [SerializeField] private BoxCollider footCollider; // for enemy head collider, for DeathZone check
    [SerializeField] private LayerMask deathLayer;

    private RaycastHit raycast; // for highlighting a tile

    #region State Machine Code
    // player's instance
    public PlayerStateMachine stateMachine { get; private set; }

    // grounded
    public IdleState idle { get; private set; }
    public RunState run { get; private set; }

    // airborne
    public JumpState jump { get; private set; }
    // FallState IMPLEMENT LATER
    #endregion


    public delegate void OnJump(Vector3 playerWorldPosition);
    public event OnJump JumpEvent;


    // raycast below for things

    #region Monobehaviour Functions
    private void Awake()
    {
        // state machines
        stateMachine = new PlayerStateMachine();

        ////////// animator stuff
        // split idle and running because why do two states depend on one animator variable? confusing.
        // also vague, not running does not imply they are idle

        // get rid of isgrounded because grounded isnt an animation, it's just a state
        // jumping and falling and etc r the animations we want to show

        // get rid of landing because there's no actual landing animation

        // idle --> isIdle
        // running --> isRunning
        // jump --> isJumping
        // falling --> isFalling

        idle = new IdleState(this, stateMachine, "isIdle");
        run = new RunState(this, stateMachine, "isRunning");
        jump = new JumpState(this, stateMachine, "isJumping");

    }

    protected override void Start()
    {
        // components
        playerInput = GetComponent<PlayerInputHandler>();
        character = GetComponent<CharacterController>();

        // grounded
        grounded = GroundedCheck();

        // state machine
        stateMachine.Initialize(idle);
    }

    protected override void Update()
    {
        base.Update();
        grounded = GroundedCheck();
        workingDirection = currentDirection; // update our working direction to hold last frame's info

        // apply gravity to workingdir
        ApplyGravity(); // can split this into falling and jumping states probably
        // update state machine logic
        stateMachine.currentState.LogicUpdate();
        /*
        // apply jump to workingdir
        Jump(); // called from entering AirborneState
        // apply movement to workingdir
        Move(playerInput.currentInput.planeMove);
        */
        // combine all into workingDirection
        SetWorkingDirectionVector();
        // set rotation based on currentDir
        HandleRotation();
        // set currentdir
        currentDirection = workingDirection; // update our current direction to the calculated magnitude
        // move
        character.Move(currentDirection * Time.deltaTime); // EXPECTING WORLD SPACE VECTOR
    }

    protected override void FixedUpdate()
    {
        stateMachine.currentState.PhysicsUpdate();
    }
    #endregion

    public override void Jump()
    {
        movementUpVector = Vector3.up * constants.jumpSpeed;
        JumpEvent?.Invoke(transform.position);
    }

    public override void Move(Vector2 move)
    {
        // create movement basis vectors in world space from camera's perspective
        Vector3 cameraRight = thirdPersonCamera.transform.right; // red-axis of camera (world-space)
        Vector3 cameraForward = thirdPersonCamera.transform.forward; // blue-axis of camera (world-space)
        // eliminate y components & re-normalize
        cameraRight.y = 0;
        cameraForward.y = 0;
        cameraRight = cameraRight.normalized;
        cameraForward = cameraForward.normalized;
        // translate our movement inputs into camera space
        movementRightVector = move.x * constants.moveSpeed * cameraRight;
        movementForwardVector = move.y * constants.moveSpeed * cameraForward;
    }

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

    public void ApplySpawnPosition(Vector3 spawnPosition, Vector3 spawnRotation)
    {
        character.Move(Vector3.zero);
        transform.rotation = Quaternion.LookRotation(spawnRotation, Vector3.up);
        character.Move(spawnPosition);
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
