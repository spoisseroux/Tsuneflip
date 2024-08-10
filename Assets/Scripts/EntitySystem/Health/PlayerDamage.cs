using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour, ITakeDamage, IDealDamage /* is the second interface necessary? could just do two hitboxes */
{
    // total lives
    private int lives;

    // event for health change, communicates to the (for example) UI, Level Manager
    public delegate void LivesChangeHandler(IDealDamage source, int newHealth);
    public event LivesChangeHandler OnLivesNumberChange;

    // how do we set up the colliders here???
    // parent object (whole body hitbox): has movement, itakedamage component + boxcollider (for taking damage when colliding with enemy)
    // child object (feet): idealdamage component + boxcollider (for when jumping)
    // child object (whole body hurtbox): idealdamage component + boxcollider (for dealing damage when colliding with enemy) THIS COMPONENT


    // enemies have a hurtbox on their head?
    // hit box is rest of it?

    private void Awake()
    {
        SetLives(3);
    }


    // interface implementations
    public int hitDamage { get => hitDamage; set => hitDamage = value; }

    public void TakeDamage(IDealDamage damager)
    {
        // don't hurt urself pls
        if ((IDealDamage)this == damager)
        {
            return;
        }
        // take away a life
        Debug.Log("I " + this.name + " was hit by " + damager);
        lives--;
        OnLivesNumberChange?.Invoke(damager, lives); // game manager, UI, etc. reacts to info
    }

    public void SetLives(int value)
    {
        lives = value;
        OnLivesNumberChange?.Invoke(this, lives);
    }

    public void GainALife()
    {
        // add a life
        lives++;
    }
}
