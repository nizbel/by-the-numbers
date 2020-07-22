using UnityEngine;
using System.Collections;

public class PlayerShip : MonoBehaviour {

	[SerializeField]
	int value = 0;

	float speed = 6;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate() {	
//		transform.Translate(Vector3.right * speed * Time.deltaTime);
		//transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x + speed,
		//                                                                  transform.position.y, transform.position.z), Time.deltaTime);
	}

	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.gameObject.tag == "Block") {
			value = collider.gameObject.GetComponent<OperationBlock>().Operation(value);

			// Change player block color
			GetComponent<Renderer>().material.color = new Color(1 - Mathf.Max(0, (float) value/(50)), 1 - Mathf.Abs((float) value/50), 1 - Mathf.Max(0, (float) value/-50));

			//			Destroy(collider.gameObject);
			collider.gameObject.SetActive(false);
			StageController.controller.BlockCaught();
		} else if (collider.gameObject.tag == "Power Up") {
			PowerUpController.controller.SetEffect(collider.gameObject.GetComponent<PowerUp>().getType());
			collider.gameObject.SetActive(false);
		} else if (collider.gameObject.tag == "Obstacle") {
			StageController.controller.GameOver();
		}
	}

	/*
	 * Getters and Setters
	 */

	public int GetValue() {
		return value;
	}
	
	public void SetValue(int value) {
		this.value = value;
	}

	public float GetSpeed() {
		return speed;
	}

	public void SetSpeed(float speed) {
		this.speed = speed;
	}
}
