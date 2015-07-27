using UnityEngine;
using System.Collections;

public class Star : MovingBackgroundElement {

//	bool isShining = false;

//	float shiningSpeed;

	// Use this for initialization
	void Start () {
		if (Random.Range(0, 100) >= 90) {
			// Add shiny star script
			gameObject.AddComponent<ShinyStar>();
		}
		// Set background moving speed depending on scale
		float speedFactor = Random.Range(1.1f - (0.01f/transform.localScale.x), 1.25f - (0.01f/transform.localScale.x));
		setSpeed(Mathf.Pow(speedFactor, 3));
//		Debug.Log("Speed: " + getSpeed() + " Scale: " + defaultScale.x);
	}
	
	// Update is called once per frame
	void Update () {
		move();
	}

	/*
	 * Getters and Setters
	 */

}
