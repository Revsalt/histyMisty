using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Linq;

public class GolemBossFight : MonoBehaviour
{
    [SerializeField] NavMeshAgent navmeshagent;
    [SerializeField] Transform[] StoneSpawnPoints;
    [SerializeField] GameObject[] Stones;
    [SerializeField] Transform[] MovePoints;
    [SerializeField] float speed;
    Queue<UnityEvent> BossActions = new Queue<UnityEvent>();
    [SerializeField] UnityEvent[] Actions;
    public float distance;
    int RandomNumber;
    int TransitionTime;
    bool currentlythrowing= false;
    bool isrunning;
    bool IsAttacking = true;
    bool isisthrowing;
    // Start is called before the first frame update
    void Start()
    {
        BossActions.Enqueue(Actions[0]);
        BossActions.Enqueue(Actions[1]);
        StartCoroutine(MoveAround());
    }
    void SwitchActions()
    {
        if (isrunning == false)
        {
            if (BossActions.ToArray()[0] == Actions[0])
            {
                if (currentlythrowing == false)
                {
                    Actions[0].Invoke();
                }
                
            }
            else if (BossActions.ToArray()[1] == Actions[1])
            {
                Actions[1].Invoke();
            }
            if (currentlythrowing == false)
            {
                BossActions.Dequeue();
                BossActions.Enqueue(Actions[0]);
                Debug.Log("action1 dequed");
            }
            if (IsAttacking == false)
            {
                BossActions.Dequeue();
                BossActions.Enqueue(Actions[0]);
                Debug.Log("action2 dequed");
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(transform.position, MovePoints[RandomNumber].position);
        if (distance <=3)
        {
            isrunning = false;
        }
        else
        {
            isrunning = true;
        }
        SwitchActions();
    }
    public void SummonRocks()
    {
        Debug.Log("here");
        currentlythrowing = true;
        if (isisthrowing)
        {
            isisthrowing = false;
            for (int i = 0; i < StoneSpawnPoints.Length; i++)
            {
                Instantiate(Stones[i], StoneSpawnPoints[i].position, Quaternion.identity);
            }
        }
        currentlythrowing = false;
    }
    IEnumerator MoveAround()
    {
        RandomNumber = UnityEngine.Random.Range(0, MovePoints.Length);
        navmeshagent.SetDestination(MovePoints[RandomNumber].position);
        isisthrowing = true;
        yield return new WaitForSeconds(10f);
        StartCoroutine(MoveAround());
    }
    public void nunusaysfuckoff()
    {
        Debug.Log("iamsupergay");
    }
    Vector3 getBQCPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        // "t" is always between 0 and 1, so "u" is other side of t
        // If "t" is 1, then "u" is 0
        float u = 1 - t;
        // "t" square
        float tt = t * t;
        // "u" square
        float uu = u * u;
        // this is the formula in one line
        // (u^2 * p0) + (2 * u * t * p1) + (t^2 * p2)
        Vector3 p = (uu * p0) + (2 * u * t * p1) + (tt * p2);
        return p;
    }
}
