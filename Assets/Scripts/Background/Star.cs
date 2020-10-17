using UnityEngine;
using System.Collections;

public class Star : MonoBehaviour {

	// Use this for initialization
	void Start() {
		if (GameController.RollChance(10)) {
			// Add shiny star script
			gameObject.AddComponent<ShinyStar>();
		}
	}

	void Update() {

	}

}
