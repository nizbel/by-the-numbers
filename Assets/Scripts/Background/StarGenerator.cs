using UnityEngine;
using System.Collections;

public class StarGenerator : Generator {

	// Use this for initialization
	void Start () {
		nextGeneration = Random.Range(0.5f, 3.5f);
		for (float startingStarPosition = transform.position.x - 10; startingStarPosition < transform.position.x + 13.75f;) {
			int i = Random.Range(0, prefabs.Length);
			Vector3 objectPosition = new Vector3(startingStarPosition, transform.position.y + Random.Range(-3.2f, 3.2f), 0);
			float objectScale = Random.Range(0.01f, 0.15f);
			GameObject newObject = (GameObject) Instantiate(prefabs[i], objectPosition, Quaternion.Euler(0, 0, Random.Range(0, 180)));
			newObject.transform.localScale = new Vector3(objectScale, objectScale, objectScale);
			
			newObject.transform.parent = transform;
			
			increaseAmountAlive();

			startingStarPosition += Random.Range(-0.05f, 0.55f);
			
			// Update last generation variable
			lastGeneratedTime = Time.timeSinceLevelLoad;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.timeSinceLevelLoad - lastGeneratedTime > nextGeneration) {
			int i = Random.Range(0, prefabs.Length);
			Vector3 objectPosition = new Vector3(transform.position.x + Random.Range(12, 15.5f),
			                                     transform.position.y + Random.Range(-3.2f, 3.2f), 0);
			float objectScale = Random.Range(0.01f, 0.15f);
			GameObject newObject = (GameObject) Instantiate(prefabs[i], objectPosition, Quaternion.Euler(0, 0, Random.Range(0, 180)));
			newObject.transform.localScale = new Vector3(objectScale, objectScale, objectScale);

			newObject.transform.parent = transform;

			increaseAmountAlive();

			// Update generation variables
			lastGeneratedTime = Time.timeSinceLevelLoad;
			nextGeneration = Random.Range(0.01f, 0.2f);
		}

	}
}
