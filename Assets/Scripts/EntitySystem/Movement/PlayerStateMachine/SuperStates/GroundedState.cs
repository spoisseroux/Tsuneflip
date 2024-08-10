using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedState : PlayerState
{
    protected bool jumpInput;

    public GroundedState(PlayerMovement move, PlayerStateMachine machine, string animName) : base(move, machine, animName) { }

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

        // check for jump input
        jumpInput = player.playerInput.currentInput.jumpMove;

        if (jumpInput)
        {
            // change state
            playerStateMachine.ChangeState(player.jump);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }
}
