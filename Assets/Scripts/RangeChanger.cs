using UnityEngine;
using System.Collections;

public class RangeChanger : MonoBehaviour {

	Transform player;

	bool finished = false;

	bool positive;

	// Use this for initialization
	void Start () {
		player = PlayerController.controller.transform;
	}
	
	// Update is called once per frame
	void Update () {
		if (!finished) {
			if (player.position.x > this.transform.position.x) {
				ValueRange.rangeController.ChangeRange(positive);
				StageController.controller.PastThroughRangeChanger();

                //// Accelerate player block
                //if (player.gameObject.GetComponent<PlayerShip>().GetSpeed() < 3) {
                //player.gameObject.GetComponent<PlayerShip>().SetSpeed(player.gameObject.GetComponent<PlayerShip>().GetSpeed() + 0.5f);
                //}

                finished = true;
			}
		}
	}

	/*
	 * Getters and Setters
	 */
	public void SetPositive(bool positive) {
		this.positive = positive;
		if (this.positive) {
			GetComponent<SpriteRenderer>().color = new Color(0.05f, 0.05f, 0.92f);
		}
		else {
			GetComponent<SpriteRenderer>().color = new Color(0.92f, 0.05f, 0.05f);
		}
	}
}
