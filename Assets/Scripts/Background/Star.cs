using UnityEngine;
using System.Collections;

public class Star : MovingBackgroundElement {

	// Use this for initialization
	void Start () {
		if (GameController.RollChance(10)) {
			// Add shiny star script
			gameObject.AddComponent<ShinyStar>();
		}

        // Set background moving speed depending on scale
        float speedFactor = Random.Range(1.1f - (StarGenerator.MIN_STAR_SCALE / transform.localScale.x), 
			1.15f - (StarGenerator.MIN_STAR_SCALE / transform.localScale.x));
        SetSpeed(Mathf.Pow(speedFactor, 2));
        //		Debug.Log("Speed: " + getSpeed() + " Scale: " + defaultScale.x);
    }

	// Update is called once per frame
	void Update () {
		Move();
	}

	/*
	 * Getters and Setters
	 */

}
