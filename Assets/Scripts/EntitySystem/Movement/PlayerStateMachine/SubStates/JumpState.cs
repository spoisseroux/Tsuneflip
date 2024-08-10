using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : AirborneState
{
    public JumpState(PlayerMovement move, PlayerStateMachine machine, string animName) : base(move, machine, animName) { }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
        player.Jump();
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
