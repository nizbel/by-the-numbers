using UnityEngine;
using System.Collections;

public class DestructibleObject : MonoBehaviour {

	// Use this for initialization
	void Start () {
		OutScreenDestroyerController.controller.addToDestructibleList(this.gameObject);
	}
}
