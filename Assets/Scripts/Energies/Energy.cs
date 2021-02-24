using UnityEngine;
using System.Collections;
using Light2D = UnityEngine.Experimental.Rendering.Universal.Light2D;
using UnityEngine.Events;

public class Energy : DestructibleObject {

	// Constants
	private const float ENERGY_SHAKE_AMOUNT = 0.075f;
	private const float ENERGY_SHAKE_DURATION = 0.15f;
	public const int DEFAULT_AMOUNT_PARTICLES = 16;

	// Keeps the current value
	int value;
	// Keeps the default value for the element
	[SerializeField]
	int baseValue;
	
	protected UnityEvent onDisappear = new UnityEvent();

	// Particles that go into the ship
	public GameObject latchingParticlesPrefab;

	public GameObject energyShock;
	public GameObject energyReaction;
	public GameObject energyBurst;

	public override void OnObjectSpawn() {
		if (destructibleType == 0) {
			DefineMultipleSpriteType();
		}
		value = baseValue;

		// Proceed to enable all possible components

		// Enable sprites
		SpriteRenderer[] childSprites = GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer sprite in childSprites) {
			sprite.enabled = true;
		}

		// Enable collider
		GetComponent<CircleCollider2D>().enabled = true;

		// Enable lights
		if (GetComponent<Light2D>()) {
			GetComponent<Light2D>().enabled = true;
		}

		Light2D[] childLights = GetComponentsInChildren<Light2D>();
		foreach (Light2D light in childLights) {
			light.enabled = true;
		}

		// Start particle animation
		transform.Find("Particle System").GetComponent<ParticleSystem>().Play();
	}

    public override void OnObjectDespawn() {
		// TODO Workaround for destructible objects list in OutScreenDestroyerController
		FixAddedToList();

		// Remove EnergyReactionPart if it exists
		EnergyReactionPart reactionScript = GetComponent<EnergyReactionPart>();
		if (reactionScript != null) {
			Destroy(reactionScript);
		}

		// Remove movement scripts
		RemoveMovementScripts();

		ObjectPool.SharedInstance.ReturnPooledObject(GetPoolType(), gameObject);

		// Remove listeners
		onDisappear.RemoveAllListeners();
    }

	public void Disappear(ParticleSystemForceField forceField, bool isPlayerForceField) {
		// Disable sprites
		SpriteRenderer[] childSprites = GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer sprite in childSprites) {
			sprite.enabled = false;
		}

		// Disable colliders
		GetComponent<CircleCollider2D>().enabled = false;

		// Disable lights
		if (GetComponent<Light2D>()) {
			GetComponent<Light2D>().enabled = false;
		}

		Light2D[] childLights = GetComponentsInChildren<Light2D>();
		foreach (Light2D light in childLights) {
			light.enabled = false;
		}

        // Stop particle animation to shoot latching particles
        transform.Find("Particle System").GetComponent<ParticleSystem>().Stop();

		GameObject latchingParticles = GameObject.Instantiate<GameObject>(latchingParticlesPrefab, null);
		latchingParticles.transform.position = transform.position;

		// Set value for ship energy state
		if (isPlayerForceField) {
			latchingParticles.GetComponent<ParticlesAffectShip>().Value = value;
		}

		// Prepare particle system
		ParticleSystem latchingParticlesPS = latchingParticles.GetComponent<ParticleSystem>();
		ParticleSystem.ExternalForcesModule externalForces = latchingParticlesPS.externalForces;
		externalForces.AddInfluence(forceField);
		latchingParticlesPS.Play();
		
		GameController.GetCamera().GetComponent<CameraShake>().Shake(ENERGY_SHAKE_DURATION, ENERGY_SHAKE_AMOUNT);

		// Invoke disappear events
		onDisappear.Invoke();
	}

	public void DisappearInReaction() {
		// Disable sprites
		SpriteRenderer[] childSprites = GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer sprite in childSprites) {
			sprite.enabled = false;
		}

		// Disable lights
		if (GetComponent<Light2D>()) {
			GetComponent<Light2D>().enabled = false;
		}

		Light2D[] childLights = GetComponentsInChildren<Light2D>();
		foreach (Light2D light in childLights) {
			light.enabled = false;
		}

		// Test shooting particles
		transform.Find("Particle System").GetComponent<ParticleSystem>().Stop();
	}

	// Reaction of two different energies fusing together then exploding
	protected void ReactOnCollision(Collider2D collider) {
		if (GetComponent<EnergyReactionPart>() == null && collider.GetComponent<EnergyReactionPart>() == null) {
			Vector3 distance = collider.transform.position - transform.position;
			GameObject reaction = GameObject.Instantiate(energyReaction, transform.position + distance / 2, new Quaternion(0, 0, 0, 1));
            // Make reaction move with other elements
            reaction.transform.parent = transform;
			reaction.transform.localScale = Vector3.one;

            // Establish link to the reaction
            EnergyReactionPart reactionPart = gameObject.AddComponent<EnergyReactionPart>();
			reactionPart.SetReactionForceField(reaction.GetComponent<ParticleSystemForceField>());
			reactionPart.SetOtherPart(collider.transform);

			// Link collided object
			EnergyReactionPart colliderReactionPart = collider.gameObject.AddComponent<EnergyReactionPart>();
			colliderReactionPart.SetReactionForceField(reaction.GetComponent<ParticleSystemForceField>());
			colliderReactionPart.SetOtherPart(transform);

			// Disable colliders
			collider.enabled = false;
			GetComponent<CircleCollider2D>().enabled = false;
		}
	}

	public void ProcessCollisionBase() {
        // Checks if energy belongs in a formation, if true remove it
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
        RadialInOutMovement radialMovementScript = GetComponent<RadialInOutMovement>();
        if (radialMovementScript != null) {
            Destroy(radialMovementScript);
        }
		IMovingObject movingScript = GetComponent<IMovingObject>();
		//if (movingScript != null) {
		//	Destroy(movingScript);
		//}
		movingScript.enabled = false;
	}

    void OnTriggerEnter2D(Collider2D collider) {
		switch (collider.tag) {
			case "Energy":
				ProcessCollisionBase();
				// Collision with another energy
				if (collider.GetComponent<Energy>().GetValue() * value > 0) {
					Vector3 distanceEnergyCollision = collider.transform.position - transform.position;
					collider.attachedRigidbody.AddForceAtPosition(distanceEnergyCollision, collider.transform.position);
					GetComponent<Rigidbody2D>().AddForceAtPosition(-distanceEnergyCollision, collider.transform.position);
					GetComponent<IMovingObject>().enabled = true;

					// Create energy shock effect
					Vector3 halfDistanceEnergyCollision = distanceEnergyCollision / 2;
					// Get angle that is perpendicular to distance
					float angleEnergyCollision = Vector3.SignedAngle(Vector3.right, halfDistanceEnergyCollision, Vector3.forward) + 90;
					GameObject newShock = GameObject.Instantiate(energyShock, transform.position + halfDistanceEnergyCollision, Quaternion.AngleAxis(angleEnergyCollision, Vector3.forward));
					newShock.transform.parent = transform;
					newShock.transform.localScale = Vector3.one;
				}
				else {
					ReactOnCollision(collider);
				}
				break;

			case "Obstacle":
			case "Indestructible Obstacle":
				ProcessCollisionBase();
				// TODO For now just move the energy away
				Vector3 distanceObstacleCollision = collider.transform.position - transform.position;
				GetComponent<Rigidbody2D>().AddForceAtPosition(-distanceObstacleCollision, collider.transform.position);
				GetComponent<IMovingObject>().enabled = true;

				// Create energy shock effect
				Vector3 halfDistanceObstacleCollision = distanceObstacleCollision / 2;
				// Get angle that is perpendicular to distance
				float angleObstacleCollision = Vector3.SignedAngle(Vector3.right, halfDistanceObstacleCollision, Vector3.forward) + 90;
				GameObject.Instantiate(energyShock, transform.position + halfDistanceObstacleCollision, Quaternion.AngleAxis(angleObstacleCollision, Vector3.forward));
				break;

			case "Frail Obstacle":
				DissolvingObject dissolveScript = collider.gameObject.AddComponent<DissolvingObject>();
				dissolveScript.SetDissolutionByEnergy(value);
				break;

			// TODO Change lightning fuse to energy fuse
			case "Lightning Fuse":
				LightningFuse lightningFuseScript = collider.GetComponent<LightningFuse>();
				lightningFuseScript.AbsorbEnergy(this);
				break;

			case "Bullet Time Detection":
				break;

			default:
				// Remove energy from formation
				ProcessCollisionBase();
				break;
		}
	}

	void OnTriggerStay2D(Collider2D collider) {
		switch (collider.tag) {
			case "Obstacle":
			case "Indestructible Obstacle":
				// TODO For now just move the energy away
				Vector3 distanceObstacleCollision = collider.transform.position - transform.position;
				GetComponent<Rigidbody2D>().AddForceAtPosition(-distanceObstacleCollision, collider.transform.position);
				GetComponent<IMovingObject>().enabled = true;

				// Create energy shock effect
				Vector3 halfDistanceObstacleCollision = distanceObstacleCollision / 2;
				// Get angle that is perpendicular to distance
				float angleObstacleCollision = Vector3.SignedAngle(Vector3.right, halfDistanceObstacleCollision, Vector3.forward) + 90;
				GameObject.Instantiate(energyShock, transform.position + halfDistanceObstacleCollision, Quaternion.AngleAxis(angleObstacleCollision, Vector3.forward));
				break;
		}
	}

	public void AddDisappearListener(UnityAction action) {
		onDisappear.AddListener(action);
	}

	/*
	 * Getters and Setters
	 */

	public void SetValue(int value) {
		this.value = value;
	}

	public int GetValue() {
		return value;
	}
}
