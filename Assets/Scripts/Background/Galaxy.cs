using UnityEngine;
using System.Collections;

public class Galaxy : MovingBackgroundElement {

	// Use this for initialization
	void Start () {
		SetSpeed(Mathf.Pow(transform.localScale.x, 2));
	}
	
	// Update is called once per frame
	//void Update () {
	//	Move();
	//}

	/*
	 * Getters and Setters
	 */
}
