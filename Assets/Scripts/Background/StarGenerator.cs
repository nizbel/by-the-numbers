using UnityEngine;
using System.Collections;

public class StarGenerator : BackgroundElementGenerator {

	public const float MIN_STAR_GENERATION_PERIOD = 0.2f;
	public const float MAX_STAR_GENERATION_PERIOD = 0.5f;
	public const float MIN_STAR_SCALE = 0.03f;
	public const float MAX_STAR_SCALE = 0.12f;

	// Use this for initialization
	void Start () {
		// Set values
		minGenerationPeriod = MIN_STAR_GENERATION_PERIOD;
		maxGenerationPeriod = MAX_STAR_GENERATION_PERIOD;
		minElementScale = MIN_STAR_SCALE;
		maxElementScale = MAX_STAR_SCALE;

		DefineNextGeneration();
		for (float startingStarPosition = GameController.GetCameraXMin() - 1; startingStarPosition < GameController.GetCameraXMax() + 1;) {
			int i = Random.Range(0, prefabs.Length);
			Vector3 objectPosition = new Vector3(startingStarPosition, 
				Random.Range(GameController.GetCameraYMin(), GameController.GetCameraYMax()), 0);
			float objectScale = GenerateNormalDistributionScale();
			GameObject newObject = (GameObject) Instantiate(prefabs[i], objectPosition, Quaternion.Euler(0, 0, Random.Range(0, 180)));
			newObject.transform.localScale = new Vector3(objectScale, objectScale, objectScale);
			
			newObject.transform.parent = transform;

			// Set this as its generator
			newObject.GetComponent<GeneratedDestructible>().setGenerator(this);
			IncreaseAmountAlive();

			startingStarPosition += Random.Range(-0.1f, 0.5f);
		}
		// Update last generation variable
		lastGeneratedTime = Time.timeSinceLevelLoad;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.timeSinceLevelLoad - lastGeneratedTime > nextGeneration) {
			// Choose prefab
			int i = Random.Range(0, prefabs.Length);

			Vector3 objectPosition = GenerateRandomPosition();
			float objectScale = GenerateNormalDistributionScale();

			GenerateNewObject(prefabs[i], objectPosition, objectScale);

			// Update generation variables
			lastGeneratedTime = Time.timeSinceLevelLoad;
			DefineNextGeneration();
		}

	}

}
