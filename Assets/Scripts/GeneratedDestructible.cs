using UnityEngine;
using System.Collections;

public class GeneratedDestructible : MonoBehaviour {

	BackgroundElementGenerator generator;

	/*
	 * Getters and Setters
	 */
	public BackgroundElementGenerator getGenerator() {
		return generator;
	}
	
	public void setGenerator(BackgroundElementGenerator generator) {
		this.generator = generator;
	}

	void OnDisable() {
		if (generator != null) {
			generator.DecreaseAmountAlive();
		}
	}
}
