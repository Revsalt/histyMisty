using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouthConfiguration : MonoBehaviour
{
    public static MouthConfiguration instance;

    public GameObject[] mouthSet;

    public void Start()
    {
        instance = this;
    }

    public void SetMouth(string mouthName)
    {
        foreach (GameObject mouth in mouthSet)
        {
            if (mouth.name == mouthName)
            {
                mouth.SetActive(true);
            }
            else
            {
                mouth.SetActive(false);
            }
        }
    }
}
