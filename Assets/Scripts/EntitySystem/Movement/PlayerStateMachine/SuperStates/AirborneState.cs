using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirborneState : PlayerState
{
    protected bool grounded;

    public AirborneState(PlayerMovement move, PlayerStateMachine machine, string animName) : base(move, machine, animName) { }

    public override void DoChecks()
    {
        base.DoChecks();
        // check whether Player is grounded
        grounded = player.GroundedCheck();
    }

    public override void Enter()
    {
        base.Enter();
        // send jump signal to player
        player.Jump(); // maybe we delegate this out to individual states, i.e. jump from ground, leap off enemy head, etc.
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // if grounded
        if (grounded)
        {
            // directional input present, Player moving
            if (directionalInput.magnitude <= 0.01f)
            {
                playerStateMachine.ChangeState(player.idle);
            }
            // idle
            else
            {
                playerStateMachine.ChangeState(player.run);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
