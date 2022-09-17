using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickAbles : MonoBehaviour
{

    float itemRotationSpeed = 50f;
    float itemBobSpeed = 2f;
    public Vector3 basePosition;

    private void Start()
    {
        basePosition = transform.position;
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, itemRotationSpeed * Time.deltaTime, Space.World);
        transform.position = basePosition + new Vector3(0f, 0.25f * Mathf.Sin(Time.time * itemBobSpeed), 0f);
    }
}
