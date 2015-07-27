using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OutScreenDestroyerController : MonoBehaviour {
	
	List<GameObject> destructibleObjectsList;
	
	int currentObjectIndex = 0;
	
	public static OutScreenDestroyerController controller;
	
	int randomTries = 1;
	
	int randomDestroys = 0;
	
	float averageDestroyTries = 0;
	
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
	}
	
	// Update is called once per frame
	void Update () {
		/*
		 * Pick one object at a serial order
		 */
		// Test if current index can be picked
		if (currentObjectIndex < Mathf.FloorToInt(destructibleObjectsList.Count*0.85f)) {
			GameObject curDestructible = destructibleObjectsList[currentObjectIndex];
			//			Debug.Log("index: " + currentObjectIndex + " size: " + destructibleObjectsList.Count);
			
			if (curDestructible.GetComponent<SpriteRenderer>().sprite.bounds.extents.x + curDestructible.transform.position.x
			    < Camera.main.ScreenToWorldPoint(Vector3.zero).x) {
//				destructibleObjectsList.Remove(curDestructible);
//				Destroy(curDestructible);
				averageDestroyTries = ((averageDestroyTries * randomDestroys) + randomTries) / (randomDestroys+1);
				Debug.Log("Destroyed " + currentObjectIndex + " of " + destructibleObjectsList.Count + " with " + randomTries + " tries, average is " + averageDestroyTries + " tries");
				randomTries = 1;
				randomDestroys++;
				//				Debug.Log("Destroyed");
			} else {
				currentObjectIndex++;
				randomTries++;
			}
		}
		// If can't, return to starting index
		else {
			if (Random.Range(0, 100) < 10) {
				currentObjectIndex = 0;
			} else {
				currentObjectIndex = Mathf.FloorToInt(destructibleObjectsList.Count*0.15f);
			}
		}
		
		/*
		 * Pick one object at random
		 */
		if (destructibleObjectsList.Count > 0) {
			int randIndex = Random.Range(0, destructibleObjectsList.Count);
			GameObject curDestructible = destructibleObjectsList[randIndex];
			//			Debug.Log("At index " + randIndex);
			
			if (curDestructible.GetComponent<SpriteRenderer>().sprite.bounds.extents.x + curDestructible.transform.position.x
			    < Camera.main.ScreenToWorldPoint(Vector3.zero).x) {
				destructibleObjectsList.Remove(curDestructible);
				Destroy(curDestructible);
			}
		}
	}
	
	public void addToDestructibleList(GameObject gameObject) {
		destructibleObjectsList.Add(gameObject);
	}
}
