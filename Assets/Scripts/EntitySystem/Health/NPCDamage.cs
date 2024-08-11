using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class NPCDamage : MonoBehaviour, IDealDamage
{
    public int hitDamage { get => hitDamage; set => hitDamage = value; }

    private void OnTriggerEnter(Collider other)
    {
        ITakeDamage damageableObject = other.GetComponent<ITakeDamage>();
        damageableObject?.TakeDamage(this);

        Destroy(this.gameObject); // hmm..... maybe some issues here
    }
}
