using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerParticles : MonoBehaviour
{
    public ParticleSystem smokeParticleSystem;
    public ParticleSystem landParticleSystem;
    public ParticleSystem dashParticleSystem;
    public ParticleSystem spinParticleSystem;

    public static PlayerParticles Instance;
    void Awake() { Instance = this; }
}