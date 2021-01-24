using UnityEngine;
using System.Collections;

public class RotatingObject : MonoBehaviour {

	public const float MIN_DEFAULT_ROTATING_SPEED = 1;
	public const float MAX_DEFAULT_ROTATING_SPEED = 10;

	float speed;

	[SerializeField]
	float minSpeed = MIN_DEFAULT_ROTATING_SPEED;
	[SerializeField]
	float maxSpeed = MAX_DEFAULT_ROTATING_SPEED;
	
	// Use this for initialization
	void Start () {
		speed = Random.Range(minSpeed, maxSpeed);
		if (GameController.RollChance(50)) {
			speed *= -1;
        }
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Rotate ();
	}
	
	public void Rotate() {
		transform.Rotate(0, 0, speed * Time.deltaTime);
	}
	
	/*
	 * Getters and Setters
	 */
	public float GetSpeed() {
		return speed;
	}
	
	public void SetSpeed(float speed) {
		this.speed = speed;
	}

	public float GetMinSpeed() {
		return minSpeed;
	}

	public void SetMinSpeed(float minSpeed) {
		this.minSpeed = minSpeed;
	}

	public float GetMaxSpeed() {
		return maxSpeed;
	}

	public void SetMaxSpeed(float maxSpeed) {
		this.maxSpeed = maxSpeed;
	}
}
