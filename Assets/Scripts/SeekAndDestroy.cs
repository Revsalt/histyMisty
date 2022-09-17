using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekAndDestroy : MonoBehaviour
{
    
    [SerializeField] Transform PointToGo;
    [SerializeField] float TimeToWait = 3f;
    [SerializeField] float time;
    [SerializeField] float speed = 50f;
    bool isSeeking = true;
    bool CanSeek;
    [Header("Stone HardCoded Values")]
    [SerializeField] float distanceMoved = 3;
    [SerializeField] float stoneSpeed = 0.5f;

    Vector3 CurrentPos = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        CurrentPos = transform.position;
    }

    void Update()
    {
        if (transform.position.y < CurrentPos.y + distanceMoved && CanSeek)
        {
            Debug.Log("haha");
            transform.position += new Vector3(0,stoneSpeed * Time.deltaTime,0);
            CanSeek = false;
        }
        else
        {
            CanSeek = true;
        }

        if (!CanSeek)
            return;

        transform.position += new Vector3(0, 0.1f * Time.deltaTime, 0);

        if (time <= TimeToWait)
        {
            time += Time.deltaTime;
        }
        else
        {
            Debug.Log("Target Set");
            if (isSeeking == true)
            {
                PointToGo.position = Player.instance.transform.position;
                isSeeking = false;
            }
            else
            {
                this.transform.position = Vector3.MoveTowards(this.transform.position, PointToGo.position, speed * Time.deltaTime);
            }
            Destroy(gameObject, 6f);
        }
    }
}
