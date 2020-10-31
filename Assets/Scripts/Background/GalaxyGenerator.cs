using UnityEngine;
using System.Collections;

public class GalaxyGenerator : BackgroundElementGenerator {

	public const float MIN_GALAXY_GENERATION_PERIOD = 0.5f;
	public const float MAX_GALAXY_GENERATION_PERIOD = 12.5f;
	public const float MIN_GALAXY_SCALE = 0.1f;
	public const float MAX_GALAXY_SCALE = 0.95f;
	public const int MAX_GALAXY_AMOUNT = 5;

	// Use this for initialization
	void Start () {
		// Set values
		minGenerationPeriod = MIN_GALAXY_GENERATION_PERIOD;
		maxGenerationPeriod = MAX_GALAXY_GENERATION_PERIOD;
		minElementScale = MIN_GALAXY_SCALE;
		maxElementScale = MAX_GALAXY_SCALE;

		DefineNextGeneration();
		DefineMaxAmount(MAX_GALAXY_AMOUNT);
    }
	
	// Update is called once per frame
	void Update () {
		if (amountAlive < maxAmount) {
			if (Time.timeSinceLevelLoad - lastGeneratedTime > nextGeneration) {
				// Choose prefab
				int i = Random.Range(0, prefabs.Length);

				Vector3 objectPosition = GenerateRandomPosition();
				float objectScale = GenerateRandomScale();

				GenerateNewObject(prefabs[i], objectPosition, objectScale);

				// Update generation variables
				lastGeneratedTime = Time.timeSinceLevelLoad;
				DefineNextGeneration();
			}
		}

    }
}
