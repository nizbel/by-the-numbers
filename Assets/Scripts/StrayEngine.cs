using System.Collections;
using UnityEngine;

public class StrayEngine : DestructibleObject {
    // Chances
    public const float DEFAULT_ACTIVATING_CHANCE = 33.33f;
    public const float DEFAULT_ROTATING_CHANCE = 50f;
    public const float DEFAULT_PRE_ACTIVATED_CHANCE = 50f;

    // Intervals
    public const float ENGINE_FIRE_WARMING_INTERVAL = 0.1f;

    [SerializeField]
    Vector2 spriteAmount = new Vector2(1,1);

    ParticleSystem fragments = null;

    [SerializeField]
    bool activeEngine = false;

    // Determines if engine has been activated for a while
    [SerializeField]
    bool isPreActivated = false;

    [SerializeField]
    DirectionalMovingObject directionalMovingScript;

    [SerializeField]
    RotatingObject rotatingScript;

    bool shouldShakeOnActivate = true;

    [SerializeField]
    ParticleSystemForceField forceField;

    StrayEngineActivator activatorScript = null;

    [SerializeField]
    [Tooltip("Trigger that activates inactive engines")]
    BoxCollider2D energyEffect = null;

    [SerializeField]
    [Tooltip("Game object with the particles that appear when activated")]
    StrayEngineEffects activatedParticles;

    // Define chance for rotation
    float rotatingChance = DEFAULT_ROTATING_CHANCE;

    [SerializeField]
    // Value dictates which type of energy is the engine using
    int value;

	public override void OnObjectSpawn() {
		base.OnObjectSpawn();

		// Proceed to enable all possible components

		// Enable colliders
		Collider2D[] colliders = GetComponents<Collider2D>();
		foreach (Collider2D collider in colliders) {
			collider.enabled = true;
		}

        // Reset default stray engine values
        activeEngine = false;
        shouldShakeOnActivate = true;

        isPreActivated = GameController.RollChance(DEFAULT_PRE_ACTIVATED_CHANCE);
        value = GameController.RollChance(50f) ? 1 : -1;

        // Add activator script
        activatorScript = gameObject.AddComponent<StrayEngineActivator>();
        if (isPreActivated) {
            // Make sure it will come out as activated
            activatorScript.SetActivatingChance(100f);
            // Enable energy effect
            energyEffect.enabled = true;
        } else {
            activatorScript.SetActivatingChance(DEFAULT_ACTIVATING_CHANCE);
            // Energy effect to activate other engines is disabled
            energyEffect.enabled = false;
        }
        rotatingChance = DEFAULT_ROTATING_CHANCE;
    }


    void Start() {
        // Set random seed and dissolving amount
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.material.SetVector("_Seed", new Vector4(Random.Range(0, 1f), Random.Range(0, 1f), 0, 0));
        float dissolveAmount = Random.Range(0, 0.3f);
        spriteRenderer.material.SetFloat("_DissolveAmount", dissolveAmount);
        spriteRenderer.material.SetVector("_SpriteAmount", new Vector4(spriteAmount.x, spriteAmount.y, 0, 0));

        // Set color as transparent
        spriteRenderer.material.SetColor("_DissolveColorOuter", new Color(1,1,1,0));
        spriteRenderer.material.SetColor("_DissolveColorMiddle", new Color(1, 1, 1, 0));
        spriteRenderer.material.SetColor("_DissolveColorInner", new Color(1, 1, 1, 0));

        //if (dissolveAmount >= 0.3f) {
        //    foreach (Transform child in transform) {
        //        child.gameObject.SetActive(true);
        //        fragments = child.GetComponent<ParticleSystem>();

        //        // Change fragments
        //        ParticleSystemRenderer fragmentsRenderer = child.GetComponent<ParticleSystemRenderer>();
        //        fragmentsRenderer.material.SetColor("_DissolveColorOuter", new Color(1, 1, 1, 0));
        //        fragmentsRenderer.material.SetColor("_DissolveColorMiddle", new Color(1, 1, 1, 0));
        //        fragmentsRenderer.material.SetColor("_DissolveColorInner", new Color(1, 1, 1, 0));
        //    }
        //}
    }

    public override void OnObjectDespawn() {
		// TODO Workaround for destructible objects list in OutScreenDestroyerController
		FixAddedToList();

        // Remove dissolving particles
        foreach (Transform child in transform) {
            if (child.name.StartsWith("Dissolving Particles")) {
                Destroy(child.gameObject);
            }
        }

        // Remove dissolving script if available
        DissolvingObject dissolvingScript = GetComponent<DissolvingObject>();
        if (dissolvingScript != null) {
            Destroy(dissolvingScript);
        }

		// Remove movement scripts
		RemoveMovementScripts();

		ObjectPool.SharedInstance.ReturnPooledObject(GetPoolType(), gameObject);
	}

    public void Activate() {
        activeEngine = true;

        // Set value for the particles colors
        activatedParticles.SetEngineValue(value);

        activatedParticles.StartShock();

        StartCoroutine(ActivateEngine());
        if (shouldShakeOnActivate) {
            gameObject.AddComponent<ShakyObject>();
        }

        // Can activate other engines now
        energyEffect.enabled = true;
    }

    IEnumerator ActivateEngine() {
        if (isPreActivated) {
            activatedParticles.StartEngineFire(true);
        } else {
            yield return new WaitForSeconds(Random.Range(0.1f, 0.2f));
            activatedParticles.StartEngineFire();
            yield return new WaitForSeconds(ENGINE_FIRE_WARMING_INTERVAL);
        }
        // Activate directional movement and rotation
        directionalMovingScript.enabled = true;

        if (GameController.RollChance(rotatingChance)) {
            rotatingScript.enabled = true;
        }

        // Stop shaking
        ShakyObject shakyObjectScript = GetComponent<ShakyObject>();
        if (shakyObjectScript != null) {
            Destroy(shakyObjectScript);
        }
    }

    public void ProcessCollisionBase() {
        // Checks if debris belongs in a formation, if true remove it
        Formation parentFormation = transform.parent.GetComponent<Formation>();
        if (parentFormation != null) {
            transform.parent = parentFormation.transform.parent;

            if (this == parentFormation.GetCenterElement()) {
                parentFormation.SetCenterElement(null);
            }
            parentFormation.ImpactFormation();
        }

        RemoveMovementScripts();
    }

    // Remove energy movement scripts
    private void RemoveMovementScripts() {
        IMovingObject movingScript = GetComponent<IMovingObject>();

        movingScript.enabled = false;

        rotatingScript.enabled = false;
    }

    public void DisappearFragments() {
        if (fragments != null) {
            fragments.Stop();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        switch (collision.gameObject.tag) {
            case "Stray Engine":
                if (!activeEngine) {
                    SetValue(collision.GetComponent<StrayEngine>().GetValue());
                    Activate();
                }
                break;

            case "Energy":
                if (!activeEngine) {
                    AbsorbEnergy(collision.GetComponent<Energy>());
                } // TODO Check if there is a way not to make this value check here and in Energy class
                else if (collision.GetComponent<Energy>().GetValue() * value < 0) {
                    // Absorb energy
                    collision.GetComponent<Energy>().Disappear(forceField, false);
                    // Stop particles
                    activatedParticles.StopEffects();
                    // Dissolve
                    DissolvingObject dissolveScript = gameObject.AddComponent<DissolvingObject>();
                    dissolveScript.SetDissolutionByEnergy(value);
                }
                break;
        }
    }

    public void AbsorbEnergy(Energy energy) {
        value = energy.GetValue();

        Activate(); 

        // TODO Disappear absorbed by a force field
        energy.Disappear(forceField, false);
    }

    void OnCollisionEnter2D(Collision2D collision) {
        ProcessCollisionBase();
    }

    public void SetActivatorChance(float activatingChance) {
        activatorScript.SetActivatingChance(activatingChance);
    }

    public void SetShouldShakeOnActivate(bool shouldShakeOnActivate) {
        this.shouldShakeOnActivate = shouldShakeOnActivate;
    }

    public void SetRotatingChance(float rotatingChance) {
        this.rotatingChance = rotatingChance;
    }

    public void SetIsPreActivated(bool isPreActivated) {
        this.isPreActivated = isPreActivated;
    }

    public int GetValue() {
        return value;
    }

    public void SetValue(int value) {
        this.value = value;
    }
}
