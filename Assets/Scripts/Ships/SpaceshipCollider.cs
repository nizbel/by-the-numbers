﻿using UnityEngine;
using System.Collections;

public class SpaceshipCollider : MonoBehaviour {

	void OnCollisionEnter2D(Collision2D col) {
		switch (col.collider.tag) {
			case "Obstacle":
			case "Indestructible Obstacle":
			case "Stray Engine":
				// TODO Explode stray engine on collision
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
		}
	}
}