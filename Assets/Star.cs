using UnityEngine;
using System.Collections;

public class Star : MovingBackgroundElement {

	bool isShining = false;

	Vector3 defaultScale;

	float shiningSpeed;

	// Use this for initialization
	void Start () {
		if (Random.Range(0, 100) >= 90) {
			isShining = true;
		}
		defaultScale = transform.localScale;
		// Set background moving speed depending on scale
		float speedFactor = Random.Range(1.1f - (0.01f/defaultScale.x), 1.25f - (0.01f/defaultScale.x));
		setSpeed(Mathf.Pow(speedFactor, 3));
//		Debug.Log("Speed: " + getSpeed() + " Scale: " + defaultScale.x);
		shiningSpeed = Random.Range(0.05f, 0.2f);
//		Debug.Log("Varying between " + defaultScale.x + " and " + (defaultScale.x * (1 + (0.05f/defaultScale.x))));
	}
	
	// Update is called once per frame
	void Update () {
		move();
		if (isShining) {
			if (transform.localScale.x <= defaultScale.x) {
				shiningSpeed = Mathf.Abs(shiningSpeed);
			} else if (transform.localScale.x > defaultScale.x * (1 + (0.05f/defaultScale.x))) {
				shiningSpeed = -1 * Mathf.Abs(shiningSpeed);
			}
			transform.localScale = Vector3.Lerp(transform.localScale, transform.localScale + new Vector3(shiningSpeed, shiningSpeed, 0), Time.deltaTime);
		}
		if (canDelete()) {
			Destroy(this.gameObject);
		}
	}

	/*
	 * Getters and Setters
	 */

	public bool getIsShining() {
		return isShining;
	}

	public void setIsShining(bool isShining) {
		this.isShining = isShining;
	}

	public float getShiningSpeed() {
		return shiningSpeed;
	}
	
	public void setShiningSpeed(float shiningSpeed) {
		this.shiningSpeed = shiningSpeed;
	}
}
