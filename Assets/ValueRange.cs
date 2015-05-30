using UnityEngine;
using System.Collections;

public class ValueRange : MonoBehaviour {

	public int maxValue = 50;

	public int minValue = -50;

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

	public void changeRange() {
		// Range going up
		if (Random.Range(0, 2) == 0) {
			minValue += 2;
			maxValue += 2;
		}
		//Range going down
		else {
			minValue -= 2;
			maxValue -= 2;
		}
		this.transform.GetChild(0).GetComponent<TextMesh>().text = "Min: " + minValue + " Max: " + maxValue;
	}
}
