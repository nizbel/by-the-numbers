using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrayEngineEffects : MonoBehaviour
{
    [SerializeField]
    ParticleSystem engineFire;

    [SerializeField]
    ParticleSystem shockEffects;

    [SerializeField]
    ParticleSystem.MinMaxGradient positiveColor;

    [SerializeField]
    ParticleSystem.MinMaxGradient negativeColor;

    public void StartShock() {
        shockEffects.Play();
    }

    public void StartEngineFire(bool prewarm = false) {
        ParticleSystem.MainModule main = engineFire.main;
        main.prewarm = prewarm;
        engineFire.Play();
    }

    // Sets if the engine is using positive or negative energy
    public void SetEngineValue(int value) {
        // TODO Change effects' colors
        if (value > 0) {
            ParticleSystem.ColorOverLifetimeModule colorLifetimeModule = engineFire.colorOverLifetime;
            colorLifetimeModule.color = positiveColor;
        } else if (value < 0) {
            ParticleSystem.ColorOverLifetimeModule colorLifetimeModule = engineFire.colorOverLifetime;
            colorLifetimeModule.color = negativeColor;

            // TODO Find a way to change positive and negative shocks
        } else {
            Debug.LogError("VALUE CAN'T BE ZERO");
        }
    }

    public void StopEffects() {
        shockEffects.Stop();
        engineFire.Stop();
    }
}
