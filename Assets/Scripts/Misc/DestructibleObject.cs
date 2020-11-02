using UnityEngine;
using System.Collections;
using System;

public class DestructibleObject : MonoBehaviour {

	// Keeps track of object's speed in order to prepare destructible ordered list
	private float speed;

	// Checks if already added into destructible's list
	private Boolean addedToList = false;

	// Guarantees the object can be destroyed
	private Boolean destructibleNow = true;

	// Use this for initialization
	void Start () {
    }

    public float GetSpeed() {
        return speed;
    }

	public void SetSpeed(float speed) {
		this.speed = speed;
		if (!addedToList) {
			OutScreenDestroyerController.controller.AddToDestructibleList(this.gameObject);
			addedToList = true;
		}
	}

	public bool IsDestructibleNow() {
		return destructibleNow;
	}

	public void SetIsDestructibleNow(bool destructibleNow) {
		this.destructibleNow = destructibleNow;
	}
}
