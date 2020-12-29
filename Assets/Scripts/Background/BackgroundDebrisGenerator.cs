using UnityEngine;
using System.Collections;

public class BackgroundDebrisGenerator : BackgroundElementGenerator {

	public const float MIN_DEBRIS_GENERATION_PERIOD = 0.5f;
	public const float MAX_DEBRIS_GENERATION_PERIOD = 1.5f;
	public const float MIN_DEBRIS_SCALE = 0.1f;
	public const float MAX_DEBRIS_SCALE = 0.9f;
	public const int MAX_DEBRIS_AMOUNT = 20;

	public AnimationCurve minDebrisByDay;
	public AnimationCurve maxDebrisByDay;

	// Use this for initialization
	void Start () {
		elementType = ObjectPool.BG_DEBRIS;

		// Set values
		minGenerationPeriod = MIN_DEBRIS_GENERATION_PERIOD;
		maxGenerationPeriod = MAX_DEBRIS_GENERATION_PERIOD;
		minElementScale = MIN_DEBRIS_SCALE;
		maxElementScale = MAX_DEBRIS_SCALE;

		DefineNextGeneration();

		(int, int) minMaxDebrisAmount = CalculateDebrisAmountByDay(GameController.controller.GetCurrentDay());
		Debug.Log(minMaxDebrisAmount);
		DefineMaxAmount(minMaxDebrisAmount.Item2, minMaxDebrisAmount.Item1);

		// TODO Find a way to generate debris and galaxies at start, but debris will depend on a curve that starts at 0 (day 1), raises and ends at 0 (day 90)
		while (GameController.RollChance(CalculateGeneratingChance())) {
			//int i = Random.Range(0, prefabs.Length);

			Vector3 objectPosition = new Vector3(Random.Range(GameController.GetCameraXMin(), GameController.GetCameraXMax()),
				Random.Range(GameController.GetCameraYMin(), GameController.GetCameraYMax()), 0);
			float objectScale = GenerateRandomScale();
			//GameObject newObject = (GameObject)Instantiate(prefabs[i], objectPosition, Quaternion.Euler(0, 0, Random.Range(0, 180)));
			GameObject newObject = ObjectPool.SharedInstance.SpawnPooledObject(elementType, objectPosition, Quaternion.Euler(0, 0, Random.Range(0, 180)));
			newObject.transform.localScale = new Vector3(objectScale, objectScale, objectScale);

			newObject.transform.parent = BackgroundStateController.controller.GetFastestBackgroundLayer().transform;
			LayeredBackgroundObject layerScript = newObject.AddComponent<LayeredBackgroundObject>();

			// Set this as its generator
			newObject.GetComponent<GeneratedDestructible>().setGenerator(this);
			IncreaseAmountAlive();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.timeSinceLevelLoad - lastGeneratedTime > nextGeneration) {
			if (GameController.RollChance(CalculateGeneratingChance())) {
				// Choose prefab
				//int i = Random.Range(0, prefabs.Length);

				Vector3 objectPosition = GenerateRandomPosition();
				float objectScale = GenerateRandomScale();

				GenerateNewObject(objectPosition, objectScale, 3);
			}
			// Update generation variables
			lastGeneratedTime = Time.timeSinceLevelLoad;
			DefineNextGeneration();
		}
	}

	private float CalculateGeneratingChance() {
		if (maxAmount > 0) {
			return 100 - (amountAlive) * (100 / maxAmount);
		}
		return 0;
	}

	// Returns (min, max) int tuple
	private (int, int) CalculateDebrisAmountByDay(int currentDay) {
		(int, int) debrisByDay = ((int) (minDebrisByDay.Evaluate((float)currentDay / 90) * MAX_DEBRIS_AMOUNT), 
			(int) (maxDebrisByDay.Evaluate((float)currentDay / 90) * MAX_DEBRIS_AMOUNT));
		return debrisByDay;
    }
}
