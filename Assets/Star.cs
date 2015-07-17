using UnityEngine;
using System.Collections;

public class Star : MonoBehaviour {

	bool isShining = false;

	Vector3 defaultScale;

	float shiningSpeed;

	// Use this for initialization
	void Start () {
		if (Random.Range(0, 100) >= 90) {
			isShining = true;
		}
		defaultScale = transform.localScale;
		shiningSpeed = Random.Range(0.01f, 0.05f);
//		Debug.Log("Varying between " + defaultScale.x + " and " + (defaultScale.x * (1 + (0.05f/defaultScale.x))));
	}
	
	// Update is called once per frame
	void Update () {
		if (isShining) {
			if (transform.localScale.x <= defaultScale.x) {
				shiningSpeed = Mathf.Abs(shiningSpeed);
			} else if (transform.localScale.x > defaultScale.x * (1 + (0.05f/defaultScale.x))) {
				shiningSpeed = -1 * Mathf.Abs(shiningSpeed);
			}
			transform.localScale = Vector3.Lerp(transform.localScale, transform.localScale + new Vector3(shiningSpeed, shiningSpeed, 0), Time.deltaTime);
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
