using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NPC class, maybe we make this more generic or less generic (not even NPCMovement, specify one for each like HandMovement BunnyMovement)?
public class NPCMovement : EntityMovement
{
    // statemachine
    NPCStateMachine stateMachine;

    // distance to player
    [SerializeField] Transform player;
    public float distanceToPlayer;

    // distance to Target
    public Vector3 currentTarget;
    public Vector3 moveVector;
    public float distanceToTarget;

    // variables
    public float constantY = 0.5f;

    #region EntityMovement Interface Implementations
    // not needed for now
    public override void Jump()
    {
        Debug.Log("NPC will jump");
    }

    public override void Move(Vector2 move)
    {
        // set currentTarget and moveVector
        currentTarget = new Vector3(move.x, constantY, move.y);
        Debug.Log("NPC will move in this " + currentTarget + " direction in X,Z respectively");
        moveVector = new Vector3(move.normalized.x, constantY, move.normalized.y);
    }

    // probly not needed, can just lock all NPCs at a given y
    protected override void ApplyGravity()
    {
        // apply gravity
    }

    // probly not needed, but id say this one is more likely
    public override bool GroundedCheck()
    {
        throw new System.NotImplementedException();
    }
    #endregion

    #region Monobehaviours
    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Transform>();
    }

    protected override void Update()
    {
        base.Update();
        distanceToPlayer = UpdatePlayerDistance();
        distanceToTarget = UpdateTargetDistance();
        stateMachine.currentState.LogicUpdate();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        stateMachine.currentState.PhysicsUpdate();
    }
    #endregion

    public EntityMovementConstants GetConstants()
    {
        return constants;
    }

    private float UpdatePlayerDistance()
    {
        return Vector3.Distance(player.position, transform.position);
    }

    private float UpdateTargetDistance()
    {
        return Vector3.Distance(currentTarget, transform.position);
    }
}
