using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonDebris : MonoBehaviour {
	private const float MIN_DEBRIS_ROTATING_SPEED = 12.5f;
	private const float MAX_DEBRIS_ROTATING_SPEED = 52.5f;

	// Use this for initialization
	void Start() {
		if (GameController.RollChance(75)) {
			// Add rotating background object script
			RotatingObject rotatingScript = gameObject.AddComponent<RotatingObject>();
			rotatingScript.SetMinSpeed(MIN_DEBRIS_ROTATING_SPEED);
			rotatingScript.SetMaxSpeed(MAX_DEBRIS_ROTATING_SPEED);
		}
	}
}
