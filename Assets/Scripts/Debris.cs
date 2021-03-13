using UnityEngine;

public class Debris : DestructibleObject {
    [SerializeField]
    Vector2 spriteAmount = new Vector2(5,5);

    ParticleSystem fragments = null;

	public override void OnObjectSpawn() {
		base.OnObjectSpawn();

		// Proceed to enable all possible components

		// Enable colliders
		Collider2D[] colliders = GetComponents<Collider2D>();
		foreach (Collider2D collider in colliders) {
			collider.enabled = true;
		}
	}

    void Start() {
        // Set random seed and dissolving amount
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

        GetComponent<IMovingObject>().enabled = false;

        ObjectPool.SharedInstance.ReturnPooledObject(GetPoolType(), gameObject);
	}

	public void ProcessCollisionBase() {
        // Checks if debris belongs in a formation, if true remove it
        Formation parentFormation = transform.parent.GetComponent<Formation>();
        if (parentFormation != null) {
            transform.parent = parentFormation.transform.parent;

            if (transform == parentFormation.GetCenterElement()) {
                parentFormation.SetCenterElement(null);
            }
            parentFormation.ImpactFormation();
        }

        RemoveMovementScripts();
    }

    // Remove energy movement scripts
    private void RemoveMovementScripts() {
        //IMovingObject movingScript = GetComponent<IMovingObject>();
        //if (movingScript != null) {
        //    Destroy(movingScript);
        //}
        //movingScript.enabled = false;

        RotatingObject rotatingScript = GetComponent<RotatingObject>();
        if (rotatingScript != null) {
            Destroy(rotatingScript);
        }
    }

    public void DisappearFragments() {
        if (fragments != null) {
            fragments.Stop();
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        ProcessCollisionBase();
    }
}
