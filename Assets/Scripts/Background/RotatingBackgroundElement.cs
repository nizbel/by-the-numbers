using UnityEngine;
using System.Collections;

public class RotatingBackgroundElement : MonoBehaviour {

	public const float MIN_DEFAULT_ROTATING_SPEED = 0.1f;
	public const float MAX_DEFAULT_ROTATING_SPEED = 1;

	float speed;

	float minSpeed = MIN_DEFAULT_ROTATING_SPEED;
	float maxSpeed = MAX_DEFAULT_ROTATING_SPEED;
	
	// Use this for initialization
	void Start () {
		speed = Random.Range(minSpeed, maxSpeed);
	}
	
	// Update is called once per frame
	void Update () {
		rotate ();
	}
	
	public void rotate() {
		transform.Rotate(0, 0, speed, Space.World);
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

	public float getMinSpeed() {
		return minSpeed;
	}

	public void setMinSpeed(float minSpeed) {
		this.minSpeed = minSpeed;
	}

	public float getMaxSpeed() {
		return maxSpeed;
	}

	public void setMaxSpeed(float maxSpeed) {
		this.maxSpeed = maxSpeed;
	}
}
