using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ForegroundElementGenerator : MonoBehaviour {

	/*
	 * Constants
	 */
	// Spawn interval
	public const float DEFAULT_MIN_SPAWN_INTERVAL = 0.3f;
	public const float DEFAULT_MAX_SPAWN_INTERVAL = 0.95f;
	public const float EASY_MIN_SPAWN_INTERVAL = 0.75f;
	public const float EASY_MAX_SPAWN_INTERVAL = 1.3f;
	public const float HARD_MIN_SPAWN_INTERVAL = 0.25f;
	public const float HARD_MAX_SPAWN_INTERVAL = 0.75f;

	// Chances of energy spawns
	public const float DEFAULT_CHANCE_OF_4_ENERGIES = 5f;
	public const float DEFAULT_CHANCE_OF_3_ENERGIES = 20f;
	public const float DEFAULT_CHANCE_OF_2_ENERGIES = 45f;

	// Chance of moviment element spawn
	public const float DEFAULT_MOVING_ELEMENT_CHANCE = 20f;

	// Chance of the first element be created right at the same position as the player's
	private const float DEFAULT_PLAYER_POSITION_CHANCE = 33.33f;

	// Magnetic Barrier constants
	private const float EASY_MIN_MAGNETIC_BARRIER_SPAWN_INTERVAL = 12f;
	private const float EASY_MAX_MAGNETIC_BARRIER_SPAWN_INTERVAL = 15f;
	private const float MEDIUM_MIN_MAGNETIC_BARRIER_SPAWN_INTERVAL = 7f;
	private const float MEDIUM_MAX_MAGNETIC_BARRIER_SPAWN_INTERVAL = 10f;
	private const float HARD_MIN_MAGNETIC_BARRIER_SPAWN_INTERVAL = 3f;
	private const float HARD_MAX_MAGNETIC_BARRIER_SPAWN_INTERVAL = 6f;
	private const float DEFAULT_POSITIVE_MAGNETIC_BARRIER_CHANCE = 50f;

	// Vertical space control during debris formations
	private const float MIN_VERT_SPACE_BETWEEN_ELEMENTS = 0.1f;

	/*
	 * Energy spawning chances, chances should be put cummulatively counting from most to least energies
	 * e.g. 4-energies chance = 5 means 5%, 3-energies chance = 20 means 15%, 2-energies chance = 45 means 25%
	 */
	private float chanceOf2Energies = DEFAULT_CHANCE_OF_2_ENERGIES;
	private float chanceOf3Energies = DEFAULT_CHANCE_OF_3_ENERGIES;
	private float chanceOf4Energies = DEFAULT_CHANCE_OF_4_ENERGIES;
	
	// Chance of a spawn being an obstacle
	private List<ElementSpawnChance> elementChancesPool = new List<ElementSpawnChance>();
	private List<ElementSpawnChance> elementsSpawnChance = new List<ElementSpawnChance>();

	// Spawn control
	float nextSpawnTimer;
	List<float> additionalSpawnTimer = new List<float>();
	Coroutine generationCoroutine = null;
	int obstaclesAdded = 0;
	float durationOnScreen = 0;

	// Magnetic Barrier spawn control
	float magneticBarrierSpawnTimer;
	Coroutine magneticBarrierCoroutine = null;
	// The interval of magnetic barrier spawning defaults to easy
	float currentMinMagneticSpawnInterval = EASY_MIN_MAGNETIC_BARRIER_SPAWN_INTERVAL;
	float currentMaxMagneticSpawnInterval = EASY_MAX_MAGNETIC_BARRIER_SPAWN_INTERVAL;
	float currentPositiveMagneticBarrierChance = DEFAULT_POSITIVE_MAGNETIC_BARRIER_CHANCE;


	[SerializeField]
	ObstacleRemover obstacleRemover = null;

	// Spawn intervals
	float minSpawnInterval = DEFAULT_MIN_SPAWN_INTERVAL;
	float maxSpawnInterval = DEFAULT_MAX_SPAWN_INTERVAL;

	// Chance of spawning moving elements
	float movingElementSpawnChance = DEFAULT_MOVING_ELEMENT_CHANCE;

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
			while (additionalSpawnTimer.Count > 0) {
				float waitTime = additionalSpawnTimer[0];
				additionalSpawnTimer.RemoveAt(0);
				yield return new WaitForSeconds(waitTime);
            }

			// Define how many should be spawned
			SpawnForegroundElements();
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
			// TODO Check if additional spawn timer should also be applied to magnetic barriers

			// Decide if positive or negative
			bool magneticBarrierPositive = DefineNextMagneticBarrierType();

			// Warn
			WarnAboutMagneticBarrier(magneticBarrierPositive);

			yield return new WaitForSeconds(MagneticBarrier.WARNING_PERIOD_BEFORE_MAGNETIC_BARRIER);

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
		magneticBarrierSpawnTimer = Random.Range(currentMinMagneticSpawnInterval, currentMaxMagneticSpawnInterval);
	}

	protected bool DefineNextMagneticBarrierType() {
		return GameController.RollChance(currentPositiveMagneticBarrierChance);
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
				if (GameController.RollChance(movingElementSpawnChance)) {
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
				if (GameController.RollChance(movingElementSpawnChance)) {
					SpawnMovingElement();
				}
				break;
		}
	}

	private void SpawnMovingElement() {
		// Choose from pool of elements
		ElementsEnum elementType = DefineNewForegroundElement();

		// Define position
		float positionY = Random.Range(GameController.GetCameraYMin(), GameController.GetCameraYMax());
		Vector3 position = new Vector3(ForegroundController.SPAWN_CAMERA_OFFSET + GameController.GetCameraXMax(), positionY, 0);

		// Spawn element
		GameObject spawnedElement = SpawnForegroundElement(elementType, position, GameObjectUtil.GenerateRandomRotation());

		// Add directional moving object depending on its position
		IMovingObject moveScript = spawnedElement.GetComponent<IMovingObject>();
		moveScript.SetSpeed(new Vector3(Random.Range(ForegroundElement.MIN_FOREGROUND_ELEMENT_SPEED_X, ForegroundElement.MAX_FOREGROUND_ELEMENT_SPEED_X), -spawnedElement.transform.position.y, 0));
    }

	private void SpawnObstacles(float curSpawnPosition) {
		// Spawn first
		float positionY = Random.Range(GameController.GetCameraYMin(), GameController.GetCameraYMax());
		GameObject spawnedObstacle = SpawnForegroundElement(ChooseElement(),
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
			float positionY;

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
				GameObject spawnedObstacle = SpawnForegroundElement(ChooseElement(),
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

			// Define vertical position
			float positionY;
			if (elementsSpawned == 0 && GameController.RollChance(DEFAULT_PLAYER_POSITION_CHANCE)) {
				positionY = PlayerController.controller.transform.position.y;
			}
			else {
				positionY = Random.Range(availableSpace.Item1, availableSpace.Item2);
			}

			ElementsEnum elementType = DefineNewForegroundElement();
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
		}
	}

	// Returns whether element was succesfully spawned
	private GameObject SpawnForegroundElement(ElementsEnum elementType, Vector3 position, Quaternion rotation,
		bool randomizedX = true) {
		if (randomizedX) {
			// Add randomness to the horizontal axis
			float cameraLengthFraction = (GameController.GetCameraXMax() - GameController.GetCameraXMin()) / 4;
			position = new Vector3(position.x + Random.Range(0, cameraLengthFraction), position.y, position.z);
		}

		GameObject newForegroundElement = ObjectPool.SharedInstance.SpawnPooledObject(elementType, position, rotation);

		return newForegroundElement;
	}

	private ElementsEnum DefineNewForegroundElement() {
        // Keep reference
        ElementsEnum newElementType = ChooseElement();

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
				nextSpawnTimer = Random.Range(minSpawnInterval, maxSpawnInterval);
				break;

			case MomentSpawnStateEnum.EnergyGalore:
				nextSpawnTimer = Random.Range(minSpawnInterval, (maxSpawnInterval + minSpawnInterval) /2);
				break;

		}
	}

	public void IncreaseNextSpawnTimer(float amountToIncrease) {
		//nextSpawnTimer += amountToIncrease;
		additionalSpawnTimer.Add(amountToIncrease);
	}

	private ElementsEnum ChooseElement() {
		// Choose from available obstacle types
		float completeChance = elementChancesPool[elementChancesPool.Count - 1].chance;

		float randomChoice = Random.Range(0, completeChance);

		ElementsEnum spawnType = GetTypeFromPoolChance(randomChoice);
		// TODO Remove check once it works
		if (spawnType != 0) {
			return spawnType;
		}
		// TODO remove
		else {
			Debug.LogError("Invalid obstacle type " + spawnType + "..." + System.String.Join(",", elementChancesPool));
			return 0;
		}
	}

	void PrepareElementChancesPool() {
		elementChancesPool.Clear();

		foreach (ElementSpawnChance elementSpawnChance in elementsSpawnChance) {
			AddChanceToPool(elementSpawnChance.element, elementSpawnChance.chance);
        }
	}

	void AddChanceToPool(ElementsEnum elementType, float chance) {
		if (elementChancesPool.Count > 0) {
			ElementSpawnChance poolElement = new ElementSpawnChance(elementType, elementChancesPool[elementChancesPool.Count - 1].chance + chance);
			elementChancesPool.Add(poolElement);
		}
		else {
			ElementSpawnChance poolElement = new ElementSpawnChance(elementType, chance);
			elementChancesPool.Add(poolElement);
		}
	}

	// Returns the type chosen based on the chance
	ElementsEnum GetTypeFromPoolChance(float chance) {
		foreach (ElementSpawnChance spawnChance in elementChancesPool) {
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

	public void SetElementsSpawnChance(List<ElementSpawnChance> elementsSpawnChance) {
		this.elementsSpawnChance.Clear();

		foreach (ElementSpawnChance spawnChance in elementsSpawnChance) {
			// Remove magnetic barriers from spawning chances
			if (spawnChance.element != ElementsEnum.MAGNETIC_BARRIER) {
				this.elementsSpawnChance.Add(spawnChance);
            }
        }

		PrepareElementChancesPool();
	}

	public void SetSpawnInterval(SpawnIntervalEnum type) {
		switch (type) {
			case SpawnIntervalEnum.Default:
				minSpawnInterval = DEFAULT_MIN_SPAWN_INTERVAL;
				maxSpawnInterval = DEFAULT_MAX_SPAWN_INTERVAL;
				break;

			case SpawnIntervalEnum.Easy:
				minSpawnInterval = EASY_MIN_SPAWN_INTERVAL;
				maxSpawnInterval = EASY_MAX_SPAWN_INTERVAL;
				break;

			case SpawnIntervalEnum.Hard:
				minSpawnInterval = HARD_MIN_SPAWN_INTERVAL;
				maxSpawnInterval = HARD_MAX_SPAWN_INTERVAL;
				break;
		}
    }

	public void SetMagneticBarrierSpawnInterval(DifficultyEnum difficulty) {
		switch (difficulty) {
			case DifficultyEnum.Easy:
				currentMinMagneticSpawnInterval = EASY_MIN_MAGNETIC_BARRIER_SPAWN_INTERVAL;
				currentMaxMagneticSpawnInterval = EASY_MAX_MAGNETIC_BARRIER_SPAWN_INTERVAL;
				break;
			case DifficultyEnum.Medium:
				currentMinMagneticSpawnInterval = MEDIUM_MIN_MAGNETIC_BARRIER_SPAWN_INTERVAL;
				currentMaxMagneticSpawnInterval = MEDIUM_MAX_MAGNETIC_BARRIER_SPAWN_INTERVAL;
				break;
			case DifficultyEnum.Hard:
				currentMinMagneticSpawnInterval = HARD_MIN_MAGNETIC_BARRIER_SPAWN_INTERVAL;
				currentMaxMagneticSpawnInterval = HARD_MAX_MAGNETIC_BARRIER_SPAWN_INTERVAL;
				break;
        }
    }

	public void SetMagneticBarrierPositiveChance(MagneticBarrierValueEnum value) {
		switch (value) {
			case MagneticBarrierValueEnum.Random:
				currentPositiveMagneticBarrierChance = DEFAULT_POSITIVE_MAGNETIC_BARRIER_CHANCE;
				break;
			case MagneticBarrierValueEnum.Positive:
				currentPositiveMagneticBarrierChance = 100f;
				break;
			case MagneticBarrierValueEnum.Negative:
				currentPositiveMagneticBarrierChance = 0f;
				break;
			case MagneticBarrierValueEnum.PositiveOrNegative:
				// Pick either positive or negative to continue
				currentPositiveMagneticBarrierChance = GameController.RollChance(50f) ? 100f : 0f;
				break;
		}
	}

	public void SetMovingElementSpawnChance(float movingElementSpawnChance) {
		this.movingElementSpawnChance = movingElementSpawnChance;
	}
}
