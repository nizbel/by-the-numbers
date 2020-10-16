using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour {

	public const float DEFAULT_SHAKE_DURATION = 0.5f;
	// How long the object should shake for.
	float shakeDuration = DEFAULT_SHAKE_DURATION;

	// Amplitude of the shake. A larger value shakes the camera harder.
	float shakeAmount = 0.3f;

	Vector3 originalPos;

	void OnEnable() {
		originalPos = transform.localPosition;
		shakeDuration = DEFAULT_SHAKE_DURATION;
	}

	void Update() {
		if (shakeDuration > 0) {
			Vector3 randomPosition = Random.insideUnitCircle * shakeAmount;
			transform.localPosition = originalPos + randomPosition ;

			shakeDuration -= Time.deltaTime;
		}
		else {
			transform.localPosition = originalPos;
			enabled = false;
		}
	}
}