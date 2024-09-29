using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour, ITakeDamage
{
    // total lives
    [SerializeField] private int lives;

    // event for health change, communicates to the (for example) UI, Level Manager
    public delegate void LivesChangeHandler(IDealDamage source, int newHealth);
    public event LivesChangeHandler OnLivesNumberChange;

    private void Awake()
    {
        SetLives(1); // can be changed later for more lives
    }

    // interface implementations
    public void TakeDamage(IDealDamage damager /*= null*/)
    {
        // take away a life
        Debug.Log(this.name + " was hit by " + damager.ToString());
        lives--;
        OnLivesNumberChange?.Invoke(damager, lives); // game manager, UI, etc. reacts to info
    }

    public void SetLives(int value)
    {
        lives = value;
        OnLivesNumberChange?.Invoke(null, lives);
    }

    public void GainALife()
    {
        // add a life
        lives++;
    }

    public int GetLives()
    {
        return lives;
    }
}
