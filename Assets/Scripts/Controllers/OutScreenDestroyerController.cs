using UnityEngine;
using System.Collections.Generic;

public class OutScreenDestroyerController : MonoBehaviour {

	public const float DEFAULT_SQR_DISTANCE_TO_DESTROY = 2500;

	List<DestructibleObject> destructibleObjectsList;
	
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

	float currentCameraBorder;

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
        destructibleObjectsList = new List<DestructibleObject>();
		currentLimit = Random.Range(0.5f, 1);
	}

	// Update is called once per frame
	void FixedUpdate () {
		/*
		 * Pick one object at a serial order
		 */
		// Test if current index can be picked
        currentCameraBorder = GameController.GetCameraXMin();
        while (true) {
            if (currentObjectIndex < Mathf.FloorToInt(destructibleObjectsList.Count * currentLimit)) {
                DestructibleObject curDestructible = destructibleObjectsList[currentObjectIndex];
				//			Debug.Log("index: " + currentObjectIndex + " size: " + destructibleObjectsList.Count);

				if (curDestructible.IsDestructibleNow() && ObjectCrossedCameraLimits(curDestructible)) {
					destructibleObjectsList.Remove(curDestructible);
                    //Destroy(curDestructible.gameObject);
                    curDestructible.OnObjectDespawn();
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
	
	public void AddToDestructibleList(DestructibleObject newDestructible) {
		// Check speeds to create order
		for (int index = 0; index < destructibleObjectsList.Count; index++) {
			DestructibleObject currentObj = destructibleObjectsList[index];
			if (currentObj.GetSpeed() < newDestructible.GetSpeed()) {
				destructibleObjectsList.Insert(index, newDestructible);
				return;
            }
        }
        destructibleObjectsList.Add(newDestructible);
    }

	public bool ObjectCrossedCameraLimits(DestructibleObject destructible) { 
		switch (destructible.GetDestructibleType()) {
			case DestructibleObject.COMMON_SPRITE_TYPE:
			case DestructibleObject.MULTIPLE_SPRITE_TYPE:
				// Get bigger side of the sprite, the scale is always constant between x, y and z
				float maxSideSprite = GameObjectUtil.GetBiggestSideOfSprite(destructible.GetSpriteRenderer().sprite);

                return maxSideSprite * destructible.transform.localScale.x
                        + destructible.transform.position.x
                        < currentCameraBorder || destructible.transform.position.sqrMagnitude > DEFAULT_SQR_DISTANCE_TO_DESTROY;

            case DestructibleObject.FORMATION_TYPE:
				return destructible.transform.childCount == 0;

			default:
				return false; 
		}
	}
}
