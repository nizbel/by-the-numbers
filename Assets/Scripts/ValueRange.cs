using UnityEngine;
using System.Collections;

public class ValueRange : MonoBehaviour {

	private const int CHANGE_INTERVAL = 2;

	[SerializeField]
	int maxValue = 5;

	[SerializeField]
	int minValue = -5;

	public static ValueRange rangeController;

	void Awake() {
		if (rangeController == null) {
			rangeController = this;
		} 
		else {
			Destroy(this.gameObject);
		}
	}

	// Use this for initialization
	void Start () {
		this.transform.GetChild(0).GetComponent<TextMesh>().text = "Min: " + minValue + " Max: " + maxValue;
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void ChangeRange() {
		// Range going up
		if (GameController.RollChance(50)) {
			minValue += CHANGE_INTERVAL;
			maxValue += CHANGE_INTERVAL;
		}
		//Range going down
		else {
			minValue -= CHANGE_INTERVAL;
			maxValue -= CHANGE_INTERVAL;
		}
		this.transform.GetChild(0).GetComponent<TextMesh>().text = "Min: " + minValue + " Max: " + maxValue;
	}

	/*
	 * Getters and Setters
	 */
	public int GetMaxValue() {
		return maxValue;
	}

	public int GetMinValue() {
		return minValue;
	}
}
