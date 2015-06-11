using UnityEngine;
using System.Collections;

public class PowerUpController : MonoBehaviour {

	bool neutralizer = false;

	public static PowerUpController controller;

	void Awake() {
		if (controller == null) {
			controller = this;
		}
		else {
			Destroy(gameObject);
		}
	}

	/*
	 * Getters and Setters
	 */
	public bool getNeutralizer() {
		return neutralizer;
	}

	public void setNeutralizer(bool neutralizer) {
		this.neutralizer = neutralizer;
	}

}
