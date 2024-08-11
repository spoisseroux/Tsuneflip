using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
    protected PlayerMovement player;
    protected PlayerStateMachine playerStateMachine;

    protected Vector2 directionalInput;

    protected float startTime;
    private string animationName;

    public PlayerState(PlayerMovement move, PlayerStateMachine machine, string animName)
    {
        this.player = move;
        this.playerStateMachine = machine;
        this.animationName = animName;
    }

    public virtual void Enter()
    {
        DoChecks();
        startTime = Time.time;
        player.GetComponent<Animator>().SetBool(animationName, true); // change when we animate player
        //Debug.Log("entering state: " + playerStateMachine.currentState.ToString());
    }

    public virtual void Exit()
    {
        //Debug.Log("exiting state " + playerStateMachine.currentState.ToString());
        player.GetComponent<Animator>().SetBool(animationName, false); // change when we animate player
    }

    public virtual void LogicUpdate()
    {
        // get planar movement changes
        directionalInput = player.playerInput.currentInput.planeMove;

        // set planar move direction for Player
        // placed this call here because the Player can move directionally no matter what state they are in
        player.Move(directionalInput);
    }

    public virtual void PhysicsUpdate()
    {
        DoChecks();
    }

    public virtual void DoChecks()
    {

    }
}
