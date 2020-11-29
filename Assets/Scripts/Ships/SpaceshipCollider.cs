using UnityEngine;
using System.Collections;

public class SpaceshipCollider : MonoBehaviour {

	// Use this for initialization
	void Start() {

	}

	// Update is called once per frame
	void Update() {

	}

	void OnCollisionEnter2D(Collision2D col) {
		if (col.collider.tag == "Power Up") {
			PlayerController.controller.PowerUpCollisionReaction(col.collider);
		}
		else if (col.collider.tag == "Obstacle") {
			PlayerController.controller.ObstacleCollisionReaction(col.GetContact(0));
		}
		// TODO Change this to trigger
		else if (col.collider.tag == "Energy Strike") {
			PlayerController.controller.EnergyStrikeCollisionReaction(col.collider);
		}
	}

	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.gameObject.tag == "Energy") {
			PlayerController.controller.EnergyCollisionReaction(collider);
		}
	}
}