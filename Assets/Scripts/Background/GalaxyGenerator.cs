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
		elementType = ObjectPool.GALAXY;

		// Set values
		minGenerationPeriod = MIN_GALAXY_GENERATION_PERIOD;
		maxGenerationPeriod = MAX_GALAXY_GENERATION_PERIOD;
		minElementScale = MIN_GALAXY_SCALE;
		maxElementScale = MAX_GALAXY_SCALE;

		DefineNextGeneration();
		DefineMaxAmount(MAX_GALAXY_AMOUNT);

		StartCoroutine(SpawnMovingGalaxies());
	}

	IEnumerator SpawnMovingGalaxies() {
		while (StageController.controller.GetState() != StageController.ENDING_STATE) {
			yield return new WaitForSeconds(nextGeneration);
			if (amountAlive < maxAmount) {
				Vector3 objectPosition = GenerateRandomPosition();
				float objectScale = GenerateRandomScale();

				GenerateNewObject(objectPosition, objectScale);
			}

			// Update generation variables
			DefineNextGeneration();
		}
	}
}
