using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flower : MonoBehaviour
{
    public Animator anim;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            anim.SetBool("landed", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
            anim.SetBool("landed", false);
    }
}
