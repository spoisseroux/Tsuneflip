using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// could extend this to be "invincible for a few seconds after taking damage with a coroutine
public interface ITakeDamage
{
    void TakeDamage(IDealDamage damager); // pass in source of damage, MAYBE MAKE THIS GAMEOBJECT?
}
