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
    }

    public override void Enter()
    {
        base.Enter();
        nextPos = ResolveToTile(ResolveToGrid(roamStrategy.FindNextPosition(npc.transform)));
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
        
        else if (npc.distanceToTarget <= 0.05f)
        {
            // stop

            // flip tile (change to ability) WOULD GO HERE

            // pick new target spot
            nextPos = ResolveToTile(ResolveToGrid(roamStrategy.FindNextPosition(npc.transform)));
            // for now, we just move to new target
            npc.Move(new Vector2(nextPos.x, nextPos.y));

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
        npc.transform.position = Vector3.MoveTowards(npc.transform.position, npc.currentTarget, npc.GetConstants().moveSpeed * Time.deltaTime);
    }

    private Vector2 ResolveToGrid(Vector2 potentialMove)
    {
        potentialMove.x = Mathf.Clamp(potentialMove.x, npc.minX, npc.maxX);
        potentialMove.y = Mathf.Clamp(potentialMove.y, npc.minZ, npc.maxZ);
        return potentialMove;
    }

    private Vector2 ResolveToTile(Vector2 gridLocation)
    {
        return new Vector2(NearestHalf(gridLocation.x), NearestHalf(gridLocation.y));
    }

    private float NearestHalf(float num)
    {
        return Mathf.Round(num * 2) / 2;
    }
}
