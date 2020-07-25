using UnityEngine;
using System.Collections;

public class StarGenerator : Generator {

	public const float MIN_STAR_SCALE = 0.03f;
	public const float MAX_STAR_SCALE = 0.12f;

	// Use this for initialization
	void Start () {
		nextGeneration = DefineNextGeneration();
		for (float startingStarPosition = GameController.GetCameraXMin() - 1; startingStarPosition < GameController.GetCameraXMax() + 1;) {
			int i = Random.Range(0, prefabs.Length);
			Vector3 objectPosition = new Vector3(startingStarPosition, transform.position.y + Random.Range(-3.2f, 3.2f), 0);
			float objectScale = DefineNewStarScale();
			GameObject newObject = (GameObject) Instantiate(prefabs[i], objectPosition, Quaternion.Euler(0, 0, Random.Range(0, 180)));
			newObject.transform.localScale = new Vector3(objectScale, objectScale, objectScale);
			
			newObject.transform.parent = transform;
			
			increaseAmountAlive();

			startingStarPosition += Random.Range(-0.1f, 0.5f);
			
			// Update last generation variable
			lastGeneratedTime = Time.timeSinceLevelLoad;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.timeSinceLevelLoad - lastGeneratedTime > nextGeneration) {
			int i = Random.Range(0, prefabs.Length);
			Vector3 objectPosition = new Vector3(GameController.GetCameraXMax() + 1,
			                                     Random.Range(GameController.GetCameraYMin(), GameController.GetCameraYMax()), 0);
			float objectScale = DefineNewStarScale();
			GameObject newObject = (GameObject) Instantiate(prefabs[i], objectPosition, Quaternion.Euler(0, 0, Random.Range(0, 180)));
			newObject.transform.localScale = new Vector3(objectScale, objectScale, objectScale);

			newObject.transform.parent = transform;

			increaseAmountAlive();

			// Update generation variables
			lastGeneratedTime = Time.timeSinceLevelLoad;
			nextGeneration = DefineNextGeneration();
		}

	}

	private float DefineNextGeneration() {
		return Random.Range(0.2f, 0.5f);

	}

	private float DefineNewStarScale() {
		float averageStarScale = (MIN_STAR_SCALE + MAX_STAR_SCALE) / 2;
		return Random.Range(0, averageStarScale - MIN_STAR_SCALE) + Random.Range(0, averageStarScale - MIN_STAR_SCALE) + MIN_STAR_SCALE;

		//return Random.Range(MIN_STAR_SCALE, MAX_STAR_SCALE);
	}
}
