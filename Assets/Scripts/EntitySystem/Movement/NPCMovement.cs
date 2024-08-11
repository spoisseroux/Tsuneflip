using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NPC class, maybe we make this more generic or less generic (not even NPCMovement, specify one for each like HandMovement BunnyMovement)?
public class NPCMovement : EntityMovement
{
    // statemachine
    public NPCStateMachine stateMachine;

    // states
    public NPCState roam { get; private set; }

    // distance to player
    [SerializeField] Transform player;
    public float distanceToPlayer;

    // grid size
    public float maxX, maxZ, minX, minZ;

    // distance to Target
    public Vector3 currentTarget;
    public Vector3 moveVector;
    public float distanceToTarget;

    // variables
    public float constantY;

    #region Monobehaviours
    private void Awake()
    {
        // constants made
        constantY = 1.0f;
        maxX = 5.0f;//LevelMenuManager.loaded.rows - 0.5f;
        maxZ = 5.0f;//LevelMenuManager.loaded.columns - 0.5f;
        minX = 0.0f;
        minZ = 0.0f;

        // state machine
        stateMachine = new NPCStateMachine();
        roam = new RoamingState(this, stateMachine, new FindRandomTile(3.0f, 10.0f));

        // get player
        player = GameObject.Find("Player").GetComponent<Transform>();
    }

    protected override void Start()
    {
        stateMachine.Initialize(roam);
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
        //moveVector = new Vector3(move.normalized.x, 0.0f, move.normalized.y);
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
