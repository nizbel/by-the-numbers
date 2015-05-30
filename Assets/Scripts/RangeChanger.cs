using UnityEngine;
using System.Collections;

public class RangeChanger : MonoBehaviour {

	Transform player;

	bool finished = false;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player").transform;
	}
	
	// Update is called once per frame
	void Update () {
		if (!finished) {
			if (player.position.x > this.transform.position.x) {
				ValueRange.rangeController.changeRange();
				StageController.controller.pastThroughRangeChanger();

				// Accelerate player block
				if (player.gameObject.GetComponent<PlayerBlock>().getSpeed() < 10) {
					player.gameObject.GetComponent<PlayerBlock>().setSpeed(player.gameObject.GetComponent<PlayerBlock>().getSpeed()+1);
				}

				finished = true;
			}
		}
	}
}
