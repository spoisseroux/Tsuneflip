using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunState : GroundedState
{
    public RunState(PlayerMovement move, PlayerStateMachine machine, string animName) : base(move, machine, animName) { }

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

        // check if player no longer is moving
        if (directionalInput.magnitude < 0.01f)
        {
            // change to Idle
            playerStateMachine.ChangeState(player.idle);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
