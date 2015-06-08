using UnityEngine;
using System.Collections;

public class PlayerBlock : MonoBehaviour {

	public int value = 0;

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

	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.gameObject.tag == "Block") {
			Debug.Log("Collided with block");
			value = collider.gameObject.GetComponent<OperationBlock>().operation(value);
			Destroy(collider.gameObject);
			StageController.controller.blockCaught();
		}
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
