using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ScriptableObject template for EntityMovement variables
[CreateAssetMenu(fileName = "???_Movement", menuName = "EntityMovement")]
public class EntityMovementConstants : ScriptableObject
{
    // moving constants
    [SerializeField] public float moveSpeed;

    // gravity constants
    [SerializeField] public float ascentGravity, descentGravity, groundedGravity;

    // turning constants
    [SerializeField] public float turnSpeed, turnSmoothTime;

    // jumping constants
    [SerializeField] public float jumpSpeed;
}
