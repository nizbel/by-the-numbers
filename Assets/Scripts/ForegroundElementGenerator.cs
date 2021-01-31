using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ForegroundElementGenerator : MonoBehaviour {

	/*
	 * Constants
	 */
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
	public const int DEFAULT_ASTEROID_SPAWN_CHANCE = 30;
	public const int DEFAULT_STRAY_ENGINE_SPAWN_CHANCE = 5;

	private const float DEFAULT_MOVING_ELEMENT_CHANCE = 20f;

	// Magnetic Barrier constants
	private const float MIN_MAGNETIC_BARRIER_SPAWN_INTERVAL = 10;
	private const float MAX_MAGNETIC_BARRIER_SPAWN_INTERVAL = 15;
	private const float WARNING_PERIOD_BEFORE_MAGNETIC_BARRIER = 4.5f;

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
	private List<ElementSpawnChance> obstacleSpawnChancePool = new List<ElementSpawnChance>();
	private List<ElementSpawnChance> elementsSpawnChance = new List<ElementSpawnChance>();

	// Spawn control
	float nextSpawnTimer;
	Coroutine generationCoroutine = null;
	int obstaclesAdded = 0;
	float durationOnScreen = 0;

	// Magnetic Barrier spawn control
	float magneticBarrierSpawnTimer;
	Coroutine magneticBarrierCoroutine = null;

	[SerializeField]
	ObstacleRemover obstacleRemover = null;

	// Spawn intervals
	float minSpawnInterval = DEFAULT_MIN_SPAWN_INTERVAL;
	float maxSpawnInterval = DEFAULT_MAX_SPAWN_INTERVAL;

	// Use this for initialization
	void Start() {
		// Define the amount of time elements stay on screen
		durationOnScreen = (GameController.GetCameraXMax() + 5 - GameController.GetCameraXMin()) / PlayerController.controller.GetSpeed();
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
			// TODO Find a way to reflect IncreaseSpawnTimer
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

	public void StartMagneticBarriersSpawn() {
		magneticBarrierCoroutine = StartCoroutine(GenerateMagneticBarriers());
    }

	public void StopMagneticBarriersSpawn() {
		if (magneticBarrierCoroutine != null) {
			StopCoroutine(magneticBarrierCoroutine);
		}
	}

	private IEnumerator GenerateMagneticBarriers() {
		while (true) {
			// Define spawn timer
			DefineMagneticBarrierSpawn();

			yield return new WaitForSeconds(magneticBarrierSpawnTimer);

			// Decide if positive or negative
			bool magneticBarrierPositive = DefineNextMagneticBarrierType();

			// Warn
			WarnAboutMagneticBarrier(magneticBarrierPositive);

			yield return new WaitForSeconds(WARNING_PERIOD_BEFORE_MAGNETIC_BARRIER);

			// Show on screen
			SpawnMagneticBarrier(magneticBarrierPositive);
		}
    }

	private void SpawnMagneticBarrier(bool positiveMagneticBarrier) {
		MagneticBarrier newMagneticBarrier = ValueRange.controller.CreateMagneticBarrier();
		newMagneticBarrier.transform.position = new Vector3(GameController.GetCameraXMax() + 2, 0, 0);

		// Set whether it is positive
		newMagneticBarrier.SetPositive(positiveMagneticBarrier);

		DefineMagneticBarrierSpawn();
	}

	// Define current magnetic barrier timer to appear
	private void DefineMagneticBarrierSpawn() {
		magneticBarrierSpawnTimer = Random.Range(MIN_MAGNETIC_BARRIER_SPAWN_INTERVAL, MAX_MAGNETIC_BARRIER_SPAWN_INTERVAL);
	}

	protected bool DefineNextMagneticBarrierType() {
		return GameController.RollChance(50);
	}

	// Show warning regarding magnetic barrier
	protected void WarnAboutMagneticBarrier(bool positiveMagneticBarrier) {
		// TODO Roll chance to test if narrator will also warn
		if (GameController.RollChance(50)) {
			NarratorController.controller.WarnBarrier(positiveMagneticBarrier);
		}

		ValueRange.controller.ActivateMagneticBarrierWarning(positiveMagneticBarrier);
	}

	private void SpawnForegroundElements() {
		float curSpawnPosition = ForegroundController.SPAWN_CAMERA_OFFSET + GameController.GetCameraXMax();

		MomentSpawnStateEnum currentState = StageController.controller.GetCurrentMomentState();

		switch (currentState) {
			case MomentSpawnStateEnum.NoSpawn:
				// Disable itself to not spawn anything
				enabled = false;
				break;

			case MomentSpawnStateEnum.CommonRandomSpawn:
				SpawnSimpleRandom(curSpawnPosition);
				// Test if moving elements will also spawn
				if (GameController.RollChance(DEFAULT_MOVING_ELEMENT_CHANCE)) {
					SpawnMovingElement();
				}
				break;

			case MomentSpawnStateEnum.ObstacleGalore:
				SpawnObstacles(curSpawnPosition);
				// TODO Add moving object spawning
				break;

			case MomentSpawnStateEnum.EnergyGalore:
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
		moveScript.Speed = new Vector3(Random.Range(MovingObject.MIN_FOREGROUND_ELEMENT_SPEED_X, MovingObject.MAX_FOREGROUND_ELEMENT_SPEED_X), -spawnedElement.transform.position.y, 0);
    }

	private void SpawnObstacles(float curSpawnPosition) {
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
		int elementType = DefineNewForegroundElement();

		int elementsSpawned = 0;

		List<(float, float)> availableSpaces = new List<(float, float)>();
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
			if (maxPositionY > minPositionY) {
				availableSpaces.Add((minPositionY, maxPositionY));
			}

			// Item 2 (above)
			minPositionY = positionY + verticalSize + MIN_VERT_SPACE_BETWEEN_ELEMENTS;
            maxPositionY = availableSpace.Item2;
			if (maxPositionY > minPositionY) {
				availableSpaces.Add((minPositionY, maxPositionY));
			}

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
		MomentSpawnStateEnum currentState = StageController.controller.GetCurrentMomentState();

		switch (currentState) {
			//case MomentSpawnStateEnum.NoSpawn:
			//	nextSpawnTimer = 0;
			//	break;

			case MomentSpawnStateEnum.CommonRandomSpawn:
				nextSpawnTimer = Random.Range(minSpawnInterval, maxSpawnInterval);
				break;

			case MomentSpawnStateEnum.ObstacleGalore:
				nextSpawnTimer = minSpawnInterval;
				break;

			case MomentSpawnStateEnum.EnergyGalore:
				nextSpawnTimer = Random.Range(minSpawnInterval, (maxSpawnInterval + minSpawnInterval) /2);
				break;

		}
	}

	public void IncreaseNextSpawnTimer(float amountToIncrease) {
		nextSpawnTimer += amountToIncrease;
	}

	private int ChooseObstacle() {
		// Choose from available obstacle types
		float completeChance = obstacleSpawnChancePool[obstacleSpawnChancePool.Count - 1].chance;

		float randomChoice = Random.Range(0, completeChance);

		ElementsEnum spawnType = GetTypeFromPoolChance(randomChoice);
		switch (spawnType) {
			case ElementsEnum.Debris:
				return ObjectPool.DEBRIS;
			case ElementsEnum.Asteroid:
				return ObjectPool.ASTEROID;
			case ElementsEnum.EnergyMine:
				return ObjectPool.ENERGY_MINE;
			case ElementsEnum.LightningFuse:
				return ObjectPool.ENERGY_FUSE;
			case ElementsEnum.StrayEngine:
				return ObjectPool.STRAY_ENGINE;
			case ElementsEnum.GenesisAsteroid:
				return ObjectPool.GENESIS_ASTEROID;
		}
		// TODO remove
		Debug.LogError("Invalid obstacle type " + spawnType + "..." + System.String.Join(",",obstacleSpawnChancePool));
		return 0;
	}

	void PrepareObstacleChancesPool() {
		obstacleSpawnChancePool.Clear();

		foreach (ElementSpawnChance elementSpawnChance in elementsSpawnChance) {
			AddChanceToPool(elementSpawnChance.element, elementSpawnChance.chance);
        }
	}

	void AddChanceToPool(ElementsEnum elementType, float chance) {
		if (obstacleSpawnChancePool.Count > 0) {
			ElementSpawnChance poolElement = new ElementSpawnChance(elementType, obstacleSpawnChancePool[obstacleSpawnChancePool.Count - 1].chance + chance);
			obstacleSpawnChancePool.Add(poolElement);
		}
		else {
			ElementSpawnChance poolElement = new ElementSpawnChance(elementType, chance);
			obstacleSpawnChancePool.Add(poolElement);
		}
	}

	// Returns the type chosen based on the chance
	ElementsEnum GetTypeFromPoolChance(float chance) {
		foreach (ElementSpawnChance spawnChance in obstacleSpawnChancePool) {
			if (spawnChance.chance >= chance) {
				return spawnChance.element;
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
    }

	public void SetElementsSpawnChance(List<ElementSpawnChance> elementsSpawnChance) {
		// Remove magnetic barriers from spawning chances
		foreach (ElementSpawnChance spawnChance in elementsSpawnChance) {
			if (spawnChance.element == ElementsEnum.MagneticBarrier) {
				elementsSpawnChance.Remove(spawnChance);
				break;
            }
        }

		this.elementsSpawnChance = elementsSpawnChance;
		PrepareObstacleChancesPool();
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
