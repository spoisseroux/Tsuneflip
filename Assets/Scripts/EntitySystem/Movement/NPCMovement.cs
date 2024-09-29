using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum State
{
    Idle,
    Roam,
    Ability,
    Alert
}

// this is just the movement class for the hand NPC for now. can abstract and refactor later for multiple use (PassiveNPC, AggressiveNPC)
public class NPCMovement : EntityMovement
{
    // navmesh
    public NavMeshAgent agent;

    // animator
    protected float animationName;
    private Animator animator;

    // states & implementations (each of these have their own update time to check player distance???)
    [SerializeField] protected State mode;
    private float timePassed;
    // roam
    private IRoaming roam;
    private float roamTargetUpdate = 0.1f; // depends on roam strategy
    private float roamPlayerUpdate = 0.5f;

    // ability
    private IAbility ability;
    private float castTime = 0.5f; // in practice, this will depend on the ability!
    public bool casting;

    // alert
    //private IAlert alert;
    //private float alertUpdate = 0.25f; // depends on alert strategy

    // idle
    //private IIdle idle;
    private float idleTime = 1.0f; // depends on NPC
    public bool waiting;

    // distance to player
    [SerializeField] Transform player;
    public float distanceToPlayer;

    // grid size
    public float maxX, maxZ, minX, minZ;

    // distance to Target
    public Vector3 currentTarget;
    private Vector3 startingTarget = new Vector3(-5.0f, 1.0f, -5.0f);
    public Vector3 moveVector;
    public float distanceToTarget;

    // variables
    public float constantY;

    #region Monobehaviours
    private void Awake()
    {
        // setup stuff
        currentTarget = startingTarget;

        // constants
        constantY = 1.0f;
        maxX = 8.5f;//LevelMenuManager.loaded.rows - 0.5f;
        maxZ = 8.5f;//LevelMenuManager.loaded.columns - 0.5f;
        minX = 0.5f;
        minZ = 0.5f;
        agent.speed = constants.moveSpeed;
        agent.stoppingDistance = 0.01f; // hmm
        // agent.angularSpeed = ???;

        // states and movement strategies
        roam = new FindRandomTile(2.0f, 10.0f, minX, maxX, minZ, maxZ);
        ability = new FlipTile(1.0f, "FlipTile", this);
        waiting = false;
        casting = false;

        // get player
        player = GameObject.Find("Player").GetComponent<Transform>();
    }

    protected override void Start()
    {
        mode = State.Idle;
        EnterIdle();
    }

    // logic updates
    protected override void Update()
    {
        // update constants
        base.Update();
        distanceToPlayer = UpdatePlayerDistance();
        distanceToTarget = UpdateTargetDistance();
        timePassed += Time.deltaTime;

        // big switch statement
        switch (mode)
        {
            case State.Idle:
                UpdateIdle();
                break;

            case State.Roam:
                UpdateRoam();
                break;

            case State.Ability:
                UpdateAbility();
                break;

            case State.Alert:
                // UpdateAlert();
                break;
        }
    }

    // physics updates
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
    #endregion

    #region Idle State
    private void EnterIdle()
    {
        //Debug.Log("NPC entering Idle");
        timePassed = 0.0f;
        mode = State.Idle;
        // begin waiting coroutine
        StartCoroutine(Wait(idleTime));
        // begin coroutine to change startingTarget to a new target upong Ready! Go! completing!
        // this could also just be changed to disabling movement for all EntityMovement units, like we do for Player at start, end, yada yada
    }

    private void UpdateIdle()
    {
        // change from starting Target
        if (startingTarget == currentTarget)
        {
            mode = State.Roam;
            EnterRoam();
        }

        if (!waiting && currentTarget != startingTarget)
        {
            // go to ability (TAKES PRECEDENCE) needs a distant target on initialize IDLE <--> ABILITY loops
            distanceToTarget = UpdateTargetDistance(); 
            
            if (distanceToTarget <= 0.05f)
            {
                mode = State.Ability;
                EnterAbility();
            }
            

            // go to alert (SECOND MOST)

            // go to roam (THIRD MOST)
            else
            {
                mode = State.Roam;
                EnterRoam();
            }
        }
    }
    #endregion

    #region Roam State
    private void EnterRoam()
    {
        //Debug.Log("NPC entering Roam");
        // set up mode and time
        timePassed = 0.0f;
        mode = State.Roam;

        // generate next destination
        Vector2 pos = roam.FindNextPosition(this.transform);
        currentTarget = new Vector3(pos.x, constantY, pos.y);
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(currentTarget, path);
        // if we cannot fully reach the target, reset it to a random corner point on partial path
        // MAYBE WE MAKE AN IRoamFromPartial strategy, like choose random along path, choose last along path, etc.
        if (path.status != NavMeshPathStatus.PathComplete)
        {
            int randomCorner = Random.Range(0, path.corners.Length-1);
            currentTarget = path.corners[randomCorner];
            pos = new Vector2(currentTarget.x, currentTarget.z);
        }

        //Debug.Log(currentTarget);
        Move(pos);

        // update distances
        distanceToTarget = UpdateTargetDistance();
        distanceToPlayer = UpdatePlayerDistance();
    }

    private void UpdateRoam()
    {
        // check if near target, then check to go to idle (TAKES PRECEDENCE)
        if (timePassed >= roamTargetUpdate)
        {
            distanceToTarget = UpdateTargetDistance();
            if (distanceToTarget <= 0.05f)
            {
                mode = State.Idle;
                EnterIdle();
            }
        }
        // go to alert (SECOND)
        if (timePassed >= roamPlayerUpdate)
        {
            /*
            distanceToPlayer = UpdatePlayerDistance();
            if (distanceToPlayer <= 3.0f)
            {
                mode = State.Alert;
                EnterAlert();
            }
            */
        }
    }
    #endregion

    #region Alert State
    private void EnterAlert()
    {
        Debug.Log("NPC entering Alert");
        timePassed = 0.0f;
        mode = State.Alert;

        //currentTarget = alert.FindNextPosition(this.transform);

        // update distances
        //distanceToTarget = UpdateTargetDistance();
        //distanceToPlayer = UpdatePlayerDistance();
    }

    private void UpdateAlert()
    {
        // go to idle (only)
    }
    #endregion

    #region Ability State
    private void EnterAbility()
    {
        //Debug.Log("NPC entering Ability");
        timePassed = 0.0f;
        mode = State.Ability;
        ability.CastAbility(this.transform);
        StartCoroutine(Cast(ability.CastTime));
    }

    private void UpdateAbility()
    {
        if (!casting)
        {
            // update distance from Player --> enter alert or roam
            distanceToPlayer = UpdatePlayerDistance();

            // debug for now, so just roam
            mode = State.Roam;
            EnterRoam();
        }
    }
    #endregion

    #region EntityMovement Interface Implementations
    // not needed for now
    public override void Jump()
    {
        //Debug.Log("NPC will jump");
    }

    public override void Move(Vector2 move)
    {
        // set currentTarget and moveVector
        currentTarget = new Vector3(move.x, constantY, move.y);
        agent.SetDestination(currentTarget);
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

    #region Logic Updates
    private float UpdatePlayerDistance()
    {
        return Vector3.Distance(player.position, transform.position);
    }

    private float UpdateTargetDistance()
    {
        return Vector3.Distance(currentTarget, transform.position);
    }
    #endregion

    #region Gizmos

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        if (currentTarget != null)
        {
            Gizmos.DrawSphere(currentTarget, 0.1f);
        }
    }

    #endregion

    public EntityMovementConstants GetConstants()
    {
        return constants;
    }

    private IEnumerator Wait(float waitTime)
    {
        waiting = true;
        yield return new WaitForSeconds(waitTime);
        waiting = false;
    }

    private IEnumerator Cast(float waitTime)
    {
        casting = true;
        yield return new WaitForSeconds(waitTime);
        casting = false;
    }
}
