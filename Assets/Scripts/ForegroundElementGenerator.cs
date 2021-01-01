using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ForegroundElementGenerator : MonoBehaviour {

	/*
	 * Constants
	 */
	// Obstacle types
	private const int DEBRIS_TYPE = 1;
	private const int METEOR_TYPE = 2;
	private const int STRAY_ENGINE_TYPE = 3;

	// Spawn interval
	public const int DEFAULT_SPAWN_INTERVAL_TYPE = 1;
	public const int EASY_SPAWN_INTERVAL_TYPE = 2;
	public const int HARD_SPAWN_INTERVAL_TYPE = 3;
	private const float DEFAULT_MIN_SPAWN_INTERVAL = 0.3f;
	private const float DEFAULT_MAX_SPAWN_INTERVAL = 0.95f;
	private const float EASY_MIN_SPAWN_INTERVAL = 0.75f;
	private const float EASY_MAX_SPAWN_INTERVAL = 1.5f;
	private const float HARD_MIN_SPAWN_INTERVAL = 0.25f;
	private const float HARD_MAX_SPAWN_INTERVAL = 0.75f;

	// Chances of energy spawns
	public const float DEFAULT_CHANCE_OF_4_ENERGIES = 5f;
	public const float DEFAULT_CHANCE_OF_3_ENERGIES = 20f;
	public const float DEFAULT_CHANCE_OF_2_ENERGIES = 45f;

	public const float DEFAULT_OBSTACLE_SPAWN_CHANCE = 20f;
	public const int DEFAULT_DEBRIS_SPAWN_CHANCE = 65;
	public const int DEFAULT_METEOR_SPAWN_CHANCE = 30;
	public const int DEFAULT_STRAY_ENGINE_SPAWN_CHANCE = 5;

	private const float DEFAULT_MOVING_ELEMENT_CHANCE = 20f;

	// Vertical space control during debris formations
	private const float MIN_VERT_SPACE_BETWEEN_ELEMENTS = 0.1f;

	/*
	 * Energy prefabs
	 */
	public GameObject positiveEnergyPrefab;
	public GameObject negativeEnergyPrefab;

	/*
	 * Energy spawning chances, chances should be put cummulatively counting from most to least energies
	 * e.g. 4-energies chance = 5 means 5%, 3-energies chance = 20 means 15%, 2-energies chance = 45 means 25%
	 */
	private float chanceOf2Energies = DEFAULT_CHANCE_OF_2_ENERGIES;
	private float chanceOf3Energies = DEFAULT_CHANCE_OF_3_ENERGIES;
	private float chanceOf4Energies = DEFAULT_CHANCE_OF_4_ENERGIES;

	/*
	 * Power Up prefabs
	 */
	public GameObject neutralizerPrefab;
	public GameObject growthPrefab;

	/*
	 * Obstacle prefabs
	 */
	//public List<GameObject> obstaclePrefabList;
	public List<GameObject> meteorPrefabList;
	public List<GameObject> debrisPrefabList;
	public List<GameObject> strayEnginePrefabList;
	// Chance of a spawn being an obstacle
	private float obstacleSpawnChance;
	private List<(int, int)> obstacleSpawnChancePool = new List<(int, int)>();
	// Individual chances for each obstacle type
	private int debrisSpawnChance = DEFAULT_DEBRIS_SPAWN_CHANCE;
	private int meteorSpawnChance = DEFAULT_METEOR_SPAWN_CHANCE;
	private int strayEngineSpawnChance = DEFAULT_STRAY_ENGINE_SPAWN_CHANCE;

	// Spawn control
	float nextSpawnTimer;
	Coroutine generationCoroutine = null;

	// Spawn intervals
	float minSpawnInterval = DEFAULT_MIN_SPAWN_INTERVAL;
	float maxSpawnInterval = DEFAULT_MAX_SPAWN_INTERVAL;

	// Use this for initialization
	void Start() {

	}


	void OnEnable() {
		generationCoroutine = StartCoroutine(GenerateForegroundElements());
	}

    void OnDisable() {
		if (generationCoroutine != null) {
			StopCoroutine(generationCoroutine);
		}
	}

    private IEnumerator GenerateForegroundElements() {
		while (true) {
			DefineNextSpawnTimer();

			yield return new WaitForSeconds(nextSpawnTimer);

			// Define how many should be spawned
			SpawnForegroundElements();

			//TODO get a better way of spawning power ups
			//float curSpawnPosition = SPAWN_CAMERA_OFFSET + GameController.GetCameraXMax();
			//switch (Random.Range(0, 30)) {
			//	case 0:
			//                    GameObject neutralizer = (GameObject)Instantiate(neutralizerPrefab, new Vector3(curSpawnPosition, Random.Range(-3.1f, 3.1f), 0), transform.rotation);
			//		break;

			//	case 1:
			//                    GameObject growth = (GameObject)Instantiate(growthPrefab, new Vector3(curSpawnPosition, Random.Range(-3.1f, 3.1f), 0), transform.rotation);
			//		break;
			//}
		}
	}

	private void SpawnForegroundElements() {
		float curSpawnPosition = ForegroundController.SPAWN_CAMERA_OFFSET + GameController.GetCameraXMax();

		int currentState = StageController.controller.GetCurrentMomentState();

		switch (currentState) {
			case StageMoment.NO_SPAWN:
				// Disable itself to not spawn anything
				enabled = false;
				break;

			case StageMoment.COMMON_RANDOM_SPAWN:
				SpawnSimpleRandom(curSpawnPosition);
				// Test if moving elements will also spawn
				if (GameController.RollChance(DEFAULT_MOVING_ELEMENT_CHANCE)) {
					SpawnMovingElement();
				}
				break;

			case StageMoment.OBSTACLE_GALORE:
				SpawnObstacles(curSpawnPosition);
				// TODO Add moving object spawning
				break;

			case StageMoment.ENERGY_GALORE:
				SpawnEnergies(curSpawnPosition);
				// Test if moving elements will also spawn
				if (GameController.RollChance(DEFAULT_MOVING_ELEMENT_CHANCE)) {
					SpawnMovingElement();
				}
				break;
		}
	}

	private void SpawnMovingElement() {
		// TODO Choose from pool of elements
		int elementType = DefineNewForegroundElement();

		// Define position
		float positionY = Random.Range(GameController.GetCameraYMin(), GameController.GetCameraYMax());
		Vector3 position = new Vector3(ForegroundController.SPAWN_CAMERA_OFFSET + GameController.GetCameraXMax(), positionY, 0);

		// Spawn element
		GameObject spawnedElement = SpawnForegroundElement(elementType, position, GameObjectUtil.GenerateRandomRotation());

		// Add directional moving object depending on its position
		MovingObject moveScript = spawnedElement.AddComponent<MovingObject>();
		moveScript.Speed = new Vector3(Random.Range(-2.25f, -1f), -spawnedElement.transform.position.y, 0);
    }


	[SerializeField]
	ObstacleRemover obstacleRemover = null;

	int obstaclesAdded = 0;

	float durationOnScreen = 0;

	private void SpawnObstacles(float curSpawnPosition) {
		if (durationOnScreen == 0) {
			durationOnScreen = (GameController.GetCameraXMax() + 5 - GameController.GetCameraXMin()) / PlayerController.controller.GetSpeed();
		}

		// Spawn first
		float positionY = Random.Range(GameController.GetCameraYMin(), GameController.GetCameraYMax());
		GameObject spawnedObstacle = SpawnForegroundElement(ChooseObstacle(),
			new Vector3(curSpawnPosition, positionY, 0), GameObjectUtil.GenerateRandomRotation(), false);

		obstaclesAdded++;
		StartCoroutine(DecreaseObstaclesAdded());

		// Start continuous spawning coroutine
		int newObstaclesAmount = Random.Range(0, 3);
		for (int i = 0; i < newObstaclesAmount; i++) {
			StartCoroutine(SpawnNextObstacles(spawnedObstacle.transform));
        }
	}

	IEnumerator SpawnNextObstacles(Transform parentObstacle) {
		yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));

		// Check if parent is still ahead of the screen for building obstacles
		if (parentObstacle.position.x > GameController.GetCameraXMax()) {

			// Gather parent obstacle data
			float parentSizeX = GameObjectUtil.GetGameObjectHorizontalSize(parentObstacle.gameObject);
			float parentSizeY = GameObjectUtil.GetGameObjectVerticalSize(parentObstacle.gameObject);

			// Get random position between a cell size, to twice its size
			float positionX = parentObstacle.position.x + Random.Range(parentSizeX, parentSizeX * 2);

			// Define Y axis position
			float positionY = 0;

			// Tries enough times before giving up to find a satisfactory Y position
			int tries = 1;
			int maxTries = 3;

			do {
				float obstacleOffset = Random.Range(parentSizeY / 2, parentSizeY);
				if (GameController.RollChance(50)) {
					obstacleOffset *= -1;
				}
				positionY = parentObstacle.position.y + obstacleOffset;
				tries++;
			}
			while (tries < maxTries && (positionY > GameController.GetCameraYMax() || positionY < GameController.GetCameraYMin()));

			if (tries < maxTries) {
				// Position vector is ready
				Vector3 obstaclePosition = new Vector3(positionX, positionY, 0);

				// Define obstacle prefab
				GameObject spawnedObstacle = SpawnForegroundElement(ChooseObstacle(),
					obstaclePosition, GameObjectUtil.GenerateRandomRotation(), false);

				// Update count
				obstaclesAdded++;
				StartCoroutine(DecreaseObstaclesAdded());

				// Add next obstacles
				float chance = Random.Range(0, 100);
				int newObstaclesAmount = 0;
				if (chance < 40) {
				}
				else if (chance < 80) {
					newObstaclesAmount = 1;
				}
				else {
					newObstaclesAmount = 2;
				}
				for (int i = 0; i < newObstaclesAmount; i++) {
					StartCoroutine(SpawnNextObstacles(spawnedObstacle.transform));
				}
			}
		}

        // Check the amount of obstacled added to decide whether remover should be activated
        if (obstaclesAdded > 30) {
            if (obstacleRemover.gameObject.activeSelf) {
                obstacleRemover.ResetDeactivationTimer();
            }
            else {
                obstacleRemover.gameObject.SetActive(true);
            }
		}
	}

	IEnumerator DecreaseObstaclesAdded() {
		yield return new WaitForSeconds(durationOnScreen);

		obstaclesAdded--;
		Debug.Log(obstaclesAdded + " Obstacles");
	}

	private void SpawnSimpleRandom(float curSpawnPosition) {
		// Roll random chances to define whether there will be 1 to 4 energies
		float chance = Random.Range(0, 99.99f);

		if (chance <= chanceOf4Energies) {
			// Create 4 energies pattern
			CreateElementsPattern(curSpawnPosition, 4);
		}
		else if (chance <= chanceOf3Energies) {
			// Create 3 energies pattern
			CreateElementsPattern(curSpawnPosition, 3);
		}
		else if (chance <= chanceOf2Energies) {
			// Create 2 energies pattern
			CreateElementsPattern(curSpawnPosition, 2);
		}
		else {
			// Simply add one energy
			CreateElementsPattern(curSpawnPosition, 1);
		}
	}

	private void SpawnEnergies(float curSpawnPosition) {
		// TODO Find better ways to spawn only energies
		CreateElementsPattern(curSpawnPosition, 5);
	}

	private void CreateElementsPattern(float positionX, int numElements) {
		//GameObject foregroundPrefab = DefineNewForegroundElement();
		//float energyVerticalSize = GameObjectUtil.GetGameObjectVerticalSize(foregroundPrefab);
		int elementType = DefineNewForegroundElement();

		int elementsSpawned = 0;

		List<(float, float)> availableSpaces = new List<(float, float)>();
        //float minPositionY = GameController.GetCameraYMin() + energyVerticalSize / 2;
        //float maxPositionY = GameController.GetCameraYMax() - energyVerticalSize / 2;
        float minPositionY = GameController.GetCameraYMin();
        float maxPositionY = GameController.GetCameraYMax();

        availableSpaces.Add((minPositionY, maxPositionY));

		while (elementsSpawned < numElements) {
			// Choose between available spaces
			if (availableSpaces.Count == 0) {
				// No available space
				return;
			}
			(float, float) availableSpace = availableSpaces[Random.Range(0, availableSpaces.Count)];

			float positionY = Random.Range(availableSpace.Item1, availableSpace.Item2);

			GameObject spawnedObject = SpawnForegroundElement(elementType, new Vector3(positionX, positionY, 0), GameObjectUtil.GenerateRandomRotation());
			float verticalSize = GameObjectUtil.GetGameObjectVerticalSize(spawnedObject);

			elementsSpawned++;

			// Remove item from available spaces list
			availableSpaces.Remove(availableSpace);

			// Generate two new items for available spaces list
			// Item 1 (underneath)
			minPositionY = availableSpace.Item1;
			maxPositionY = positionY - verticalSize - MIN_VERT_SPACE_BETWEEN_ELEMENTS;
            availableSpaces.Add((minPositionY, maxPositionY));

			// Item 2 (above)
			minPositionY = positionY + verticalSize + MIN_VERT_SPACE_BETWEEN_ELEMENTS;
            maxPositionY = availableSpace.Item2;
            availableSpaces.Add((minPositionY, maxPositionY));

			// Check if it is a moving object
			if (spawnedObject.GetComponent<MovingObjectActivator>() != null) {
				// TODO Making so every moving object is a shaky one
				//if (GameController.RollChance(50)) {
					// Chance of moving object start delayed
					//if (GameController.RollChance(50)) {
						spawnedObject.GetComponent<MovingObjectActivator>().ActivationDelay = 0.75f;
						// TODO Improve this, possibly with more prefabs
						spawnedObject.AddComponent<ShakyObject>();
					//}
					spawnedObject.GetComponent<MovingObjectActivator>().enabled = true;
				//}
			}
			
		}
	}

	// Returns whether element was succesfully spawned
	private GameObject SpawnForegroundElement(int elementType, Vector3 position, Quaternion rotation,
		bool randomizedX = true) {
		if (randomizedX) {
			// Add randomness to the horizontal axis
			float cameraLengthFraction = (GameController.GetCameraXMax() - GameController.GetCameraXMin()) / 4;
			position = new Vector3(position.x + Random.Range(0, cameraLengthFraction), position.y, position.z);
		}

		GameObject newForegroundElement = ObjectPool.SharedInstance.SpawnPooledObject(elementType, position, rotation);

		return newForegroundElement;
	}

	private int DefineNewForegroundElement() {
        // Keep reference
        int newElementType = 0;

		//TODO improve obstacle/energy choosing
		if (GameController.RollChance(obstacleSpawnChance)) {
			newElementType = ChooseObstacle();
		}
		else {
			// TODO Find another way to define chance of energy prefab
			if (GameController.controller.GetCurrentDay() == 15) {
				return ObjectPool.NEGATIVE_ENERGY;
            }
			// Define each energy
			switch (Random.Range(0, 2)) {
				case 0:
					newElementType = ObjectPool.POSITIVE_ENERGY;
					break;

				case 1:
					newElementType = ObjectPool.NEGATIVE_ENERGY;
					break;
			}
		}

		return newElementType;
	}

	private void DefineNextSpawnTimer() {
		int currentState = StageController.controller.GetCurrentMomentState();

		switch (currentState) {
			//case StageMoment.NO_SPAWN:
			//	nextSpawnTimer = 0;
			//	break;

			case StageMoment.COMMON_RANDOM_SPAWN:
				nextSpawnTimer = Random.Range(minSpawnInterval, maxSpawnInterval);
				break;

			case StageMoment.OBSTACLE_GALORE:
				nextSpawnTimer = minSpawnInterval;
				break;

			case StageMoment.ENERGY_GALORE:
				nextSpawnTimer = Random.Range(minSpawnInterval, (maxSpawnInterval + minSpawnInterval) /2);
				break;

		}
	}

	public void IncreaseNextSpawnTimer(float amountToIncrease) {
		nextSpawnTimer += amountToIncrease;
	}

	private int ChooseObstacle() {
		// TODO Choose from available obstacle types
		int completeChance = obstacleSpawnChancePool[obstacleSpawnChancePool.Count - 1].Item2;

		int randomChoice = Mathf.RoundToInt(1 + Random.Range(0, 1.0f) * (completeChance - 1));

		int spawnType = GetTypeFromPoolChance(randomChoice);
		switch (spawnType) {
			case DEBRIS_TYPE:
				return ObjectPool.DEBRIS;
			case METEOR_TYPE:
				return ObjectPool.ASTEROID;
			case STRAY_ENGINE_TYPE:
				return ObjectPool.STRAY_ENGINE;
		}
		// TODO remove
		Debug.LogError("Invalid obstacle type " + spawnType + "..." + System.String.Join(",",obstacleSpawnChancePool));
		return 0;
	}

	void PrepareObstacleChancesPool() {
		obstacleSpawnChancePool.Clear();

		// Debris
		if (debrisSpawnChance > 0) {
			AddChanceToPool(DEBRIS_TYPE, debrisSpawnChance);
		}

		// Meteors
		if (meteorSpawnChance > 0) {
			AddChanceToPool(METEOR_TYPE, meteorSpawnChance);
		}

		// Stray engines
		if (strayEngineSpawnChance > 0) {
			AddChanceToPool(STRAY_ENGINE_TYPE, strayEngineSpawnChance);
		}
	}

	void AddChanceToPool(int spawnType, int chance) {
		if (obstacleSpawnChancePool.Count > 0) {
			obstacleSpawnChancePool.Add((spawnType, obstacleSpawnChancePool[obstacleSpawnChancePool.Count - 1].Item2 + chance));
		}
		else {
			obstacleSpawnChancePool.Add((spawnType, chance));
		}
	}

	// Returns the type chosen based on the chance
	int GetTypeFromPoolChance(int chance) {
		foreach ((int, int) spawnChance in obstacleSpawnChancePool) {
			if (spawnChance.Item2 >= chance) {
				return spawnChance.Item1;
			}
		}
		return 0;
	}

	/*
	 * Getters and Setters
	 */
	public void SetChanceOf2Energies(float chanceOf2Energies) {
		this.chanceOf2Energies = chanceOf2Energies;
    }

	public void SetChanceOf3Energies(float chanceOf3Energies) {
		this.chanceOf3Energies = chanceOf3Energies;
    }
	
	public void SetChanceOf4Energies(float chanceOf4Energies) {
		this.chanceOf4Energies = chanceOf4Energies;
    }

	public void SetObstacleSpawnChance(float obstacleSpawnChance) {
		this.obstacleSpawnChance = obstacleSpawnChance;
		if (obstacleSpawnChance != 0) {
			PrepareObstacleChancesPool();
		}
    }

	public void SetDebrisSpawnChance(int debrisSpawnChance) {
		this.debrisSpawnChance = debrisSpawnChance;
	}

	public void SetMeteorSpawnChance(int meteorSpawnChance) {
		this.meteorSpawnChance = meteorSpawnChance;
	}

	public void SetStrayEngineSpawnChance(int strayEngineSpawnChance) {
		this.strayEngineSpawnChance = strayEngineSpawnChance;
	}

	public void SetSpawnInterval(int type) {
		switch (type) {
			case DEFAULT_SPAWN_INTERVAL_TYPE:
				minSpawnInterval = DEFAULT_MIN_SPAWN_INTERVAL;
				maxSpawnInterval = DEFAULT_MAX_SPAWN_INTERVAL;
				break;

			case EASY_SPAWN_INTERVAL_TYPE:
				minSpawnInterval = EASY_MIN_SPAWN_INTERVAL;
				maxSpawnInterval = EASY_MAX_SPAWN_INTERVAL;
				break;

			case HARD_SPAWN_INTERVAL_TYPE:
				minSpawnInterval = HARD_MIN_SPAWN_INTERVAL;
				maxSpawnInterval = HARD_MAX_SPAWN_INTERVAL;
				break;
		}
    }
}
