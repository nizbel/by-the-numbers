using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering.Universal;

public class BackgroundStateController : MonoBehaviour {
	[SerializeField]
	private Light2D globalLight;

	[Header("Layers")]
	[Tooltip("Controls the background layers that moves objects in the background")]
	[SerializeField]
	protected List<Transform> backgroundLayers = new List<Transform>();

	[Tooltip("Constrols the layers with foreground elements in the background")]
	[SerializeField]
	protected List<Transform> distantForegroundLayers = new List<Transform>();

	[Header("Generators")]
	[SerializeField]
	StarGenerator starGenerator;

    [SerializeField]
    GalaxyGenerator galaxyGenerator;

    [SerializeField]
    BackgroundDebrisGenerator backgroundDebrisGenerator;

	[SerializeField]
	NebulaGenerator nebulaGenerator;

	[SerializeField]
	DistantForegroundGenerator distantForegroundGenerator;

	public static BackgroundStateController controller;

	void Awake() {
		if (controller == null) {
			controller = this;
			
			globalLight.intensity = 0;
		}
		else {
			Destroy(gameObject);
		}
	}

    public void PrepareConstellationSpawn() {
		GetComponent<ConstellationController>().enabled = true;
	}

	/*
	 * Background layer methods
	 */
	// TODO Check if should add layers later
	public void AddBackgroundLayer(Transform layer) {
		backgroundLayers.Add(layer);
	}

	public Transform GetRandomBackgroundLayer() {
		return backgroundLayers[Random.Range(0, backgroundLayers.Count)];
	}

	public Transform GetStaticBackgroundLayer() {
		return backgroundLayers[0];
    }

    public Transform GetRandomMovingBackgroundLayer() {
        return backgroundLayers[Random.Range(1, backgroundLayers.Count)];
    }

	public Transform GetFastestBackgroundLayer() {
		return backgroundLayers[backgroundLayers.Count - 1];
    }

	public Transform GetSlowestBackgroundLayer() {
		return backgroundLayers[1];
	}

	public bool IsBackgroundLayer(Transform layerTransform) {
		return backgroundLayers.Contains(layerTransform);
    }

	/*
	 * Distant foreground layer methods
	 */
	public Transform GetRandomDistantForegroundLayer() {
		return distantForegroundLayers[Random.Range(0, distantForegroundLayers.Count)];
	}

	public Transform GetFastestDistantForegroundLayer() {
		return distantForegroundLayers[distantForegroundLayers.Count - 1];
	}

	public Transform GetSlowestDistantForegroundLayer() {
		return distantForegroundLayers[0];
	}

	/*
	 * Background light methods
	 */
	public void IncreaseLight(float intensity) {
		globalLight.intensity += intensity;
		PlayerController.controller.SetLightIntensity(Mathf.Max(1 - globalLight.intensity, 0));
	}

	public void DecreaseLight(float intensity) {
		globalLight.intensity -= intensity;
		PlayerController.controller.SetLightIntensity(Mathf.Max(1 - globalLight.intensity, 0));
	}

	public StarGenerator GetStarGenerator() {
		return starGenerator;
	}

	/*
	 * Generator methods
	 */
	public void StopAllGeneration() {
		// TODO Add other generators
		distantForegroundGenerator.StopGenerating();
    }

	// Method to enable generators that depend on day data being available
	public void EnableGeneratorsWithDayDataAvailable() {
		distantForegroundGenerator.enabled = true;
	}

	public void UpdateDistantForegroundGenerator(bool shouldSpawn, List<ElementSpawnChance> elements) {
		if (shouldSpawn) {
			if (!distantForegroundGenerator.IsGenerating()) {
				distantForegroundGenerator.ResumeGenerating();
			}
			ChangeAvailableElementsDistantForegroundGenerator(elements);
        } else if (distantForegroundGenerator.IsGenerating()) {
			distantForegroundGenerator.StopGenerating();
		}
    }

	public void ChangeAvailableElementsDistantForegroundGenerator(List<ElementSpawnChance> elements) {
		List<ElementsEnum> availableElements = new List<ElementsEnum>();

		// Translate foreground elements to background elements
		// TODO add other elements
		foreach (ElementSpawnChance elementSpawnChance in elements) {
			switch (elementSpawnChance.element) {
				case ElementsEnum.POSITIVE_ENERGY:
					availableElements.Add(ElementsEnum.DF_POSITIVE_ENERGY);
					break;
				case ElementsEnum.NEGATIVE_ENERGY:
					availableElements.Add(ElementsEnum.DF_NEGATIVE_ENERGY);
					break;
				case ElementsEnum.DEBRIS:
					availableElements.Add(ElementsEnum.DF_DEBRIS);
					break;
			}

        }
		distantForegroundGenerator.SetAvailableElements(availableElements);
    }
}
