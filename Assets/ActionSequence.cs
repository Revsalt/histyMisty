using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ActionSequence : MonoBehaviour
{
    [Serializable]
    public class Stages
    {
        public float delay;
        public UnityEvent[]Actions;
    }
    public Stages[] stages = new Stages[] { };
    private void Update()
    {
        
    }
}