using UnityEngine;
using System.Collections;

public class ChubbyMovement : MonoBehaviour {

	float speed = 4;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}

	void FixedUpdate() {
		transform.Translate(Vector3.right * speed * Time.deltaTime);
	}

	public void setSpeed(float speed) {
		this.speed = speed;
	}
}
