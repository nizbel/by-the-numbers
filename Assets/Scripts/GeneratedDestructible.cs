using UnityEngine;
using System.Collections;

public class GeneratedDestructible : MonoBehaviour {

	Generator generator;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/*
	 * Getters and Setters
	 */
	public Generator getGenerator() {
		return generator;
	}
	
	public void setGenerator(Generator generator) {
		this.generator = generator;
	}

	void OnDestroy() {
		if (generator != null) {
			generator.decreaseAmountAlive();
		}
	}
}
