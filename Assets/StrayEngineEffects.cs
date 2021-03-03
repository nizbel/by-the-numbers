using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrayEngineEffects : MonoBehaviour
{
    [SerializeField]
    ParticleSystem engineFire;

    [SerializeField]
    ParticleSystem shockEffects;

    public void StartShock() {
        shockEffects.Play();
    }

    public void StartEngineFire(bool prewarm = false) {
        ParticleSystem.MainModule main = engineFire.main;
        main.prewarm = prewarm;
        engineFire.Play();
    }

    // Sets if the engine is using positive or negative energy
    public void SetEngineValue() {
        // TODO Change effects' colors
    }
}
