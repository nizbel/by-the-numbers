using UnityEngine;
using System.Collections;

public class AddBlock : OperationBlock {

	void Start() {
		if (PowerUpController.controller.GetAvailablePowerUp(PowerUpController.NEUTRALIZER_POWER_UP)) {
			value = 0;
			
			// Change color
			GetComponent<Renderer>().material.color = new Color(0, 1, 0, 0.6f);
		} else {
			value = 1;
		}
	}

	public override int Operation(int curValue) {
		return (curValue + value);
	}
}
