using UnityEngine;
using System.Collections;

public class MovingBackgroundElement : MonoBehaviour {

	float speed = 1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		move();
	}

	public void move() {
		transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(transform.localPosition.x - speed,
		                                                                            transform.localPosition.y, transform.localPosition.z), Time.deltaTime);
	}
		
	/*
	 * Getters and Setters
	 */
	public float getSpeed() {
		return speed;
	}

	public void setSpeed(float speed) {
		this.speed = speed;
	}
}
