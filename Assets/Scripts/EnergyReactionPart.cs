using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyReactionPart : MonoBehaviour
{
    ParticleSystemForceField reactionForceField;

    // Start is called before the first frame update
    void Start()
    {
        ParticleSystem.ExternalForcesModule externalForces = transform.Find("Particle System").GetComponent<ParticleSystem>().externalForces;
        externalForces.enabled = true;
        externalForces.AddInfluence(reactionForceField);
    }

    // Update is called once per frame
    void Update()
    {
        if ((transform.position - reactionForceField.transform.position).sqrMagnitude < 0.005f) {
            reactionForceField.transform.Find("Particle System").gameObject.SetActive(true);
            GetComponent<OperationBlock>().DisappearInReaction();
            this.enabled = false;
        }
    }

    void FixedUpdate() {
        // Move towards the reaction center
        transform.position = Vector3.Lerp(transform.position, reactionForceField.transform.position, 3*Time.deltaTime);

        // Concentrate sprites
        SpriteRenderer[] childSprites = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sprite in childSprites) {
            sprite.transform.localScale = Vector3.Lerp(sprite.transform.localScale, sprite.transform.localScale * 0.3f, Time.deltaTime);
        }

        // Energize
        ParticleSystem.EmissionModule emission = transform.Find("Particle System").GetComponent<ParticleSystem>().emission;
        emission.rateOverTimeMultiplier += 5;
    }

    /*
     * Getters and Setters
     */
    public void SetReactionForceField(ParticleSystemForceField reactionForceField) {
        this.reactionForceField = reactionForceField;
    }
}
