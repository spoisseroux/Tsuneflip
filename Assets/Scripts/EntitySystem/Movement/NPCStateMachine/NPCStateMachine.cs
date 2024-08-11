using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCStateMachine
{ 
    public NPCState currentState { get; private set; }

    public void Initialize(NPCState state)
    {
        currentState = state;
        currentState.Enter();
    }

    public void ChangeState(NPCState state)
    {
        currentState.Exit();
        currentState = state;
        currentState.Enter();
    }
}
