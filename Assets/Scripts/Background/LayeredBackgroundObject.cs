using UnityEngine;
using System.Collections;

public class LayeredBackgroundObject : MonoBehaviour {

	// Use this for initialization
	void Start() {
		if (transform.parent.GetComponent<BackgroundLayer>() != null) {
			GetComponent<DestructibleObject>().SetSpeed(transform.parent.GetComponent<BackgroundLayer>().GetSpeed());
		} else {
			Destroy(GetComponent<DestructibleObject>());
        }
		Destroy(this);
	}

}
