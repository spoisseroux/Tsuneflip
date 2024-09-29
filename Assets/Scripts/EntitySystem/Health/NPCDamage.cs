using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// attached on a child gameobject
[RequireComponent(typeof(Collider))]
public class NPCDamage : MonoBehaviour, IDealDamage
{
    public int hitDamage { get => hitDamage; set => hitDamage = value; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerDamage>(out PlayerDamage player))
        {
            player?.TakeDamage(this);
            Destroy(this.transform.parent.gameObject);
        }
    }
}
