using UnityEngine;
using System.Collections;
using Light2D = UnityEngine.Experimental.Rendering.Universal.Light2D;

public class OperationBlock : MonoBehaviour {

	protected int value;
	
	// Update is called once per frame
	void Update () {
	}

	public virtual int Operation(int curValue) {
		return curValue;
	}

	public void Disappear() {
		// Disable sprites
		if (GetComponent<SpriteRenderer>()) {
			GetComponent<SpriteRenderer>().enabled = false;
		}

		SpriteRenderer[] childSprites = GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer sprite in childSprites) {
			sprite.enabled = false;
		}

		// Disable colliders
		if (GetComponent<BoxCollider2D>()) {
			GetComponent<BoxCollider2D>().enabled = false;
		} else if (GetComponent<CircleCollider2D>()) {
			GetComponent<CircleCollider2D>().enabled = false;
		}

		// Disable lights
		if (GetComponent<Light2D>()) {
			GetComponent<Light2D>().enabled = false;
		}

		Light2D[] childLights = GetComponentsInChildren<Light2D>();
		foreach (Light2D light in childLights) {
			light.enabled = false;
		}
	}

	/*
	 * Getters and Setters
	 */

	public void SetValue(int value) {
		this.value = value;
	}

	public int GetValue() {
		return value;
	}
}
