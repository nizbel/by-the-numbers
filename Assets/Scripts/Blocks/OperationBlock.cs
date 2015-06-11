using UnityEngine;
using System.Collections;

public class OperationBlock : MonoBehaviour {

	protected int value;
	
	// Update is called once per frame
	void Update () {
//		if (transform.position.x < Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x - 3) {
//			Destroy(this.gameObject);
//		}
	}

	public virtual int operation(int curValue) {
		return curValue;
	}

	/*
	 * Getters and Setters
	 */

	public void setValue(int value) {
		this.value = value;
	}

	public int getValue() {
		return value;
	}
}
