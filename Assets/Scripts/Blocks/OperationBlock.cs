using UnityEngine;
using System.Collections;
using Light2D = UnityEngine.Experimental.Rendering.Universal.Light2D;
using UnityEngine.Events;

public class OperationBlock : MonoBehaviour {

	// Constants
	private const float ENERGY_SHAKE_AMOUNT = 0.075f;
	private const float ENERGY_SHAKE_DURATION = 0.15f;

	protected int value;

	protected UnityEvent onDisappear = new UnityEvent();

	// Particles that go into the ship
	public GameObject latchingParticlesPrefab;

	public GameObject energyShock;
	
	// Update is called once per frame
	void Update () {
	}

	public virtual int Operation(int curValue) {
		return curValue;
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
		if (GetComponent<BoxCollider2D>()) {
			GetComponent<BoxCollider2D>().enabled = false;
		} else if (GetComponent<CircleCollider2D>()) {
			GetComponent<CircleCollider2D>().enabled = false;
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
        transform.Find("Particle System").gameObject.GetComponent<ParticleSystem>().Stop();
		//transform.Find("Particle System").gameObject.SetActive(false);

		//      if (transform.Find("Latching Particles") != null) {
		//	GameObject latchingParticles = transform.Find("Latching Particles").gameObject;
		//          latchingParticles.transform.parent = null;
		//          latchingParticles.GetComponent<ParticleSystem>().Play();
		//}

		if (latchingParticlesPrefab != null) {
			GameObject latchingParticles = GameObject.Instantiate<GameObject>(latchingParticlesPrefab, null);
			latchingParticles.transform.position = transform.position;

			// Set value for ship energy state
			latchingParticles.GetComponent<ParticlesAffectShip>().Value = GetComponent<AddBlock>() != null ? 1 : -1;

			latchingParticles.GetComponent<ParticleSystem>().Play();
		}
		Camera.main.GetComponent<CameraShake>().Shake(ENERGY_SHAKE_DURATION, ENERGY_SHAKE_AMOUNT);

		// Invoke disappear events
		onDisappear.Invoke();
	}

	//void OnTriggerEnter2D(Collider2D collider) {
	//	// Collision with another energy
	//	if (collider.gameObject.tag == "Block" && energyShock != null) {
	//		if (GetComponent<SubtractBlock>() != null && collider.GetComponent<SubtractBlock>() != null) {
	//			Vector3 distance = collider.transform.position - transform.position;
	//			collider.attachedRigidbody.AddForceAtPosition(distance, collider.transform.position);
	//			GetComponent<Rigidbody2D>().AddForceAtPosition(-distance, collider.transform.position);

	//			// Create energy shock effect
	//			Vector3 halfDistance = distance / 2;
	//			// Get angle that is perpendicular to distance
	//			float angle = Vector3.SignedAngle(Vector3.right, halfDistance, Vector3.forward) + 90;
	//			GameObject.Instantiate(energyShock, transform.position + halfDistance, Quaternion.AngleAxis(angle, Vector3.forward));
	//		}
	//	}
	//}

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
