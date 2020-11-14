using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostEffect : MonoBehaviour
{
    [SerializeField]
    ParticleSystem ghostParticle;

    void Awake() {
        transform.rotation = PlayerController.controller.transform.rotation;
        ParticleSystem.MainModule main = ghostParticle.main;
        main.startRotation = -transform.rotation.z*2;
        ghostParticle.Play();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
