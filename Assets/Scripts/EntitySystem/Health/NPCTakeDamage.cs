using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// attached as a child game object
public class NPCTakeDamage : MonoBehaviour, ITakeDamage
{
    public void TakeDamage(IDealDamage damager)
    {
        // take away a life
        Debug.Log(this.name + " was hit by " + damager.ToString());
        // destroy NPC for now
        Destroy(this.transform.parent.gameObject);

        // event to trigger stats keeping, etc.
        // if damager == player.... yeah
    }
}
