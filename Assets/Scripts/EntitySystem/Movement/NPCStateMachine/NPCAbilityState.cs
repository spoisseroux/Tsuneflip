using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAbilityState : NPCState
{
    protected NPCMovement npc;
    protected NPCStateMachine stateMachine;

    public NPCAbilityState(NPCMovement entityIn, NPCStateMachine npcSM) : base(entityIn, npcSM)
    {

    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
