using UnityEngine;
using System.Collections;

public class ShinyStar : MonoBehaviour {
	
	float shiningSpeed;
	
	Vector3 defaultScale;

	// Use this for initialization
	void Start () {		
		defaultScale = transform.localScale;
		shiningSpeed = Random.Range(0.05f, 0.2f);
		//		Debug.Log("Varying between " + defaultScale.x + " and " + (defaultScale.x * (1 + (0.05f/defaultScale.x))));
	}

	// Update is called once per frame
	void Update () {
		if (transform.localScale.x <= defaultScale.x) {
			shiningSpeed = Mathf.Abs(shiningSpeed);
		} else if (transform.localScale.x > defaultScale.x * (1 + (0.05f/defaultScale.x))) {
			shiningSpeed = -1 * Mathf.Abs(shiningSpeed);
		}
		transform.localScale = Vector3.Lerp(transform.localScale, transform.localScale + new Vector3(shiningSpeed, shiningSpeed, 0), Time.deltaTime);
	}

	/*
	 * Getters and Setters
	 */

	public float getShiningSpeed() {
		return shiningSpeed;
	}
	
	public void setShiningSpeed(float shiningSpeed) {
		this.shiningSpeed = shiningSpeed;
	}
}
