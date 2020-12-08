using UnityEngine;
using System.Collections;

public class Star : MonoBehaviour {

	// Use this for initialization
	void Start() {
		if (GameController.RollChance(10)) {
			// Add shiny star script
			gameObject.AddComponent<ShinyStar>();
		}

		// TODO make this better
		if (GameController.RollChance(20)) {
			float randomColor = Random.Range(1f, 1.5f);
			// TODO Ridiculous variable name
			GetComponent<SpriteRenderer>().material.SetColor("Color_64919911", new Color(randomColor, randomColor, randomColor, 1));
		}
	}
}
