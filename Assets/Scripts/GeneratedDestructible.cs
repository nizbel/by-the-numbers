using UnityEngine;
using System.Collections;

public class GeneratedDestructible : MonoBehaviour {

	BackgroundElementGenerator generator;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/*
	 * Getters and Setters
	 */
	public BackgroundElementGenerator getGenerator() {
		return generator;
	}
	
	public void setGenerator(BackgroundElementGenerator generator) {
		this.generator = generator;
	}

	void OnDestroy() {
		if (generator != null) {
			generator.DecreaseAmountAlive();
		}
	}
}
