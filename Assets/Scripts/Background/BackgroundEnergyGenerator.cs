using UnityEngine;
using System.Collections;

public class BackgroundEnergyGenerator : BackgroundElementGenerator {

	public const float MIN_ENERGY_GENERATION_PERIOD = 0.1f;
	public const float MAX_ENERGY_GENERATION_PERIOD = 0.3f;
	public const float MIN_ENERGY_SCALE = 0.75f;
	public const float MAX_ENERGY_SCALE = 1f;
	public const int MIN_ENERGY_AMOUNT = 10;
	public const int MAX_ENERGY_AMOUNT = 20;
	public const float MOVING_ENERGY_CHANCE = 20;

	// Use this for initialization
	void Start() {
		// Set values
		minGenerationPeriod = MIN_ENERGY_GENERATION_PERIOD;
		maxGenerationPeriod = MAX_ENERGY_GENERATION_PERIOD;
		minElementScale = MIN_ENERGY_SCALE;
		maxElementScale = MAX_ENERGY_SCALE;

		DefineNextGeneration();
		DefineMaxAmount(MAX_ENERGY_AMOUNT, MIN_ENERGY_AMOUNT);

		if (maxAmount > 0) {
			while (amountAlive < maxAmount) {
				Vector3 objectPosition = new Vector3(Random.Range(GameController.GetCameraXMin(), GameController.GetCameraXMax()),
					Random.Range(GameController.GetCameraYMin(), GameController.GetCameraYMax()), 0);
				DefineElementType();

				GameObject newBackgroundEnergy = GenerateNewObject(objectPosition, 0, BackgroundLayerEnum.RandomDistantForegroundLayer, false);

				if (GameController.RollChance(MOVING_ENERGY_CHANCE)) {
					MovingObject movingScript = newBackgroundEnergy.AddComponent<MovingObject>();
					movingScript.Speed = new Vector2(Random.Range(MovingObject.MIN_FOREGROUND_ELEMENT_SPEED_X, MovingObject.MAX_FOREGROUND_ELEMENT_SPEED_X), -newBackgroundEnergy.transform.position.y);
				}
			}

			StartCoroutine(SpawnBackgroundEnergies());
		}
	}

	IEnumerator SpawnBackgroundEnergies() {
		while (StageController.controller.GetState() != StageController.ENDING_STATE) {
			yield return new WaitForSeconds(nextGeneration);

			if (amountAlive < maxAmount) {
				DefineElementType();

				// Decide if should be coming vertically or normal right spawn
				Vector3 newPosition;
				if (GameController.RollChance(20)) { 
					float positionX = Random.Range(GameController.GetCameraXMin()+1, GameController.GetCameraXMax());
					float positionY = (GameController.RollChance(50) ? 1 : -1) * (GameController.GetCameraYMax() + 2);

					newPosition = new Vector3(positionX, positionY, 0);
				} else {
					newPosition = GenerateRandomPosition();
                }
                GameObject newBackgroundEnergy = GenerateNewObject(newPosition, 0, BackgroundLayerEnum.RandomDistantForegroundLayer);

                if (newPosition.x < GameController.GetCameraXMax() || GameController.RollChance(MOVING_ENERGY_CHANCE)) {
                    MovingObject movingScript = newBackgroundEnergy.AddComponent<MovingObject>();
					movingScript.Speed = new Vector2(Random.Range(MovingObject.MIN_FOREGROUND_ELEMENT_SPEED_X, MovingObject.MAX_FOREGROUND_ELEMENT_SPEED_X * PlayerController.controller.GetSpeed()), -newBackgroundEnergy.transform.position.y * Random.Range(0.5f, 1));
				}
            }

            // Update generation variables
            DefineNextGeneration();
		}
	}

	// Define if it is positive or negative
	private void DefineElementType() {
		elementType = GameController.RollChance(50) ? ObjectPool.BG_POSITIVE_ENERGY : ObjectPool.BG_NEGATIVE_ENERGY;
    }

    protected override void PositionObjectOffScreen(GameObject gameObject) {
		// Get the biggest side of the sprite, then scales
		SpriteRenderer spriteRenderer = gameObject.GetComponent<MultipleSpriteObject>().GetBiggestSpriteRenderer();
		float maxDimension = Mathf.Max(spriteRenderer.sprite.bounds.extents.x,
			spriteRenderer.sprite.bounds.extents.y) * gameObject.transform.localScale.x;
		gameObject.transform.position = new Vector3(gameObject.transform.position.x + maxDimension,
					gameObject.transform.position.y, 0);
	}
}
