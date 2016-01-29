using UnityEngine;
using System.Collections;

public class MothershipMovement : MonoBehaviour {
	
	[SerializeField]
	float speed;
	
	[SerializeField]
	Vector3 upperPosition;
	
	[SerializeField]
	Vector3 lowerPosition;

	// If true, mothership moves up, false moves down
	bool goingUp;

	// Use this for initialization
	void Start () {
		if (Random.Range(0, 10) >= 5) {
			goingUp = true;
		} else {
			goingUp = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (goingUp) {
			transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y +speed, transform.position.z), Time.deltaTime);
			if (Mathf.Abs(transform.position.y - upperPosition.y) < 0.1f) {
				goingUp = false;
			}
		} else {
			transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y -speed, transform.position.z), Time.deltaTime);
			if (Mathf.Abs(transform.position.y - lowerPosition.y) < 0.1f) {
				goingUp = true;
			}
		}
	}
}
