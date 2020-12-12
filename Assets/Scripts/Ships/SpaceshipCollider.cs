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
		switch (col.collider.tag) {
			case "Power Up":
				PlayerController.controller.PowerUpCollisionReaction(col.collider);
				break;

			case "Obstacle":
			case "Indestructible Obstacle":
				PlayerController.controller.ObstacleCollisionReaction(col.GetContact(0));
				break;

			case "Frail Obstacle":
				// Damage obstacle then destroy ship
				DissolvingObject dissolveScript = col.collider.gameObject.AddComponent<DissolvingObject>();
				dissolveScript.SetDissolutionByDamage();
				PlayerController.controller.ObstacleCollisionReaction(col.GetContact(0));
				break;
        }
	}

	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.tag == "Energy") {
			PlayerController.controller.EnergyCollisionReaction(collider);
		} else if (collider.tag == "Energy Strike") {
			PlayerController.controller.EnergyStrikeCollisionReaction(collider);
		}
	}
}