using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAbilityState : NPCState
{
    // protected IAbility ability; maybe some info about enter, cast, exit coroutines in here?

    // statemachine and npc movement component
    protected NPCMovement npc;
    protected NPCStateMachine stateMachine;

    private bool waiting = false;

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
        // preability coroutine

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

    private IEnumerator Wait()
    {
        waiting = true;
        yield return new WaitForSeconds(2.0f);
        waiting = false;
    }
}
