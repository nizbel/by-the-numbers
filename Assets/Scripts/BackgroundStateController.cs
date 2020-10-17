using UnityEngine;
using System.Collections.Generic;

public class BackgroundStateController : MonoBehaviour {

    // Controls the background layers that moves objects in the background
    protected List<GameObject> backgroundLayers = new List<GameObject>();

    public static BackgroundStateController controller;

	void Awake() {
		if (controller == null) {
			controller = this;
			
			// Fill layers
			foreach (Transform child in transform) {
				if (child.name == "Layers") {
					foreach (Transform layer in child) {
						backgroundLayers.Add(layer.gameObject);
                    }
					break;
                }
            }
		}
		else {
			Destroy(gameObject);
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void AddBackgroundLayer(GameObject layer) {
		backgroundLayers.Add(layer);

	}

	public GameObject GetRandomBackgroundLayer() {
		return backgroundLayers[Random.Range(0, backgroundLayers.Count)];
	}

    public GameObject GetRandomMovingBackgroundLayer() {
        return backgroundLayers[Random.Range(1, backgroundLayers.Count)];
    }
}
