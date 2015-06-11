using UnityEngine;
using System.Collections;

public class PowerUp : MonoBehaviour {

	// Power up duration
	float effectDuration = 10;

	// Moment power up started
	float effectStart;

	// Shows if the effect was already applied
	bool effectStarted = false;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (effectStarted) {
			if (Time.timeSinceLevelLoad - effectStart > effectDuration) {
				passEffect();
			}
		}
	}

	public virtual void setEffect() {
		effectStart = Time.timeSinceLevelLoad;
		effectStarted = true;
	}

	public virtual void passEffect() {

	}

	/*
	 * Getters and Setters
	 */
	public float getEffectDuration() {
		return effectDuration;
	}

	public void setEffectDuration(float effectDuration) {
		this.effectDuration = effectDuration;
	}

	public float getEffectStart() {
		return effectStart;
	}
	
	public void setEffectStart(float effectStart) {
		this.effectStart = effectStart;
	}
}
