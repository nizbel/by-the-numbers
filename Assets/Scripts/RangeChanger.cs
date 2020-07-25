using UnityEngine;
using System.Collections;

public class RangeChanger : MonoBehaviour {

	Transform player;

	bool finished = false;

	bool changeToUp;

	// Use this for initialization
	void Start () {
		player = StageController.controller.GetPlayerTransform();

		// Define whether it's an increase or decrease for the range
		if (GameController.RollChance(50)) {
			changeToUp = true;
			GetComponent<Renderer>().material.color = new Color(0.05f, 0.05f, 0.92f);
		} else {
			changeToUp = false;
			GetComponent<Renderer>().material.color = new Color(0.92f, 0.05f, 0.05f);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!finished) {
			if (player.position.x > this.transform.position.x) {
				ValueRange.rangeController.ChangeRange(changeToUp);
				StageController.controller.PastThroughRangeChanger();

                //// Accelerate player block
                //if (player.gameObject.GetComponent<PlayerShip>().GetSpeed() < 3) {
                //player.gameObject.GetComponent<PlayerShip>().SetSpeed(player.gameObject.GetComponent<PlayerShip>().GetSpeed() + 0.5f);
                //}

                finished = true;
			}
		}
	}
}
