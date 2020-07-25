using UnityEngine;
using System.Collections;

public abstract class MovingBackgroundElement : MonoBehaviour {

	public const float DEFAULT_SPEED = 1;

	float speed;

	// Use this for initialization
	void Start () {
		SetSpeed(DEFAULT_SPEED);
	}
	
	// Update is called once per frame
	void Update () {
		Move();
	}

	public void Move() {
		transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(transform.localPosition.x 
			- (speed * StageController.controller.GetPlayerShipSpeed() / PlayerShip.DEFAULT_SHIP_SPEED),
			transform.localPosition.y, transform.localPosition.z), Time.deltaTime);
	}
		
	/*
	 * Getters and Setters
	 */
	public float GetSpeed() {
		return speed;
	}

	public void SetSpeed(float speed) {
		this.speed = speed;
		if (transform.GetComponent<DestructibleObject>() != null) {
			transform.GetComponent<DestructibleObject>().SetSpeed(speed);
		}
	}
}
