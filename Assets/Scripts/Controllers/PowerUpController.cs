using UnityEngine;
using System.Collections;

public class PowerUpController : MonoBehaviour {

	// Constants
	public const int NEUTRALIZER_POWER_UP = 0;

	/*
	 * Effects that are on
	 */
	bool[] availablePowerUps;

	/*
	 * Activation start times
	 */
	float[] powerUpStartTimes;

	/*
	 * Power up duration intervals
	 */
	float[] powerUpDurationIntervals;

	// Boolean to check if there is any power up activated at the moment
	bool anyPowerUpActivated = false;

	public static PowerUpController controller;

	void Awake() {
		if (controller == null) {
			controller = this;

			availablePowerUps = new bool[1];
			// Initialize available power ups
			for (int i = 0; i < availablePowerUps.Length; i++) {
				availablePowerUps[i] = false;
			}

			powerUpStartTimes = new float[1];
			// Initialize power up start times
			for (int i = 0; i < powerUpStartTimes.Length; i++) {
				powerUpStartTimes[i] = -1;
			}
			
			powerUpDurationIntervals = new float[1];
			// Initialize power up start times
			for (int i = 0; i < powerUpDurationIntervals.Length; i++) {
				powerUpDurationIntervals[i] = 10;
			}
		}
		else {
			Destroy(gameObject);
		}
	}

	void Update() {
		/*
		 * Checks if there is any power up activated. If there is at least one, run through the array of power ups,
		 * checking if any of them is activated and if the start time indicates that it should be deactivated
		 */
		if (anyPowerUpActivated) {
			for (int i = 0; i < availablePowerUps.Length; i++) {
				if (availablePowerUps[i]) {
					if (Time.timeSinceLevelLoad - powerUpStartTimes[i] > powerUpDurationIntervals[i]) {
						passEffect(i);
					}
				}
			}
		}
	}

	public void setEffect (int powerUp)
	{		
		switch (powerUp) {
		case NEUTRALIZER_POWER_UP:
			// Set neutralizer as on 
			availablePowerUps[NEUTRALIZER_POWER_UP] = true;
			
			OperationBlock[] operationBlocks = (OperationBlock[]) GameObject.FindObjectsOfType(typeof(OperationBlock));
			foreach (OperationBlock block in operationBlocks) {
				block.setValue(0);
				block.renderer.material.color = new Color(0, 1, 0, 0.6f);
			}
			break;
		}

		// Set start time
		powerUpStartTimes[powerUp] = Time.timeSinceLevelLoad;

		// Show that there is one power up activated
		anyPowerUpActivated = true;
	}
	
	public void passEffect (int powerUp)
	{
		switch (powerUp) {
		case NEUTRALIZER_POWER_UP:
			// Set neutralizer as off 
			availablePowerUps[NEUTRALIZER_POWER_UP] = false;
			
			OperationBlock[] operationBlocks = (OperationBlock[]) GameObject.FindObjectsOfType(typeof(OperationBlock));
			foreach (OperationBlock block in operationBlocks) {
				block.setValue(1);
				block.renderer.material.color = new Color(1, 1, 1, 1);
			}
			break;
		}
		
		// Set start time
		powerUpStartTimes[powerUp] = -1;

		// Checks if there is any other power up activated
		bool powerUpActivated = false;
		for (int i = 0; i < availablePowerUps.Length; i++) {
			if (availablePowerUps[i]) {
				powerUpActivated = true;
			}
		}
		if (!powerUpActivated) {
			anyPowerUpActivated = false;
		}
	}

	/*
	 * Getters and Setters
	 */
	public bool getAvailablePowerUp(int powerUp) {
		return availablePowerUps[powerUp];
	}

	public void setAvailablePowerUp(int powerUp, bool isActive) {
		availablePowerUps[powerUp] = isActive;
	}

}
