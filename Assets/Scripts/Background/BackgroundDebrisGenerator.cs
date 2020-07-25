using UnityEngine;
using System.Collections;

public class BackgroundDebrisGenerator : BackgroundElementGenerator {

	public const float MIN_DEBRIS_GENERATION_PERIOD = 0.5f;
	public const float MAX_DEBRIS_GENERATION_PERIOD = 1.5f;
	public const float MIN_DEBRIS_SCALE = 0.1f;
	public const float MAX_DEBRIS_SCALE = 0.9f;
	public const int MAX_DEBRIS_AMOUNT = 50;

	// Use this for initialization
	void Start () {// Set values
		minGenerationPeriod = MIN_DEBRIS_GENERATION_PERIOD;
		maxGenerationPeriod = MAX_DEBRIS_GENERATION_PERIOD;
		minElementScale = MIN_DEBRIS_SCALE;
		maxElementScale = MAX_DEBRIS_SCALE;

		DefineNextGeneration();
		maxAmount = Random.Range(0, MAX_DEBRIS_AMOUNT);
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
		
		else if (GameController.RollChance(5)) { 
			maxAmount = Random.Range(0, MAX_DEBRIS_AMOUNT);
		}
		
	}
}
