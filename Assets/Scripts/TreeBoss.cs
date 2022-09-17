using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBoss : MonoBehaviour
{
    public GameObject Tooth;
    public GameObject Apple;
    public GameObject Branch;
    public GameObject TreeFace;
    public Vector3 cubesize;
    public Vector3 cubeOffset;
    public Transform[] SpawnPosistions;
    public Transform spawnPositions;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + cubeOffset, cubesize);

        Gizmos.DrawSphere(transform.position + new Vector3(
            -(cubesize.x / 2)
            , cubeOffset.y,
            -(cubesize.z / 2)
            ) , 1);

        Gizmos.DrawSphere(transform.position + new Vector3(
            (cubesize.x / 2)
            , cubeOffset.y,
            (cubesize.z / 2)
            ), 1);
    }

    GameObject g;
    private void Start()
    {
        GetComponentInChildren<Animator>().speed = 0;
        g = new GameObject("lookat");
        g.transform.position = transform.position;
    }

    bool isTired = false;
    bool canlookatplayer = false;
    bool fightstarted = false;
    bool fight2started = false;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            GetComponent<Health>().currentHealth = 250;

        if (canlookatplayer)
        {
            g.transform.LookAt(Player.instance.transform.position);
            TreeFace.transform.rotation = Quaternion.Lerp(TreeFace.transform.rotation, Quaternion.Euler(0, g.transform.eulerAngles.y - 90, 0), 5 * Time.deltaTime);
        }

        if (Vector3.Distance(transform.position, Player.instance.transform.position) <= 40 && !fightstarted)
        {
            StartFight();
            StartCoroutine(tireddelay());
            fightstarted = true;
        }

        if (!isTired && fightstarted && !fight2started && GetComponent<Health>().currentHealth <= 500 / 2)
        {
            StopAllCoroutines();
            StartFight2();
            StartCoroutine(aftertired());
            fight2started = true;
        }

        spawnPositions.transform.Rotate(0, 5 * Time.deltaTime, 0);
    }

    public void TakeDamageEffect()
    {
        StartCoroutine(DamageDelay());

        IEnumerator DamageDelay()
        {
            GetComponentInChildren<Animator>().SetBool("isDamaged", true);
            yield return new WaitForSeconds(0.1f);
            GetComponentInChildren<Animator>().SetBool("isDamaged", false);
        }
    }
    
    IEnumerator tireddelay()
    {
        yield return new WaitForSeconds(25);
        Debug.Log("tired");

        isTired = true;
        GetComponentInChildren<Animator>().SetBool("isTired", true);
        GetComponent<CapsuleCollider>().enabled = true;
        StopAllCoroutines();
        StartCoroutine(aftertired());

    }
    IEnumerator aftertired()
    {
        yield return new WaitForSeconds(4);

        GetComponent<CapsuleCollider>().enabled = false;
        GetComponentInChildren<Animator>().SetBool("isTired", false);
        if (fight2started)
        {
            StartFight2();
        }
        else
        {
            StartFight();
        }

        StartCoroutine(tireddelay());
        isTired = false;
    }


    public void StartFight()
    {
        StartCoroutine(SpawanApple(1));
        StartCoroutine(InvokedSpawanBranches());

        GetComponentInChildren<Animator>().speed = 1;
        canlookatplayer = true;

        IEnumerator InvokedSpawanBranches()
        {
            yield return new WaitForSeconds(0);
            StartCoroutine(SpawanBranches(2 , 2));
        }
    }

    public void StartFight2()
    {
        GetComponentInChildren<Animator>().SetBool("isAngry", true);

        StartCoroutine(SpawanApple(1));
        StartCoroutine(SpawanTreeTooth(3));
        StartCoroutine(InvokedSpawanBranches());

        GetComponentInChildren<Animator>().speed = 1;
        canlookatplayer = true;

        IEnumerator InvokedSpawanBranches()
        {
            yield return new WaitForSeconds(0);
            StartCoroutine(SpawanBranches(1 , 1));
        }
    }

    IEnumerator SpawanApple(float rate)
    {
        
        Vector3 GetRandPos = new Vector3(
            Random.Range(-(cubesize.x / 2), cubesize.x / 2)
            , cubeOffset.y,
            Random.Range(-(cubesize.z / 2), cubesize.z / 2)
            );

        Instantiate(Apple, transform.position + GetRandPos, Apple.transform.rotation, null);

        RaycastHit hit;
        Physics.Raycast(Player.instance.transform.position, Player.instance.transform.up, out hit);

        if (hit.collider != null &&
            hit.collider.CompareTag("TreeBoss"))
        {
            Vector3 GetPos = hit.point;
            Instantiate(Apple, GetPos, Apple.transform.rotation, null);
        }

        yield return new WaitForSeconds(rate);
        StartCoroutine(SpawanApple(rate));
    }

    IEnumerator SpawanTreeTooth(float rate)
    {
        RaycastHit hit;
        Physics.Raycast(Player.instance.transform.position, -Player.instance.transform.up, out hit);

        GameObject g = Instantiate(Tooth, hit.point, Quaternion.identity, null);

        Destroy(g, 5);
        yield return new WaitForSeconds(rate);
        StartCoroutine(SpawanTreeTooth(rate));
    }

    int pattern = 1;
    IEnumerator SpawanBranches(int amount ,float rate)
    {
        yield return new WaitForSeconds(rate);

        for (int i = 0; i < SpawnPosistions.Length; i+= amount)
        {
            GameObject b = Instantiate(Branch, SpawnPosistions[i].transform.position, SpawnPosistions[i].transform.rotation);
            Destroy(b, 9);
        }

        if (pattern == 1) { pattern = 2; } else { pattern = 1; }
        StartCoroutine(SpawanBranches(pattern , rate));
    }

   public void Destroythis()
    {
        Destroy(this);
    }
}
