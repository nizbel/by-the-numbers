using UnityEngine;
using System.Collections;

public class LayeredBackgroundObject : MonoBehaviour {

	BackgroundLayer parentLayer;

	bool isDistantForegroundLayer;

	// Use this for initialization
	void Start() {
		parentLayer = transform.parent.GetComponent<BackgroundLayer>();
		if (!CheckIfStaticLayer()) {
			GetComponent<DestructibleObject>().SetSpeed(parentLayer.GetSpeed());

			isDistantForegroundLayer = !BackgroundStateController.controller.IsBackgroundLayer(parentLayer.transform);

			if (isDistantForegroundLayer) {
				// Set collision layer
				gameObject.layer = parentLayer.GetLayer();

				// Set sprite sorting layers
				if (GetComponent<MultipleSpriteObject>() != null) {
					SpriteRenderer[] sprites = gameObject.GetComponentsInChildren<SpriteRenderer>();
					foreach (SpriteRenderer sprite in sprites) {
						sprite.sortingLayerName = parentLayer.GetSpriteLayerName();
                    }
                } else {
					GetComponent<SpriteRenderer>().sortingLayerName = parentLayer.GetSpriteLayerName();
				}
				// If there are particle systems, apply rendering layer for them too
				ParticleSystem[] particleSystems = gameObject.GetComponentsInChildren<ParticleSystem>();
				foreach (ParticleSystem ps in particleSystems) {
					ps.GetComponent<Renderer>().sortingLayerName = parentLayer.GetSpriteLayerName();
				}

				// If object has random size script and is of the start varying type, multiply random scale too
				RandomSize randomSizeScript = GetComponent<RandomSize>();
				if (randomSizeScript != null) {
					randomSizeScript.MultiplyScales(1f / parentLayer.GetDistance());
                } else {
					transform.localScale /= parentLayer.GetDistance();
				}
			}
        } else {
			Destroy(GetComponent<DestructibleObject>());
			Destroy(this);
		}

    }

    private void OnDisable() {
		if (parentLayer != null) {
			if (isDistantForegroundLayer) {
				transform.localScale *= parentLayer.GetDistance();
			}
			Destroy(this);
		}
    }

    public bool CheckIfStaticLayer() {
		return parentLayer == null;
	}
}
