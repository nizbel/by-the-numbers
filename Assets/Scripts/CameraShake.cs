using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour {

	public const float DEFAULT_SHAKE_DURATION = 0.5f;
	public const float DEFAULT_SHAKE_AMOUNT = 0.25f;

	// How long the object should shake for.
	float shakeDuration = DEFAULT_SHAKE_DURATION;

	// Amplitude of the shake. A larger value shakes the camera harder.
	float shakeAmount = DEFAULT_SHAKE_AMOUNT;

	Vector3 originalPos;

	void OnEnable() {
		originalPos = transform.localPosition;
	}

	void Update() {
		if (shakeDuration > 0) {
			if (!StageController.controller.GetGamePaused()) {
				Vector3 randomPosition = Random.insideUnitCircle * shakeAmount;
				transform.localPosition = originalPos + randomPosition;

				shakeDuration -= Time.deltaTime;
			}
		}
		else {
			transform.localPosition = originalPos;
			enabled = false;
		}
	}

	public void DefaultShake() {
		Shake(DEFAULT_SHAKE_DURATION, DEFAULT_SHAKE_AMOUNT);
	}
	public void Shake(float duration, float amount) {
		if (CanShake(amount)) {
			shakeDuration = duration;
			shakeAmount = amount;
			enabled = true;
		}
	}

	// Shake only if not already shaking or new amount is higher
	private bool CanShake(float amount) {
		return !enabled || amount > shakeAmount;
    }
}