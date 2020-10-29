using UnityEngine;

public class StarGenerator : BackgroundElementGenerator {

	public const float MIN_STAR_GENERATION_PERIOD = 0.2f;
	public const float MAX_STAR_GENERATION_PERIOD = 1.8f;
	public const float MIN_STAR_SCALE = 0.03f;
	public const float MAX_STAR_SCALE = 0.12f;

	public const int MAX_STAR_AMOUNT = 60;

    private const float BASE_CHANCE = 25;
    private const float BASE_CHANCE_MULTIPLIER = 0.475f;
    private const float CHANCE_DIFF_MULTIPLIER = 4.5f;

    public const float BASE_STAR_INTENSITY = 0.015f;

    // Use this for initialization
    void Start () {
		// Set values
		minGenerationPeriod = MIN_STAR_GENERATION_PERIOD;
		maxGenerationPeriod = MAX_STAR_GENERATION_PERIOD;
		minElementScale = MIN_STAR_SCALE;
		maxElementScale = MAX_STAR_SCALE;

        maxAmount = Random.Range(30, MAX_STAR_AMOUNT + 1);

        DefineNextGeneration();

        while (GameController.RollChance(CalculateGeneratingChance())) {
            int i = Random.Range(0, prefabs.Length);

            Vector3 objectPosition = new Vector3(Random.Range(GameController.GetCameraXMin(), GameController.GetCameraXMax()),
                Random.Range(GameController.GetCameraYMin(), GameController.GetCameraYMax()), 0);
            float objectScale = GenerateNormalDistributionScale();
            GameObject newObject = (GameObject)Instantiate(prefabs[i], objectPosition, Quaternion.Euler(0, 0, Random.Range(0, 180)));
            newObject.transform.localScale = new Vector3(objectScale, objectScale, objectScale);

            newObject.transform.parent = BackgroundStateController.controller.GetRandomBackgroundLayer().transform;
            LayeredBackgroundObject layerScript = newObject.AddComponent<LayeredBackgroundObject>();

            // Set this as its generator
            newObject.GetComponent<GeneratedDestructible>().setGenerator(this);
            IncreaseAmountAlive();
        }

        // Update last generation variable
        lastGeneratedTime = Time.timeSinceLevelLoad;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.timeSinceLevelLoad - lastGeneratedTime > nextGeneration) {
			if (GameController.RollChance(CalculateGeneratingChance())) {
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

			DefineNextGeneration();
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
