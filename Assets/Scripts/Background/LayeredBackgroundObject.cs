using UnityEngine;
using System.Collections;

public class LayeredBackgroundObject : MonoBehaviour {

	BackgroundLayer parentLayer;

	// Use this for initialization
	void Start() {
		parentLayer = transform.parent.GetComponent<BackgroundLayer>();
		if (!CheckIfStaticLayer()) {
			GetComponent<DestructibleObject>().SetSpeed(parentLayer.GetSpeed());
		} else {
			Destroy(GetComponent<DestructibleObject>());
        }
		Destroy(this);
	}

	public bool CheckIfStaticLayer() {
		return parentLayer == null;
	}
}
