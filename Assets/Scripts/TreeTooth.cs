using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeTooth : MonoBehaviour
{
    public ParticleSystem smoke;
    public float kncobackforce;
    public float Upkncobackforce;
    [Header("Colider")]
    public float radius = 2;
    public Vector3 offset = Vector3.zero;

    private void Update()
    { 
        Collider[] hitColliders = Physics.OverlapSphere(offset + transform.position, radius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player") && hitCollider.GetComponent<Health>() != null)
            {
                hitCollider.GetComponent<Health>().TakeDamage(10);
                Player.instance.SetKnockBack(kncobackforce , Vector3.up , 0);
                this.enabled = false;
            }
        }
    }

    public void SpawnSmoke()
    {
            smoke.Play();
    }
}
