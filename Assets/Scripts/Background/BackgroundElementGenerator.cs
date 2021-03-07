using UnityEngine;
using System.Collections;

public class BackgroundElementGenerator : MonoBehaviour {

	protected ElementsEnum elementType;

	// Generation variables
	protected float nextGeneration;

	protected float minGenerationPeriod;

	protected float maxGenerationPeriod;

	protected float minElementScale;

	protected float maxElementScale;
	
	// Amount variables
	protected int amountAlive = 0;
	
	protected int maxAmount;

	public virtual void IncreaseAmountAlive() {
		amountAlive++;
	}
	
	public virtual void DecreaseAmountAlive() {
		amountAlive--;
	}


	protected void DefineNextGeneration() {
		nextGeneration = Random.Range(minGenerationPeriod, maxGenerationPeriod);
	}

	protected void DefineMaxAmount(int maxAmount, int minAmount=0) {
		this.maxAmount = Random.Range(minAmount, maxAmount+1);
	}

	protected float GenerateRandomScale() {
		return Random.Range(minElementScale, maxElementScale);
	}

	protected float GenerateNormalDistributionScale() {
		return DistributionUtil.GetNormalDistribution(minElementScale, maxElementScale);
	}

	protected Vector3 GenerateRandomPosition() {
		return new Vector3(GameController.GetCameraXMax() + 1,
			Random.Range(GameController.GetCameraYMin(), GameController.GetCameraYMax()), 0);
	}

	protected GameObject GenerateNewObject(Vector3 position, float scale, BackgroundLayerEnum layer = BackgroundLayerEnum.RandomMovingBackgroundLayer, bool shouldPositionOffScreen = true) {
        // Generate object and avoid showing on screen yet
        GameObject newObject = ObjectPool.SharedInstance.SpawnPooledObject(elementType, position, Quaternion.Euler(0, 0, Random.Range(0, 180)));
		if (scale != 0) {
			newObject.transform.localScale = new Vector3(scale, scale, scale);
		}

		if (shouldPositionOffScreen) {
			PositionObjectOffScreen(newObject);
		}

		// Add to background layer
		switch (layer) {
			case BackgroundLayerEnum.RandomBackgroundLayer:
				newObject.transform.parent = BackgroundStateController.controller.GetRandomBackgroundLayer();
				break;
			case BackgroundLayerEnum.RandomMovingBackgroundLayer:
				newObject.transform.parent = BackgroundStateController.controller.GetRandomMovingBackgroundLayer();
				break;
			case BackgroundLayerEnum.FastestBackgroundLayer:
				newObject.transform.parent = BackgroundStateController.controller.GetFastestBackgroundLayer();
				break;
			case BackgroundLayerEnum.SlowestDistantForegroundLayer:
				newObject.transform.parent = BackgroundStateController.controller.GetSlowestDistantForegroundLayer();
				break;
			case BackgroundLayerEnum.RandomDistantForegroundLayer:
				newObject.transform.parent = BackgroundStateController.controller.GetRandomDistantForegroundLayer();
				break;
		}
		
		newObject.AddComponent<LayeredBackgroundObject>();

		// Set this as its generator
		newObject.GetComponent<GeneratedDestructible>().setGenerator(this);

		IncreaseAmountAlive();

		return newObject;
	}

	protected virtual void PositionObjectOffScreen(GameObject gameObject) {
		// Get the biggest side of the sprite, then scales
		SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		float maxDimension = Mathf.Max(spriteRenderer.sprite.bounds.extents.x,
			spriteRenderer.sprite.bounds.extents.y) * gameObject.transform.localScale.x;
		gameObject.transform.position = new Vector3(gameObject.transform.position.x + maxDimension,
					gameObject.transform.position.y, 0);
	}

}
