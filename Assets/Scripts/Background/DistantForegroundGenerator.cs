using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DistantForegroundGenerator : BackgroundElementGenerator {

	public const float MIN_GENERATION_PERIOD = 0.1f;
	public const float MAX_GENERATION_PERIOD = 0.3f;
	public const int MIN_AMOUNT = 10;
	public const int MAX_AMOUNT = 20;
	public const float MOVING_CHANCE = 20;

	private List<ElementsEnum> availableElements = new List<ElementsEnum>();

	private Coroutine generationCoroutine = null;

	// Use this for initialization
	void Start() {
		// Set values
		minGenerationPeriod = MIN_GENERATION_PERIOD;
		maxGenerationPeriod = MAX_GENERATION_PERIOD;

		DefineMaxAmount(MAX_AMOUNT, MIN_AMOUNT);

		if (StageController.controller.GetCurrentMomentDistantForegroundSpawn()) {
			DefineNextGeneration();

			if (maxAmount > 0) {
				while (amountAlive < maxAmount) {
					Vector3 objectPosition = new Vector3(Random.Range(GameController.GetCameraXMin(), GameController.GetCameraXMax()),
						Random.Range(GameController.GetCameraYMin(), GameController.GetCameraYMax()), 0);
					DefineElementType();

					GameObject newBackgroundEnergy = GenerateNewObject(objectPosition, 0, BackgroundLayerEnum.RandomDistantForegroundLayer, false);

					if (GameController.RollChance(MOVING_CHANCE)) {
						MovingObject movingScript = newBackgroundEnergy.AddComponent<MovingObject>();
						movingScript.Speed = new Vector2(Random.Range(MovingObject.MIN_FOREGROUND_ELEMENT_SPEED_X, MovingObject.MAX_FOREGROUND_ELEMENT_SPEED_X), -newBackgroundEnergy.transform.position.y);
					}
				}
			}
		}
	}

	IEnumerator SpawnDistantForegroundElements() {
		while (true) {
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

                if (newPosition.x < GameController.GetCameraXMax() || GameController.RollChance(MOVING_CHANCE)) {
                    MovingObject movingScript = newBackgroundEnergy.AddComponent<MovingObject>();
					movingScript.Speed = new Vector2(Random.Range(MovingObject.MIN_FOREGROUND_ELEMENT_SPEED_X, MovingObject.MAX_FOREGROUND_ELEMENT_SPEED_X * PlayerController.controller.GetSpeed()), -newBackgroundEnergy.transform.position.y * Random.Range(0.5f, 1));
				}
            }

            // Update generation variables
            DefineNextGeneration();
		}
	}

	// Define which element will be
	private void DefineElementType() {
		elementType = availableElements[Random.Range(0, availableElements.Count)];
    }

    protected override void PositionObjectOffScreen(GameObject gameObject) {
		SpriteRenderer spriteRenderer;

		// Check if multiple sprite object
		MultipleSpriteObject multiSprite = gameObject.GetComponent<MultipleSpriteObject>();

		if (multiSprite != null) {
			spriteRenderer = gameObject.GetComponent<MultipleSpriteObject>().GetBiggestSpriteRenderer();
		} else {
			spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		}

		// Get the biggest side of the sprite, then scales
		float maxDimension = Mathf.Max(spriteRenderer.sprite.bounds.extents.x,
			spriteRenderer.sprite.bounds.extents.y) * gameObject.transform.localScale.x;
		gameObject.transform.position = new Vector3(gameObject.transform.position.x + maxDimension,
					gameObject.transform.position.y, 0);
	}

	public void StopGenerating() {
		StopCoroutine(generationCoroutine);
		generationCoroutine = null;
	}

	public void ResumeGenerating() {
		DefineNextGeneration();
		generationCoroutine = StartCoroutine(SpawnDistantForegroundElements());
	}

	public void SetAvailableElements(List<ElementsEnum> availableElements) {
		this.availableElements.Clear();
		this.availableElements.AddRange(availableElements);
    }

	public bool IsGenerating() {
		return generationCoroutine != null;
    }
}
