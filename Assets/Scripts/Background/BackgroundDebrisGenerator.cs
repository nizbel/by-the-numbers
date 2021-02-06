using UnityEngine;
using System.Collections;

public class BackgroundDebrisGenerator : BackgroundElementGenerator {

	public const float MIN_DEBRIS_GENERATION_PERIOD = 0.5f;
	public const float MAX_DEBRIS_GENERATION_PERIOD = 1.5f;
	public const int MAX_DEBRIS_AMOUNT = 20;
	public const float MOVING_DEBRIS_CHANCE = 20;

	public AnimationCurve minDebrisByDay;
	public AnimationCurve maxDebrisByDay;

	// Use this for initialization
	void Start () {
		elementType = ElementsEnum.BG_DEBRIS;

		// Set values
		minGenerationPeriod = MIN_DEBRIS_GENERATION_PERIOD;
		maxGenerationPeriod = MAX_DEBRIS_GENERATION_PERIOD;

		DefineNextGeneration();

		// Define max debris amount depending on day
		// TODO Apply the same for galaxies
		(int, int) minMaxDebrisAmount = CalculateDebrisAmountByDay(GameController.controller.GetCurrentDay());
		DefineMaxAmount(minMaxDebrisAmount.Item2, minMaxDebrisAmount.Item1);

		if (maxAmount > 0) {
			while (GameController.RollChance(CalculateGeneratingChance())) {
				Vector3 objectPosition = new Vector3(Random.Range(GameController.GetCameraXMin(), GameController.GetCameraXMax()),
					Random.Range(GameController.GetCameraYMin(), GameController.GetCameraYMax()), 0);

				GameObject newObject = GenerateNewObject(objectPosition, 0, BackgroundLayerEnum.RandomMovingBackgroundLayer, false);
			}	

			StartCoroutine(SpawnMovingDebris());
		}
	}

	IEnumerator SpawnMovingDebris() {
		while (StageController.controller.GetState() != StageController.ENDING_STATE) {
			yield return new WaitForSeconds(nextGeneration);
			if (GameController.RollChance(CalculateGeneratingChance())) {
				GameObject newObject = GenerateNewObject(GenerateRandomPosition(), 0, BackgroundLayerEnum.RandomMovingBackgroundLayer);
			}
			// Update generation variables
			DefineNextGeneration();
		}
	}

	private float CalculateGeneratingChance() {
		if (maxAmount > 0) {
			return 100f * (maxAmount - amountAlive);
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
