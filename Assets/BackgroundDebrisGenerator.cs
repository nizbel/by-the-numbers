using UnityEngine;
using System.Collections;

public class BackgroundDebrisGenerator : Generator {

	// Use this for initialization
	void Start () {
		nextGeneration = Random.Range(0.5f, 1.5f);
		maxAmount = Random.Range(0,50);
	}
	
	// Update is called once per frame
	void Update () {
		if (amountAlive < maxAmount) {
			if (Time.timeSinceLevelLoad - lastGeneratedTime > nextGeneration) {
				int i = Random.Range(0, prefabs.Length);
				Vector3 objectPosition = new Vector3(transform.position.x + Random.Range(15, 25.5f),
				                                     transform.position.y + Random.Range(-3.2f, 3.2f), 0);
				float objectScale = Random.Range(0.1f, 1);
				GameObject newObject = (GameObject) Instantiate(prefabs[i], objectPosition, Quaternion.Euler(0, 0, Random.Range(0, 180)));
				newObject.transform.localScale = new Vector3(objectScale, objectScale, objectScale);
				
				newObject.transform.parent = transform;

				// Set this as its generator
				newObject.GetComponent<GeneratedDestructible>().setGenerator(this);
				
				increaseAmountAlive();
				
				// Update generation variables
				lastGeneratedTime = Time.timeSinceLevelLoad;
				nextGeneration = Random.Range(0.1f, 1.5f);
			}
		}
		
		else if (Random.Range(0,100) >= 95) { 
			maxAmount = Random.Range(0,50);
		}
		
	}
}
