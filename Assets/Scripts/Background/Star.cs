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
			if (GameController.RollChance(100)) {
				float randomColor = Random.Range(0.75f, 1.75f);
				// TODO Ridiculous variable name
				GetComponent<SpriteRenderer>().material.SetColor("Color_64919911", new Color(randomColor, randomColor, randomColor, 0));
			}
		}
	}
}
