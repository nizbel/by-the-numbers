using UnityEngine;
using System.Collections;
using Light2D = UnityEngine.Experimental.Rendering.Universal.Light2D;
using UnityEngine.Events;

public class Energy : MonoBehaviour {

	// Constants
	private const float ENERGY_SHAKE_AMOUNT = 0.075f;
	private const float ENERGY_SHAKE_DURATION = 0.15f;

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
	
	void Start() {
		if (PowerUpController.controller.GetAvailablePowerUp(PowerUpController.NEUTRALIZER_POWER_UP)) {
			value = 0;
		}
		else {
			value = baseValue;
		}
	}

	// Update is called once per frame
	void Update () {
	}

	public int Operation(int curValue) {
		return (curValue + value);
	}

	public void Disappear() {
		// Disable sprites
		if (GetComponent<SpriteRenderer>()) {
			GetComponent<SpriteRenderer>().enabled = false;
		}

		SpriteRenderer[] childSprites = GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer sprite in childSprites) {
			sprite.enabled = false;
		}

		// Disable colliders
		//if (GetComponent<BoxCollider2D>()) {
		//	GetComponent<BoxCollider2D>().enabled = false;
		//} else if (GetComponent<CircleCollider2D>()) {
			GetComponent<CircleCollider2D>().enabled = false;
		//}

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
		latchingParticles.GetComponent<ParticlesAffectShip>().Value = GetComponent<Energy>().GetValue();

		latchingParticles.GetComponent<ParticleSystem>().Play();
		Camera.main.GetComponent<CameraShake>().Shake(ENERGY_SHAKE_DURATION, ENERGY_SHAKE_AMOUNT);

		// Invoke disappear events
		onDisappear.Invoke();
	}

	public void DisappearInReaction() {
		// Disable sprites
		if (GetComponent<SpriteRenderer>()) {
			GetComponent<SpriteRenderer>().enabled = false;
		}

		SpriteRenderer[] childSprites = GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer sprite in childSprites) {
			sprite.enabled = false;
		}

		// Disable colliders
		//if (GetComponent<BoxCollider2D>()) {
		//	GetComponent<BoxCollider2D>().enabled = false;
		//}
		//else if (GetComponent<CircleCollider2D>()) {
			//GetComponent<CircleCollider2D>().enabled = false;
		//}

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

	void OnTriggerEnter2D(Collider2D collider) {
		// Collision with another energy
		if (collider.gameObject.tag == "Energy") {
			if (collider.GetComponent<Energy>().GetValue() * value > 0) {
				Vector3 distance = collider.transform.position - transform.position;
				collider.attachedRigidbody.AddForceAtPosition(distance, collider.transform.position);
				GetComponent<Rigidbody2D>().AddForceAtPosition(-distance, collider.transform.position);

				// Create energy shock effect
				Vector3 halfDistance = distance / 2;
				// Get angle that is perpendicular to distance
				float angle = Vector3.SignedAngle(Vector3.right, halfDistance, Vector3.forward) + 90;
				GameObject.Instantiate(energyShock, transform.position + halfDistance, Quaternion.AngleAxis(angle, Vector3.forward));
			}
			else {
				ReactOnCollision(collider);
			}
		} else if (collider.gameObject.tag == "Obstacle") {
			// TODO For now just move the energy away
			Vector3 distance = collider.transform.position - transform.position;
			GetComponent<Rigidbody2D>().AddForceAtPosition(-distance, collider.transform.position);

			// Create energy shock effect
			Vector3 halfDistance = distance / 2;
			// Get angle that is perpendicular to distance
			float angle = Vector3.SignedAngle(Vector3.right, halfDistance, Vector3.forward) + 90;
			GameObject.Instantiate(energyShock, transform.position + halfDistance, Quaternion.AngleAxis(angle, Vector3.forward));
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
