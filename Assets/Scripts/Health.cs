using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public bool hasHealthBar = true;
    public Slider HealthBar;
    public int currentHealth = 100;

    [Header("DeathVisuals")]
    public float deathdelay = 1;
    public bool withparticles;
    public Animator anim;
    public ParticleSystem deathParticleSystem;
    public UnityEvent onTakedamage;
    public UnityEvent onDeath;

    bool isPlayer;
    bool canTakeDamage = true;

    private void Start()
    {
        isPlayer = GetComponent<Player>();
    }

    public void TakeDamage(int _damage)
    {
        if (!canTakeDamage)
            return;

        onTakedamage.Invoke();
        currentHealth -= _damage;

        if (currentHealth <= 0)
        {
            Death();
        }

        if (hasHealthBar)
            HealthBar.value = currentHealth;

        if (isPlayer)
        {
            canTakeDamage = false;
            StartCoroutine(InvicibleDelay());
        }

        IEnumerator InvicibleDelay()
        {
            GameObject playerModel = Player.instance.GetComponentInChildren<Animator>().gameObject;
            for (int i = 0; i < 3; i++)
            {
                playerModel.SetActive(false);
                yield return new WaitForSeconds(0.2f);
                playerModel.SetActive(true);
                yield return new WaitForSeconds(0.2f);
            }

            canTakeDamage = true;
            yield return null;
        }
    }

    public void Death()
    {
        onDeath.Invoke();

        if (isPlayer)
        {
            StartCoroutine(waitForReload());
        }else
        {
            Destroy(gameObject , deathdelay);
            if (anim != null)
                anim.SetBool("death", true);
            if (deathParticleSystem != null)
                deathParticleSystem.Play();
        }

        IEnumerator waitForReload()
        {
            if (anim != null)
                anim.SetBool("death", true);
            if (deathParticleSystem != null)
                deathParticleSystem.Play();

            yield return new WaitForSeconds(deathdelay);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Death"))
        {
            Death();
        }
    }
}
