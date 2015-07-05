using UnityEngine;
using System.Collections;

public class SubtractBlock : OperationBlock {

	void Start() {
		if (PowerUpController.controller.getAvailablePowerUp(PowerUpController.NEUTRALIZER_POWER_UP)) {
			value = 0;

			// Change color
			renderer.material.color = new Color(0, 1, 0, 0.6f);
		} else {
			value = 1;
		}
	}

	public override int operation(int curValue) {
		return (curValue - value);
	}
}
