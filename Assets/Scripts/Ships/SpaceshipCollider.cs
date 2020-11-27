﻿using UnityEngine;
using System.Collections;

public class SpaceshipCollider : MonoBehaviour {

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.gameObject.tag == "Energy") {
			PlayerController.controller.EnergyCollisionReaction(collider);
		}
		else if (collider.gameObject.tag == "Power Up") {
			PlayerController.controller.PowerUpCollisionReaction(collider);
		}
		else if (collider.gameObject.tag == "Obstacle") {
			PlayerController.controller.ObstacleCollisionReaction(collider);
		}
        else if (collider.gameObject.tag == "Mine") {
            PlayerController.controller.MineCollisionReaction(collider);
        }
        else if (collider.gameObject.tag == "Energy Strike") {
            PlayerController.controller.EnergyStrikeCollisionReaction(collider);
        }
    }
}