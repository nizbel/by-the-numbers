using UnityEngine;
using System.Collections;

public class PlayerShip : MonoBehaviour {

	public const float DEFAULT_SHIP_SPEED = 9.5f;
	public const float ASSIST_MODE_SHIP_SPEED = 7.5f;

	[SerializeField]
	int value = 0;

	float speed = DEFAULT_SHIP_SPEED;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.gameObject.tag == "Block") {
			value = collider.gameObject.GetComponent<OperationBlock>().Operation(value);

			// Change player block color
			GetComponent<SpriteRenderer>().color = new Color(1 - Mathf.Max(0, (float) value / StageController.SHIP_VALUE_LIMIT), 
				1 - Mathf.Abs((float) value / StageController.SHIP_VALUE_LIMIT), 1 - Mathf.Max(0, (float) value/-StageController.SHIP_VALUE_LIMIT));

			// Play sound on collision
			PlayEffect(collider.gameObject);

			// Disappear block
			collider.gameObject.GetComponent<OperationBlock>().Disappear();
            StageController.controller.BlockCaught();
		} else if (collider.gameObject.tag == "Power Up") {
			PowerUpController.controller.SetEffect(collider.gameObject.GetComponent<PowerUp>().getType());

			// Play sound on collision
			PlayEffect(collider.gameObject);

			//TODO disappear power up
			collider.gameObject.GetComponent<SpriteRenderer>().enabled = false;
			collider.gameObject.GetComponent<BoxCollider2D>().enabled = false;
		} else if (collider.gameObject.tag == "Obstacle") {
			StageController.controller.GameOver();
		}
	}

	private void PlayEffect(GameObject gameObject) {
		if (gameObject.GetComponent<AudioSource>() != null) {
			gameObject.GetComponent<AudioSource>().Play();
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
