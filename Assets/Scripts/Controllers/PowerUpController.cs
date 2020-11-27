using UnityEngine;
using System.Collections;

public class PowerUpController : MonoBehaviour {

	// Constants
	public const int NEUTRALIZER_POWER_UP = 0;
	public const int GROWTH_POWER_UP = 1;
	public const int DIMINISH_POWER_UP = 2;
	public const int DOUBLE_METEORS_POWER_UP = 3;

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

			availablePowerUps = new bool[4];
			// Initialize available power ups
			for (int i = 0; i < availablePowerUps.Length; i++) {
				availablePowerUps[i] = false;
			}

			powerUpStartTimes = new float[4];
			// Initialize power up start times
			for (int i = 0; i < powerUpStartTimes.Length; i++) {
				powerUpStartTimes[i] = -1;
			}
			
			powerUpDurationIntervals = new float[4];
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
						PassEffect(i);
					}
				}
			}
		}
	}

	public void SetEffect (int powerUp)
	{		
		switch (powerUp) {
		case NEUTRALIZER_POWER_UP:
			// Set neutralizer as on 
			availablePowerUps[NEUTRALIZER_POWER_UP] = true;

            Energy[] energies = (Energy[]) GameObject.FindObjectsOfType(typeof(Energy));
			foreach (Energy energy in energies) {
				energy.SetValue(0);
				energy.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 0.6f);
			}
			break;

		case GROWTH_POWER_UP:
			// If power up isn't already on
			if (!availablePowerUps[GROWTH_POWER_UP]) {
				// Set growth on
				availablePowerUps[GROWTH_POWER_UP] = true;

				Vector3 playerCurrentScale = PlayerController.controller.transform.localScale;
				PlayerController.controller.transform.localScale = new Vector3(playerCurrentScale.x*1.25f, 
				                                                                         playerCurrentScale.y*1.25f, playerCurrentScale.z*1.25f);
			}
			break;
		}

		// Set start time
		powerUpStartTimes[powerUp] = Time.timeSinceLevelLoad;

		// Show that there is one power up activated
		anyPowerUpActivated = true;
	}
	
	public void PassEffect (int powerUp)
	{
		switch (powerUp) {
		case NEUTRALIZER_POWER_UP:
			// Set neutralizer as off 
			availablePowerUps[NEUTRALIZER_POWER_UP] = false;

                Energy[] energies = (Energy[]) GameObject.FindObjectsOfType(typeof(Energy));
			foreach (Energy energy in energies) {
				energy.SetValue(1);
				energy.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1);
			}
			break;

		case GROWTH_POWER_UP:
			// Set neutralizer as off 
			availablePowerUps[GROWTH_POWER_UP] = false;
			
			Vector3 playerCurrentScale = PlayerController.controller.transform.localScale;
			PlayerController.controller.transform.localScale = new Vector3(playerCurrentScale.x/1.25f, 
			                                                                         playerCurrentScale.y/1.25f, playerCurrentScale.z/1.25f);
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
	public bool GetAvailablePowerUp(int powerUp) {
		return availablePowerUps[powerUp];
	}

	public void SetAvailablePowerUp(int powerUp, bool isActive) {
		availablePowerUps[powerUp] = isActive;
	}

}
