using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBurst : MonoBehaviour 
{
    GameObject energyBurstPrefab;

    bool positiveEnergy = true;

    float burstAngle = 0;

    void Start() {
        ParticleSystem particles = GameObject.Instantiate(energyBurstPrefab, transform).GetComponent<ParticleSystem>();

        ParticleSystem.ShapeModule shape = particles.shape;
        shape.rotation += Vector3.forward * burstAngle;

        Destroy(this);
    }

    public void SetPositiveEnergy(bool positiveEnergy) {
        this.positiveEnergy = positiveEnergy;
    }

    public void SetBurstAngle(float burstAngle) {
        this.burstAngle = burstAngle;
    }

    public void SetEnergyBurstPrefab(GameObject energyBurstPrefab) {
        this.energyBurstPrefab = energyBurstPrefab;
    }
}
