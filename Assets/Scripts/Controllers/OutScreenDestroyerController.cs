using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OutScreenDestroyerController : MonoBehaviour {
	
	List<GameObject> destructibleObjectsList;
	
	int currentObjectIndex = 0;
	
	public static OutScreenDestroyerController controller;

	[SerializeField]
	int randomTries = 1;

	[SerializeField]
	int randomDestroys = 0;

	[SerializeField]
	float averageDestroyTries = 0;

	[SerializeField]
	int objCount = 0;

	[SerializeField]
	float currentLimit;
	
	void Awake() {
		if (controller == null) {
			controller = this;
		}
		else {
			Destroy(gameObject);
		}
	}
	
	// Use this for initialization
	void Start () {
        destructibleObjectsList = new List<GameObject>();
		currentLimit = Random.Range(0.5f, 1);
	}
	
	// Update is called once per frame
	void Update () {
		/*
		 * Pick one object at a serial order
		 */
		// Test if current index can be picked
		while (true) {
			if (currentObjectIndex < Mathf.FloorToInt(destructibleObjectsList.Count * currentLimit)) {
                GameObject curDestructible = destructibleObjectsList[currentObjectIndex];
				//			Debug.Log("index: " + currentObjectIndex + " size: " + destructibleObjectsList.Count);

				if (ObjectCrossedCameraXBound(curDestructible)) {
					destructibleObjectsList.Remove(curDestructible);
					Destroy(curDestructible);
					averageDestroyTries = ((averageDestroyTries * randomDestroys) + randomTries) / (randomDestroys + 1);
					//				Debug.Log("Destroyed " + currentObjectIndex + " of " + destructibleObjectsList.Count + " with " + randomTries + " tries, average is " + averageDestroyTries + " tries");
					randomTries = 1;
					randomDestroys++;
				}
				else {
					currentObjectIndex++;
					randomTries++;
				}
			}
			// If can't, return to starting index
			else {
				currentObjectIndex = 0;

				// Set new limit
				currentLimit = Random.Range(0.25f, 1.1f);
				if (currentLimit > 1) {
					currentLimit = 1;
				}
                break;
            }
		}

		objCount = destructibleObjectsList.Count;
	}
	
	public void AddToDestructibleList(GameObject gameObject) {
		// Check speeds to create order
		for (int index = 0; index < destructibleObjectsList.Count; index++) {
            GameObject currentObj = destructibleObjectsList[index];
			if (currentObj.GetComponent<DestructibleObject>().GetSpeed() < gameObject.GetComponent<DestructibleObject>().GetSpeed()) {
				destructibleObjectsList.Insert(index, gameObject);
				return;
            }
        }
        destructibleObjectsList.Add(gameObject);
    }

	private bool ObjectCrossedCameraXBound(GameObject destructible) {
		if (destructible.GetComponent<SpriteRenderer>() != null) {
			return destructible.GetComponent<SpriteRenderer>().sprite.bounds.extents.x
					* Mathf.Max(destructible.transform.localScale.x, destructible.transform.localScale.y)
					+ destructible.transform.position.x
					< GameController.GetCameraXMin();
		} else if (destructible.GetComponent<Formation>() != null) {
			return destructible.transform.childCount == 0;
        }
		return false;
	}
}
