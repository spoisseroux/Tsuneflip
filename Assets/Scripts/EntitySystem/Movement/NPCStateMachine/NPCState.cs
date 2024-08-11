using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCState
{
    protected NPCMovement npc;
    protected NPCStateMachine npcStateMachine;

    protected float startTime;
    protected float animationName;

    public NPCState(NPCMovement entityIn, NPCStateMachine npcSM)
    {
        npc = entityIn;
        npcStateMachine = npcSM;
    }

    public virtual void Enter()
    {
        DoChecks();
        startTime = Time.time;
        // npc.GetComponent<Animator>().SetBool(animationName, true);
        Debug.Log("NPC entering state: " + this.ToString());
    }

    public virtual void Exit()
    {
        // npc.GetComponent<Animator>().SetBool(animationName, false);
        Debug.Log("NPC exited state: " + this.ToString());
    }

    // idk if there needs to be any base logic here
    public virtual void LogicUpdate() { }
    public virtual void PhysicsUpdate() { DoChecks(); }
    public virtual void DoChecks() { }
}
