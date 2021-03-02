﻿using System.Collections;
using UnityEngine;

public class StrayEngine : DestructibleObject {
    public const float DEFAULT_ACTIVATING_CHANCE = 33.33f;
    public const float DEFAULT_ROTATING_CHANCE = 50f;

    [SerializeField]
    Vector2 spriteAmount = new Vector2(1,1);

    ParticleSystem fragments = null;

    bool activeEngine = false;

    [SerializeField]
    DirectionalMovingObject directionalMovingScript;

    [SerializeField]
    RotatingObject rotatingScript;

    bool shouldShakeOnActivate = true;

    StrayEngineActivator activatorScript = null;

    [SerializeField]
    [Tooltip("Trigger that activates inactive engines")]
    BoxCollider2D energyEffect = null;

    // TODO Define chance for rotation
    float rotatingChance = DEFAULT_ROTATING_CHANCE;

	public override void OnObjectSpawn() {
		base.OnObjectSpawn();

		// Proceed to enable all possible components

		// Enable colliders
		Collider2D[] colliders = GetComponents<Collider2D>();
		foreach (Collider2D collider in colliders) {
			collider.enabled = true;
		}

        // Add activator script
        activatorScript = gameObject.AddComponent<StrayEngineActivator>();
        activatorScript.SetActivatingChance(DEFAULT_ACTIVATING_CHANCE);
    }


    void Start() {
        // Set random seed and dissolving amount
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.material.SetVector("_Seed", new Vector4(Random.Range(0, 1f), Random.Range(0, 1f), 0, 0));
        float dissolveAmount = Random.Range(0, 0.4f);
        spriteRenderer.material.SetFloat("_DissolveAmount", dissolveAmount);
        spriteRenderer.material.SetVector("_SpriteAmount", new Vector4(spriteAmount.x, spriteAmount.y, 0, 0));

        // Set color as transparent
        spriteRenderer.material.SetColor("_DissolveColorOuter", new Color(1,1,1,0));
        spriteRenderer.material.SetColor("_DissolveColorMiddle", new Color(1, 1, 1, 0));
        spriteRenderer.material.SetColor("_DissolveColorInner", new Color(1, 1, 1, 0));

        if (dissolveAmount >= 0.3f) {
            foreach (Transform child in transform) {
                child.gameObject.SetActive(true);
                fragments = child.GetComponent<ParticleSystem>();

                // Change fragments
                ParticleSystemRenderer fragmentsRenderer = child.GetComponent<ParticleSystemRenderer>();
                fragmentsRenderer.material.SetColor("_DissolveColorOuter", new Color(1, 1, 1, 0));
                fragmentsRenderer.material.SetColor("_DissolveColorMiddle", new Color(1, 1, 1, 0));
                fragmentsRenderer.material.SetColor("_DissolveColorInner", new Color(1, 1, 1, 0));
            }
        }
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
        StartCoroutine(ActivateEngine());
        if (shouldShakeOnActivate) {
            gameObject.AddComponent<ShakyObject>();
        }
    }

    IEnumerator ActivateEngine() {
        yield return new WaitForSeconds(Random.Range(0.2f, 0.3f));
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

    // TODO Test if this works
    private void OnTriggerEnter2D(Collider2D collision) {
        if (!activeEngine) {
            if (collision.gameObject.GetComponent<StrayEngine>() != null) {
                Activate();
            }
        }
    }
}
