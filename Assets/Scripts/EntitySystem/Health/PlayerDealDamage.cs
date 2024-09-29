using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * REDO IDEA:
 * 
 * Create a zone like the grounded collider for doing physics overlap for each Hitbox, create layer Hitbox.
 * Player --> foot, NPC --> body
 * 
 * Then create a separate zone for Hurtboxes, with a layer as well
 * Player --> body, NPC --> head/etc.
 * 
 * Concerns:
 * 1. Do we really check every frame for overlaps??? Ugh...
 * 2. Is there any order to checking the overlaps? Hurtboxes --> Hitboxes?
 * 3. Read article ig
 * 4. Remember death zone operates off old ITakeDamage system
 */


public class PlayerDealDamage : MonoBehaviour, IDealDamage
{
    public int hitDamage { get => hitDamage; set => hitDamage = value; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<NPCTakeDamage>(out NPCTakeDamage npc))
        {
            npc?.TakeDamage(this);

            // push info to stats
        }
    }
}
