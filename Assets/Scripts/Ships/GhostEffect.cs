using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostEffect : MonoBehaviour
{
    // TODO Decide if this info should be in here
    private const int DEFAULT_PIXELS_PER_UNIT = 100;

    [SerializeField]
    ParticleSystem ghostParticle;

    void Awake() {
        transform.rotation = PlayerController.controller.transform.rotation;
        ParticleSystem.MainModule main = ghostParticle.main;
        main.startRotation = -transform.rotation.z*2;

        // Define the size
        main.startSize = new ParticleSystem.MinMaxCurve(PlayerController.controller.GetSpaceshipSprite().sprite.rect.width / DEFAULT_PIXELS_PER_UNIT);

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
