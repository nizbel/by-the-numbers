using UnityEngine;
using System.Collections;

public class ValueRange : MonoBehaviour {

	private const int CHANGE_INTERVAL = 2;
	public const int INTERVAL = 5;

	int maxValue = INTERVAL;

	int minValue = -INTERVAL;

	// Warning
	public GameObject magneticBarrierWarningPrefab;
	MagneticBarrierWarning magneticBarrierWarning;

	public static ValueRange controller;

	void Awake() {
		if (controller == null) {
			controller = this;
		} 
		else {
			Destroy(this.gameObject);
		}
	}

	// Use this for initialization
	void Start () {
		//this.transform.GetChild(0).GetComponent<TextMesh>().text = "Min: " + minValue + " Max: " + maxValue;
	}

	public void ChangeRange(bool goingUp) {
		// Range going up
		if (goingUp) {
			minValue += CHANGE_INTERVAL;
			maxValue += CHANGE_INTERVAL;
			
			// Fix values if it passes the limit
			if (maxValue > StageController.SHIP_VALUE_LIMIT) {
				minValue -= (maxValue - StageController.SHIP_VALUE_LIMIT);
				maxValue = StageController.SHIP_VALUE_LIMIT;
            }
		}
		//Range going down
		else {
			minValue -= CHANGE_INTERVAL;
			maxValue -= CHANGE_INTERVAL;

			// Fix values if it passes the limit
			if (minValue < -StageController.SHIP_VALUE_LIMIT) {
				maxValue -= (minValue + StageController.SHIP_VALUE_LIMIT);
				minValue = StageController.SHIP_VALUE_LIMIT;
			}
		}
		//this.transform.GetChild(0).GetComponent<TextMesh>().text = "Min: " + minValue + " Max: " + maxValue;
	}

	public GameObject ActivateMagneticBarrierWarning(bool positiveBarrier) {
		// Generate warning if it does not exist
		if (magneticBarrierWarning == null) {
			magneticBarrierWarning = GameObject.Instantiate(magneticBarrierWarningPrefab).GetComponent<MagneticBarrierWarning>();
		} else {
			magneticBarrierWarning.gameObject.SetActive(true);
        }

		magneticBarrierWarning.SetIsPositiveWarning(positiveBarrier);

		return magneticBarrierWarning.gameObject;
	}

	/*
	 * Getters and Setters
	 */
	public int GetMaxValue() {
		return maxValue;
	}

	public void SetMaxValue(int maxValue) {
		this.maxValue = maxValue;
	}

	public int GetMinValue() {
		return minValue;
	}

	public void SetMinValue(int minValue) {
		this.minValue = minValue;
	}
}
