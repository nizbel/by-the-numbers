using UnityEngine;
using System.Collections;

public class BackgroundElementGenerator : MonoBehaviour {

	[SerializeField]
	protected int elementType;

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

	protected GameObject GenerateNewObject(Vector3 position, float scale, int layer=-1) {
        // Generate object and avoid showing on screen yet
        GameObject newObject = ObjectPool.SharedInstance.SpawnPooledObject(elementType, position, Quaternion.Euler(0, 0, Random.Range(0, 180)));
        newObject.transform.localScale = new Vector3(scale, scale, scale);
		PositionObjectOffScreen(newObject);

		// Add to background layer
		if (layer == -1) {
			newObject.transform.parent = BackgroundStateController.controller.GetRandomMovingBackgroundLayer().transform;
		} else {
			// TODO Make layer selectable instead of fastest layer
			newObject.transform.parent = BackgroundStateController.controller.GetFastestBackgroundLayer().transform;
		}
		newObject.AddComponent<LayeredBackgroundObject>();

		// Set this as its generator
		newObject.GetComponent<GeneratedDestructible>().setGenerator(this);

		IncreaseAmountAlive();

		return newObject;
	}

	protected void PositionObjectOffScreen(GameObject gameObject) {
		// Get the biggest side of the sprite, then scales
		float maxDimension = Mathf.Max(gameObject.GetComponent<SpriteRenderer>().sprite.bounds.extents.x, 
			gameObject.GetComponent<SpriteRenderer>().sprite.bounds.extents.y) 
			* gameObject.transform.localScale.x;
		gameObject.transform.position = new Vector3(gameObject.transform.position.x + maxDimension,
					gameObject.transform.position.y, 0);
	}

}
