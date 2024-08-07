using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// All possible commands for a given Entity
public interface IMove
{
    void Move(Vector3 move);
}

public interface IJump
{
    void Jump();
}

public abstract class EntityMovement : MonoBehaviour, IMove, IJump
{
    // abstract movement functions
    public abstract void Jump();
    public abstract void Move(Vector3 move);
    protected abstract void ApplyGravity();

    // for ground collisions
    public LayerMask groundLayer;

    // movement constants
    protected EntityMovementConstants constants;

    // entity state machine
    // protected EntityStateMachine stateMachine;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    // FixedUpdate for physics calls
    protected virtual void FixedUpdate()
    {

    }
}
