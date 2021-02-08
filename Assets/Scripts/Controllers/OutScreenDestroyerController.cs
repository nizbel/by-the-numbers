using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class OutScreenDestroyerController : MonoBehaviour {

	private const float FAST_OBJECTS_WAIT_PERIOD = 0.1f;
	private const float SLOW_OBJECTS_WAIT_PERIOD = 0.4f;

	public const float DEFAULT_SQR_DISTANCE_TO_DESTROY = 2500;

	public const float AMOUNT_TO_CHECK = 0.75f;

	List<DestructibleObject> slowDestructibleObjectsList = new List<DestructibleObject>();
	List<DestructibleObject> fastDestructibleObjectsList = new List<DestructibleObject>();

	// Coroutines
	Coroutine slowObjectsCoroutine;
	Coroutine fastObjectsCoroutine;

	int currentObjectIndex = 0;
	
	public static OutScreenDestroyerController controller;

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
		// Start removal coroutines
		slowObjectsCoroutine = StartCoroutine(RemoveElementsFromGameplay(slowDestructibleObjectsList, SLOW_OBJECTS_WAIT_PERIOD));
		fastObjectsCoroutine = StartCoroutine(RemoveElementsFromGameplay(fastDestructibleObjectsList, FAST_OBJECTS_WAIT_PERIOD));
		currentCameraBorder = GameController.GetCameraXMin();
	}

	IEnumerator RemoveElementsFromGameplay(List<DestructibleObject> destructibleList, float timeToWait) {
		if (timeToWait == SLOW_OBJECTS_WAIT_PERIOD) {
			yield return new WaitForSeconds(FAST_OBJECTS_WAIT_PERIOD/2);
		}

		while (true) {
			yield return new WaitForSeconds(timeToWait);

			while (true) {
				// Test if current index can be picked
				if (currentObjectIndex < destructibleList.Count * AMOUNT_TO_CHECK) {
					DestructibleObject curDestructible = destructibleList[currentObjectIndex];

					if (curDestructible.IsDestructibleNow() && ObjectCrossedCameraLimits(curDestructible)) {
						destructibleList.Remove(curDestructible);
						curDestructible.OnObjectDespawn();
					}
					else {
						currentObjectIndex++;
					}
				}
				// If can't, return to starting index
				else {
					currentObjectIndex = 0;
					break;
				}
			}
		}
	}

	public void AddToDestructibleList(DestructibleObject newDestructible) {
		// If speed is higher or equal to player's speed, goes to fast
		if (newDestructible.GetSpeed() >= PlayerController.controller.GetSpeed()) {
			fastDestructibleObjectsList.Add(newDestructible);
		} else {
			slowDestructibleObjectsList.Add(newDestructible);
		}
    }

	public bool ObjectCrossedCameraLimits(DestructibleObject destructible) { 
		switch (destructible.GetDestructibleType()) {
			case DestructibleObject.COMMON_SPRITE_TYPE:
			case DestructibleObject.MULTIPLE_SPRITE_TYPE:
                if (destructible.transform.position.x < currentCameraBorder) {
                    // Get bigger side of the sprite, the scale is always constant between x, y and z
                    float maxSideSprite = GameObjectUtil.GetBiggestSideOfSprite(destructible.GetSpriteRenderer().sprite);

                    return destructible.transform.position.x + maxSideSprite * destructible.transform.localScale.x < currentCameraBorder;
                }
                else {
                    return destructible.transform.position.sqrMagnitude > DEFAULT_SQR_DISTANCE_TO_DESTROY;
                }

            case DestructibleObject.FORMATION_TYPE:
				return destructible.transform.childCount == 0;

			default:
				return false; 
		}
	}

    public void Stop() {
		// Stop coroutines
		StopCoroutine(slowObjectsCoroutine);
		StopCoroutine(fastObjectsCoroutine);

		enabled = false;
	}
}
