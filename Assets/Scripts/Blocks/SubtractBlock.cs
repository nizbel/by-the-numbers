using UnityEngine;
using System.Collections;

public class SubtractBlock : OperationBlock {

	void Start() {
		if (PowerUpController.controller.GetAvailablePowerUp(PowerUpController.NEUTRALIZER_POWER_UP)) {
			value = 0;

			//// Change color
			//GetComponent<Renderer>().material.color = new Color(0, 1, 0, 0.6f);
		} else {
			value = 1;
		}
	}

	public override int Operation(int curValue) {
		return (curValue - value);
	}

	void OnTriggerEnter2D(Collider2D collider) {
		// Collision with another energy
		if (collider.gameObject.tag == "Block") {
			if (collider.GetComponent<SubtractBlock>() != null) {
				Vector3 distance = collider.transform.position - transform.position;
				// Move energies
				collider.attachedRigidbody.AddForceAtPosition(distance, collider.transform.position);
				GetComponent<Rigidbody2D>().AddForceAtPosition(-distance, collider.transform.position);

				Vector3 halfDistance = distance / 2;
				// Get angle that is perpendicular to distance
				float angle = Vector3.SignedAngle(Vector3.right, halfDistance, Vector3.forward) + 90;
				// Create energy shock effect
				GameObject.Instantiate(energyShock, transform.position + halfDistance, Quaternion.AngleAxis(angle, Vector3.forward));
			} else {
				ReactOnCollision(collider);
            }
		}
	}
}
