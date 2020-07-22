using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour {

	public int value;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

	public virtual void buttonAction() {

	}

	public int getValue() {
		return value;
	}

	public void setValue(int value) {
		this.value = value;
	}
}
