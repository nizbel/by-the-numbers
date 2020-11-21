using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering.Universal;

public class BackgroundStateController : MonoBehaviour {

    // Controls the background layers that moves objects in the background
    protected List<GameObject> backgroundLayers = new List<GameObject>();

	private Light2D globalLight = null;

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

			// Set background lighting
			globalLight = GameObject.Find("Background Light").GetComponent<Light2D>();
			globalLight.intensity = 0;
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

	public GameObject GetFastestBackgroundLayer() {
		return backgroundLayers[backgroundLayers.Count - 1];
    }

	public void IncreaseLight(float intensity) {
		globalLight.intensity += intensity;
	}

	public void DecreaseLight(float intensity) {
		globalLight.intensity -= intensity;
	}
}
