using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debris : MovingBackgroundElement
{
	private const float MIN_DEBRIS_ROTATING_SPEED = 0.1f;
	private const float MAX_DEBRIS_ROTATING_SPEED = 1.6f;

	// Use this for initialization
	void Start() {
		if (GameController.RollChance(75)) {
			// Add rotating background object script
			RotatingBackgroundElement rotatingScript = gameObject.AddComponent<RotatingBackgroundElement>();
			rotatingScript.setMinSpeed(MIN_DEBRIS_ROTATING_SPEED);
			rotatingScript.setMaxSpeed(MAX_DEBRIS_ROTATING_SPEED);
		}

		// Set background moving speed depending on scale
		SetSpeed(Mathf.Pow(transform.localScale.x, 2));
	}
}
