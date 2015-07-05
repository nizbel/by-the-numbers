using UnityEngine;
using System.Collections;

public class PlayerBlock : MonoBehaviour {

	[SerializeField]
	int value = 0;

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
			value = collider.gameObject.GetComponent<OperationBlock>().operation(value);

			// Change player block color
			renderer.material.color = new Color(1 - Mathf.Max(0, (float) value/(50)), 1 - Mathf.Abs((float) value/50), 1 - Mathf.Max(0, (float) value/-50));

			Destroy(collider.gameObject);
			StageController.controller.blockCaught();
		} else if (collider.gameObject.tag == "Power Up") {
			PowerUpController.controller.setEffect(collider.gameObject.GetComponent<PowerUp>().getType());
			Destroy(collider.gameObject);
		} else if (collider.gameObject.tag == "Obstacle") {
			StageController.controller.gameOver();
		}
	}

	/*
	 * Getters and Setters
	 */

	public int getValue() {
		return value;
	}
	
	public void setValue(int value) {
		this.value = value;
	}

	public float getSpeed() {
		return speed;
	}

	public void setSpeed(float speed) {
		this.speed = speed;
	}
}
