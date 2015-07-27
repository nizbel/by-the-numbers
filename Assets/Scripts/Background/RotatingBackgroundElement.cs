using UnityEngine;
using System.Collections;

public class RotatingBackgroundElement : MonoBehaviour {

	float speed = 1;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		rotate ();
	}
	
	public void rotate() {
//		transform.localRotation = Quaternion.Lerp(transform.localRotation, 
//		                                          Quaternion.Euler(transform.localRotation.x, transform.localRotation.y,
//		                                                                            transform.localRotation.z + speed), Time.deltaTime);
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
}
