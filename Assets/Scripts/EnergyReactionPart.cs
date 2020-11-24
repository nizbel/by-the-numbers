using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyReactionPart : MonoBehaviour
{
    ParticleSystemForceField reactionForceField;

    Transform otherPart;

    Rigidbody2D rigidBody = null;

    ParticleSystem particles = null;

    SpriteRenderer[] childSprites;

    void Awake() {
        // Define particle system
        particles = transform.Find("Particle System").GetComponent<ParticleSystem>();

        // Make it indestructible through out of screen bounds
        if (GetComponent<DestructibleObject>() != null) {
            GetComponent<DestructibleObject>().SetIsDestructibleNow(false);

            // Alter particle system stop action to destroy the object
            ParticleSystem.MainModule mainModule = particles.main;
            mainModule.stopAction = ParticleSystemStopAction.Callback;
            SetDestructibleOnParticleStop destroyCallback = particles.gameObject.AddComponent<SetDestructibleOnParticleStop>();
            destroyCallback.SetDestructibleScript(GetComponent<DestructibleObject>());
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ParticleSystem.ExternalForcesModule externalForces = particles.externalForces;
        externalForces.enabled = true;
        externalForces.influenceFilter = ParticleSystemGameObjectFilter.List;
        externalForces.AddInfluence(reactionForceField);

        rigidBody = GetComponent<Rigidbody2D>();

        rigidBody.drag = 5f;

        ParticleSystem.EmissionModule emission = particles.emission;
        emission.rateOverTimeMultiplier *= transform.localScale.x * 20;

        // Remove possible moving object scripts
        MovingObject[] movingObjectScripts = GetComponents<MovingObject>();
        for (int i = movingObjectScripts.Length-1; i >= 0; i--) {
            Destroy(movingObjectScripts[i]);
        }

        // Keep reference to child sprites
        childSprites = GetComponentsInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((transform.position - otherPart.position).sqrMagnitude < 0.01f) {
            if (reactionForceField != null) {
                reactionForceField.transform.Find("Particle System").gameObject.SetActive(true);
                Destroy(reactionForceField);
            }
            GetComponent<Energy>().DisappearInReaction();

            rigidBody.bodyType = RigidbodyType2D.Static;
            this.enabled = false;
        }
    }

    void FixedUpdate() {
        // Move towards the reaction center
        rigidBody.AddForce((reactionForceField.transform.position - transform.position));

        // Concentrate sprites
        foreach (SpriteRenderer sprite in childSprites) {
            sprite.transform.localScale = Vector3.Lerp(sprite.transform.localScale, sprite.transform.localScale * 0.3f, Time.deltaTime);
        }

        //// Energize
        //ParticleSystem.EmissionModule emission = particles.emission;
        //emission.rateOverTimeMultiplier += 5;
    }

    /*
     * Getters and Setters
     */
    public void SetReactionForceField(ParticleSystemForceField reactionForceField) {
        this.reactionForceField = reactionForceField;
    }

    public void SetOtherPart(Transform otherPart) {
        this.otherPart = otherPart;
    }
}
