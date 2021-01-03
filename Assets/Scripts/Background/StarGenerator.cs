using System.Collections;
using UnityEngine;

public class StarGenerator : BackgroundElementGenerator {

	public const float MIN_STAR_GENERATION_PERIOD = 0.2f;
	public const float MAX_STAR_GENERATION_PERIOD = 1.8f;
    public const float MIN_STAR_SCALE = 0.5f;
    public const float MAX_STAR_SCALE = 1f;

    public const int MAX_STAR_AMOUNT = 60;

    private const float BASE_CHANCE = 25;
    private const float BASE_CHANCE_MULTIPLIER = 0.475f;
    private const float CHANCE_DIFF_MULTIPLIER = 4.5f;

    public const float BASE_STAR_INTENSITY = 0.015f;

    [SerializeField]
    Sprite[] starSprites;

    // Use this for initialization
    void Start () {
        elementType = ObjectPool.STAR;

		// Set values
		minGenerationPeriod = MIN_STAR_GENERATION_PERIOD;
		maxGenerationPeriod = MAX_STAR_GENERATION_PERIOD;
		minElementScale = MIN_STAR_SCALE;
		maxElementScale = MAX_STAR_SCALE;

        maxAmount = Random.Range(30, MAX_STAR_AMOUNT + 1);

        DefineNextGeneration();

        while (GameController.RollChance(CalculateGeneratingChance())) {
            //int i = Random.Range(0, prefabs.Length);

            Vector3 objectPosition = new Vector3(Random.Range(GameController.GetCameraXMin(), GameController.GetCameraXMax()),
                Random.Range(GameController.GetCameraYMin(), GameController.GetCameraYMax()), 0);
            float objectScale = GenerateNormalDistributionScale();
            //GameObject newObject = (GameObject)Instantiate(prefabs[i], objectPosition, Quaternion.Euler(0, 0, Random.Range(0, 180)));
            GameObject newObject = ObjectPool.SharedInstance.SpawnPooledObject(elementType, objectPosition, Quaternion.Euler(0, 0, Random.Range(0, 180)));

            newObject.transform.localScale = new Vector3(objectScale, objectScale, objectScale);

            newObject.transform.parent = BackgroundStateController.controller.GetRandomBackgroundLayer().transform;
            LayeredBackgroundObject layerScript = newObject.AddComponent<LayeredBackgroundObject>();

            // Set sprite randomly
            newObject.GetComponent<SpriteRenderer>().sprite = starSprites[Random.Range(0, starSprites.Length)];

            // Set this as its generator
            newObject.GetComponent<GeneratedDestructible>().setGenerator(this);
            IncreaseAmountAlive();
        }

        StartCoroutine(SpawnMovingStars());
	}

    IEnumerator SpawnMovingStars() {
        while (StageController.controller.GetState() != StageController.ENDING_STATE) {
            yield return new WaitForSeconds(nextGeneration);
            if (StageController.controller.GetCurrentMomentType() != StageMoment.TYPE_CUTSCENE) {
                if (GameController.RollChance(CalculateGeneratingChance())) {
                    Vector3 objectPosition = GenerateRandomPosition();
                    float objectScale = GenerateRandomScale();

                    GameObject newObject = GenerateNewObject(objectPosition, objectScale);

                    // Set sprite randomly
                    newObject.GetComponent<SpriteRenderer>().sprite = starSprites[Random.Range(0, starSprites.Length)];

                    // Update generation variables
                    DefineNextGeneration();
                }

                DefineNextGeneration();
            }
        }
    }

    private float CalculateGeneratingChance() {
        if (maxAmount - amountAlive > 5) {
            return 100;
        }
        else {
            int targetAmountDiff = amountAlive - maxAmount;
            return BASE_CHANCE - targetAmountDiff*CHANCE_DIFF_MULTIPLIER + Mathf.Pow(BASE_CHANCE_MULTIPLIER, targetAmountDiff);
        }
    }

    public override void IncreaseAmountAlive() {
        base.IncreaseAmountAlive();
        BackgroundStateController.controller.IncreaseLight(BASE_STAR_INTENSITY);
    }
    public override void DecreaseAmountAlive() {
        base.DecreaseAmountAlive();
        BackgroundStateController.controller.DecreaseLight(BASE_STAR_INTENSITY);
    }

}
