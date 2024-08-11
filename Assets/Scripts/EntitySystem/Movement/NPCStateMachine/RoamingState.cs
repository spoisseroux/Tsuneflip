using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoamingState : NPCState
{
    protected IRoaming roamStrategy;
    // protected IUpdate updateStrategy;
    protected Vector2 nextPos;

    // update time
    private float updateTime;
    private float timePassed;
    private float bufferedDistanceToPlayer;
    private float distanceToTarget;


    public RoamingState(NPCMovement entityIn, NPCStateMachine npcSM, IRoaming strategy) : base(entityIn, npcSM)
    {
        roamStrategy = strategy;
        updateTime = 0.5f;
    }

    public override void DoChecks()
    {
        base.DoChecks();
        // check if player near (alert?) FOR NOW WE SKIP THIS
        bufferedDistanceToPlayer = npc.distanceToPlayer;
        // check if near destination (ability?)
    }

    public override void Enter()
    {
        base.Enter();
        nextPos = roamStrategy.FindNextPosition(npc.transform.position);
        timePassed = 0.0f;
    }

    public override void Exit()
    {
        base.Exit();
        nextPos = npc.transform.position; // set as current position
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate(); // how do we ACTUALLY move the npc objects?
        timePassed += Time.deltaTime;
        if (timePassed >= updateTime)
        {
            timePassed = 0;
            bufferedDistanceToPlayer = npc.distanceToPlayer;
        }
        // if at target position or alerted, change... maybe depends on unit
        if (bufferedDistanceToPlayer <= 0.5f)
        {
            // chase
            // flee
            // etc
        }
        
        else if (distanceToTarget <= 0.01f)
        {
            // stop

            // flip tile (change to ability)
            // pick new target spot
            nextPos = roamStrategy.FindNextPosition(npc.transform.position);

            // maybe in tileflip ability we send an event out to the grid and request that the tile at our world position flips?
        }
        // else move towards position
        else
        {
            npc.Move(new Vector2(nextPos.x, nextPos.y));
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        npc.transform.LookAt(npc.currentTarget);
        npc.transform.Translate(npc.moveVector * npc.GetConstants().moveSpeed * Time.deltaTime);
    }
}
