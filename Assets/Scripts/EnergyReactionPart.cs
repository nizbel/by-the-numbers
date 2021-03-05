using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyReactionPart : MonoBehaviour
{
    public const float DEFAULT_REACTION_MULTIPLIER = 15.5f;

    ParticleSystemForceField reactionForceField;

    Transform otherPart;

    Rigidbody2D rigidBody = null;

    ParticleSystem particles = null;

    SpriteRenderer[] childSprites;

    // Keep original scale to undo changes while repooling
    Vector3 originalScale;

    void Awake() {
        // Define particle system
        particles = transform.Find("Particle System").GetComponent<ParticleSystem>();

        // Get original scale
        originalScale = transform.localScale;

        // Make it indestructible through out of screen bounds
        DestructibleObject destructibleScript = GetComponent<DestructibleObject>();
        if (destructibleScript != null) {
            destructibleScript.SetIsDestructibleNow(false);

            // Alter particle system stop action to destroy the object
            ParticleSystem.MainModule mainModule = particles.main;
            mainModule.stopAction = ParticleSystemStopAction.Callback;
            SetDestructibleOnParticleStop destroyCallback = particles.gameObject.AddComponent<SetDestructibleOnParticleStop>();
            destroyCallback.SetDestructibleScript(destructibleScript);
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
        emission.rateOverTimeMultiplier *= transform.localScale.x * DEFAULT_REACTION_MULTIPLIER;

        // TODO Check if multiple scripts are really used
        // Remove possible moving object scripts
        IMovingObject[] movingObjectScripts = GetComponents<IMovingObject>();
        for (int i = movingObjectScripts.Length-1; i >= 0; i--) {
            //Destroy(movingObjectScripts[i]);
            movingObjectScripts[i].enabled = false;
        }

        // Keep reference to child sprites
        childSprites = GetComponentsInChildren<SpriteRenderer>();
    }

    void FixedUpdate() {
        // Move towards the reaction center
        rigidBody.AddForce((reactionForceField.transform.position - transform.position));

        // Concentrate sprites
        foreach (SpriteRenderer sprite in childSprites) {
            sprite.transform.localScale = Vector3.Lerp(sprite.transform.localScale, sprite.transform.localScale * 0.3f, Time.deltaTime);
        }

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

    // Called to return to pool
    void OnDestroy() {
        // Return rigid body
        rigidBody.bodyType = RigidbodyType2D.Dynamic;
        rigidBody.drag = 0;
        
        // Return emission
        ParticleSystem.EmissionModule emission = particles.emission;
        emission.rateOverTimeMultiplier = Energy.DEFAULT_AMOUNT_PARTICLES;
        transform.localScale = originalScale;

        // Return sprites to default scale
        foreach (SpriteRenderer sprite in childSprites) {
            sprite.transform.localScale = Vector3.one;
        }
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
