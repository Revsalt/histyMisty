using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class branch : MonoBehaviour
{
    public float rotspeed;
    public float speed;
    public float kncobackforce;
    public float Upkncobackforce;
    public GameObject model;
    [Header("Colider")]
    public float radius = 2;
    public Vector3 offset = Vector3.zero;

    void Update()
    {
        transform.position = transform.position + transform.right * speed * Time.deltaTime;
        model.transform.Rotate(new Vector3(rotspeed * Time.deltaTime,0,0));

        Collider[] hitColliders = Physics.OverlapSphere(offset + transform.position, radius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player") && hitCollider.GetComponent<Health>() != null)
            {
                hitCollider.GetComponent<Health>().TakeDamage(10);
                Player.instance.SetKnockBack(kncobackforce , transform.right , Upkncobackforce);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(offset + transform.position, radius);
    }
}
