using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : MonoBehaviour
{
    public ParticleSystem ExplosionparticleSystem;
    public MeshRenderer model;
    public float BoomRadius = 5;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position , BoomRadius);
    }

    private void Start()
    {
        StartCoroutine(Boom());
    }

    IEnumerator Boom()
    {
        yield return new WaitForSeconds(3);
        ExplosionparticleSystem.transform.parent = null;
        ExplosionparticleSystem.Play();
        Destroy(ExplosionparticleSystem.gameObject, 3);
        Destroy(gameObject);

        EZCameraShake.CameraShaker.Instance.ShakeOnce(4, 2, 0.5f, 0.3f);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, BoomRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.GetComponent<Health>() != null && hitCollider.CompareTag("Player"))
            {

                hitCollider.GetComponent<Health>().TakeDamage(10);

            }
        }
    }
}
