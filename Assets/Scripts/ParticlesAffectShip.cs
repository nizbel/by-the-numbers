using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesAffectShip : MonoBehaviour
{
    private int value = 0;

    public int Value { get => value; set => this.value = value; }

    // Start is called before the first frame update
    void Start()
    {
        // TODO Add play sound for energy latching
    }

    public void OnParticleSystemStopped() {
        PlayerController.controller.UpdateShipValue(value);
        Destroy(gameObject);
    }
}
