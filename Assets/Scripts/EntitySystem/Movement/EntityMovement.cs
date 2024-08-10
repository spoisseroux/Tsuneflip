using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// All possible commands for a given Entity
public interface IMove
{
    void Move(Vector2 move);
}

public interface IJump
{
    void Jump();
}

public abstract class EntityMovement : MonoBehaviour, IMove, IJump
{
    // abstract movement functions
    public abstract void Jump();
    public abstract void Move(Vector2 move);
    protected abstract void ApplyGravity();

    // for ground collisions
    [SerializeField] public LayerMask groundLayer;
    [SerializeField] public bool grounded;
    public abstract bool GroundedCheck();

    // rotation
    [SerializeField] public Quaternion facingDir;

    // movement constants
    [SerializeField] protected EntityMovementConstants constants;

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
