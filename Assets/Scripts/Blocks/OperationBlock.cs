using UnityEngine;
using System.Collections;

public class OperationBlock : MonoBehaviour {

	protected int value;
	
	// Update is called once per frame
	void Update () {
	}

	public virtual int Operation(int curValue) {
		return curValue;
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
