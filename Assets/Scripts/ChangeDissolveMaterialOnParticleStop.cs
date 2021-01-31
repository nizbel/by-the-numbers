using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeDissolveMaterialOnParticleStop : MonoBehaviour
{
    private const float MIN_DISSOLVE_AMOUNT = 0;
    private const float MAX_DISSOLVE_AMOUNT = 0.9f;

    Material material;

    ParticleSystem particles;

    private void Start() {
        material = GetComponent<ParticleSystemRenderer>().material;
        material.SetFloat("_DissolveAmount", Random.Range(MIN_DISSOLVE_AMOUNT, MAX_DISSOLVE_AMOUNT));

        particles = GetComponent<ParticleSystem>();
    }

    public void OnParticleSystemStopped() {
        material.SetFloat("_DissolveAmount", Random.Range(MIN_DISSOLVE_AMOUNT, MAX_DISSOLVE_AMOUNT));
        particles.Play();
    }
}
