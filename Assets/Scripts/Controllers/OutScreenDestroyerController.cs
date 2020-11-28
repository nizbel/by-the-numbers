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
		Debug.Log("Amount: " + Mathf.FloorToInt(destructibleObjectsList.Count * currentLimit));
		if (Mathf.FloorToInt(destructibleObjectsList.Count * currentLimit) == 62) {
			Debug.Break();
        }
		float inicio = Time.realtimeSinceStartup;
		while (true) {
			if (currentObjectIndex < Mathf.FloorToInt(destructibleObjectsList.Count * currentLimit)) {
                GameObject curDestructible = destructibleObjectsList[currentObjectIndex];
				//			Debug.Log("index: " + currentObjectIndex + " size: " + destructibleObjectsList.Count);

				if (ObjectCrossedCameraXBound(curDestructible) && CanDestroyNow(curDestructible)) {
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
		Debug.Log("Elapsed: " + (Time.realtimeSinceStartup - inicio));

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

	public bool ObjectCrossedCameraXBound(GameObject destructible) {
		DestructibleObject destructibleObjScript = destructible.GetComponent<DestructibleObject>();

		switch (destructibleObjScript.GetDestructibleType()) {
			case DestructibleObject.COMMON_SPRITE_TYPE:
				// Get bigger side of the sprite, the scale is always constant between x, y and z
				/*
				 * Teste 1
				 */
				float maxSideSprite = GameObjectUtil.GetBiggestSideOfSprite(destructibleObjScript.GetSpriteRenderer().sprite);
				// End teste 1

				/*
				 * Teste 2
				 */
				//float maxSide = Mathf.Max(destructible.GetComponent<SpriteRenderer>().sprite.bounds.extents.x,
				//    destructible.GetComponent<SpriteRenderer>().sprite.bounds.extents.y);
				// End teste 2

				/*
				 * Teste 3
				 */
				//Sprite sprite = destructible.GetComponent<SpriteRenderer>().sprite;
				//float maxSide = Mathf.Max(sprite.bounds.extents.x, sprite.bounds.extents.y);
				// End teste 3

				return maxSideSprite * destructible.transform.localScale.x
						+ destructible.transform.position.x
						< GameController.GetCameraXMin();

			case DestructibleObject.FORMATION_TYPE:
				return destructible.transform.childCount == 0;

			case DestructibleObject.MULTIPLE_SPRITE_TYPE:

				float maxSideBiggestSprite = GameObjectUtil.GetBiggestSideOfSprite(destructibleObjScript.GetSpriteRenderer().sprite);

				return maxSideBiggestSprite * destructible.transform.localScale.x
						+ destructible.transform.position.x
						< GameController.GetCameraXMin();

			default:
				return false; 
		}
	}

	private bool CanDestroyNow(GameObject destructible) {
		return destructible.GetComponent<DestructibleObject>().IsDestructibleNow();
    }
}
