using UnityEngine;
using System.Collections.Generic;

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
	public const float DEFAULT_CHANCE_OF_4_BLOCKS = 5f;
	public const float DEFAULT_CHANCE_OF_3_BLOCKS = 20f;
	public const float DEFAULT_CHANCE_OF_2_BLOCKS = 45f;

	public const float DEFAULT_OBSTACLE_SPAWN_CHANCE = 20f;
	public const int DEFAULT_DEBRIS_SPAWN_CHANCE = 65;
	public const int DEFAULT_METEOR_SPAWN_CHANCE = 30;
	public const int DEFAULT_STRAY_ENGINE_SPAWN_CHANCE = 5;

	private const float DEFAULT_MOVING_ELEMENT_CHANCE = 20f;

	// Vertical space control during debris formations
	private const float MIN_VERT_SPACE_BETWEEN_BLOCKS = 0.1f;

	// Keeps half of the horizontal span of a cluster of obstacles
	private const float CLUSTER_HORIZONTAL_RADIUS = 1.25f;

	/*
	 * Block prefabs
	 */
	public GameObject addBlockPrefab;
	public GameObject subtractBlockPrefab;
	//public GameObject multiplyBlockPrefab;
	//public GameObject divideBlockPrefab;

	/*
	 * Energy spawning chances, chances should be put cummulatively counting from most to least blocks
	 * e.g. 4-blocks chance = 5 means 5%, 3-blocks chance = 20 means 15%, 2-blocks chance = 45 means 25%
	 */
	private float chanceOf2Blocks = DEFAULT_CHANCE_OF_2_BLOCKS;
	private float chanceOf3Blocks = DEFAULT_CHANCE_OF_3_BLOCKS;
	private float chanceOf4Blocks = DEFAULT_CHANCE_OF_4_BLOCKS;

	/*
	 * Energy formation prefabs
	 */
	//public List<GameObject> energyFormationList;
	//private bool formationSpawned = false;
	//private float formationCooldown = 0;

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

	/*
	 * Obstacle generator prefabs
	 */
	//public List<GameObject> obstacleGeneratorPrefabList;

	// Composite obstacle spawn control
	private List<Transform> currentObstacleControl = new List<Transform>();
	private List<(Transform, List<(float, float)>)> availableSpaceControl = new List<(Transform, List<(float, float)>)>();

	// Spawn control
	//float lastSpawn;
	float nextSpawnTimer;

	// Spawn intervals
	float minSpawnInterval = DEFAULT_MIN_SPAWN_INTERVAL;
	float maxSpawnInterval = DEFAULT_MAX_SPAWN_INTERVAL;

	// Use this for initialization
	void Start() {
		//lastSpawn = Time.timeSinceLevelLoad;

		//DefineNextSpawnTimer();
	}

	void OnEnable() {
		DefineNextSpawnTimer();
	}

	// Update is called once per frame
	void Update() {
		if (nextSpawnTimer <= 0) {

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
			DefineNextSpawnTimer();
		} else {
			nextSpawnTimer -= Time.deltaTime;
		}

		if (availableSpaceControl.Count > 0) {
			if (availableSpaceControl[0].Item1.position.x < GameController.GetCameraXMax() - CLUSTER_HORIZONTAL_RADIUS) {
				availableSpaceControl.RemoveAt(0);
			}
		}
	}

	private void SpawnForegroundElements() {
		float curSpawnPosition = ForegroundController.SPAWN_CAMERA_OFFSET + GameController.GetCameraXMax();

		int currentState = StageController.controller.GetCurrentMomentState();

		switch (currentState) {
			case StageMoment.NO_SPAWN:
				currentObstacleControl.Clear();
				// Disable itself to not spawn anything
				enabled = false;
				break;

			case StageMoment.COMMON_RANDOM_SPAWN:
				currentObstacleControl.Clear();
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

			case StageMoment.OPERATION_BLOCK_GALORE:
				currentObstacleControl.Clear();
				SpawnBlocks(curSpawnPosition);
				// Test if moving elements will also spawn
				if (GameController.RollChance(DEFAULT_MOVING_ELEMENT_CHANCE)) {
					SpawnMovingElement();
				}
				break;
		}
	}

	private void SpawnMovingElement() {
		// TODO Choose from pool of elements
		GameObject elementPrefab = DefineNewForegroundElement();

		// Define position
		float positionY = Random.Range(GameController.GetCameraYMin(), GameController.GetCameraYMax());
		Vector3 position = new Vector3(ForegroundController.SPAWN_CAMERA_OFFSET + GameController.GetCameraXMax(), positionY, 0);

		// Spawn element
		(bool, GameObject) spawnedElement = SpawnForegroundElement(elementPrefab, position, GameObjectUtil.GenerateRandomRotation());

		// Add directional moving object depending on its position
		MovingObject moveScript = spawnedElement.Item2.AddComponent<MovingObject>();
		moveScript.Speed = new Vector3(Random.Range(-2.25f, -1f), -spawnedElement.Item2.transform.position.y, 0);
    }

	private void SpawnObstacles(float curSpawnPosition) {
		if (currentObstacleControl.Count == 0) {
			// Spawn first
			float positionY = Random.Range(GameController.GetCameraYMin(), GameController.GetCameraYMax());
            GameObject spawnedObstacle = SpawnForegroundElement(ChooseObstaclePrefab(),
				new Vector3(curSpawnPosition, positionY, 0), GameObjectUtil.GenerateRandomRotation(), false).Item2;
			// Set it as control cell
			if (spawnedObstacle != null) {
				currentObstacleControl.Add(spawnedObstacle.transform);

				// Control available space for player
				availableSpaceControl.Clear();

				availableSpaceControl.Add(DefineCurrentAvailableSpaces(new List<Transform>() { spawnedObstacle.transform }));
			}
		}
		else {
			// Repeat until it reaches current spawn position
			float positionX = GameController.GetCameraXMax();

			while (positionX < curSpawnPosition && currentObstacleControl.Count > 0) {
				// Gather last added cell data
				Transform lastCellAdded = currentObstacleControl[currentObstacleControl.Count - 1];
				float lastCellPositionX = lastCellAdded.position.x;
				float lastCellSizeX = lastCellAdded.GetComponent<SpriteRenderer>().sprite.bounds.extents.x * lastCellAdded.localScale.x;


				List<Transform> newCells = new List<Transform>();
				foreach (Transform cell in currentObstacleControl) {
					float obstacleVerticalSize = GameObjectUtil.GetGameObjectVerticalSize(cell.gameObject);

					// Spawn up to 2 more for each cell
					int newCellsAmount = Random.Range(0, 3);
					for (int i = 0; i < newCellsAmount; i++) {
						// Get random position between a cell size, to twice its size
						positionX = lastCellPositionX + Random.Range(lastCellSizeX, lastCellSizeX * 2);

						// Define Y axis position
						float obstacleOffset = Random.Range(obstacleVerticalSize / 2, obstacleVerticalSize);
						if (GameController.RollChance(50)) {
							obstacleOffset *= -1;
						}
						float positionY = (cell.position + new Vector3(0, obstacleOffset, 0)).y;

						// Position vector is ready
						Vector3 obstaclePosition = new Vector3(positionX, positionY, 0);

                        // Define obstacle prefab
                        GameObject obstaclePrefab = ChooseObstaclePrefab();

						// Check if visible on camera and not too close to another obstacle
						if (Mathf.Abs(positionY) - GameObjectUtil.GetGameObjectVerticalSize(obstaclePrefab) / 2
							<= GameController.GetCameraYMax() && EnoughDistanceToTransformsList(obstaclePosition, newCells, 0.25f)) {

                            GameObject spawnedObstacle = SpawnForegroundElement(obstaclePrefab,
								obstaclePosition, GameObjectUtil.GenerateRandomRotation(), false).Item2;

							if (spawnedObstacle != null) {
								// Check if available space is enough for the ship
								(int, (Transform, List<(float, float)>)) newCluster = AddToObstacleCluster(spawnedObstacle.transform);
								bool enoughSpace = CheckIfEnoughAvailableSpace(newCluster.Item1, newCluster.Item2);

								// Spawn
								if (enoughSpace) {
									newCells.Add(spawnedObstacle.transform);

									// Add or update cluster of obstacles
									if (newCluster.Item1 == availableSpaceControl.Count) {
										// Add
										availableSpaceControl.Add(newCluster.Item2);
									}
									else {
										// Update
										availableSpaceControl.RemoveAt(newCluster.Item1);
										availableSpaceControl.Insert(newCluster.Item1, newCluster.Item2);
									}
								}
								else {
									// If the obstacle leaves not enough space for the player, delete it
									Destroy(spawnedObstacle);
								}
							}
						}
					}
				}
				// Remove old cells
				currentObstacleControl.Clear();
				currentObstacleControl.AddRange(newCells);
			}
		}
	}

	private (int, (Transform, List<(float, float)>)) AddToObstacleCluster(Transform transformToBeAdded) {
		// Iterate through positions in the available space control
		(Transform, List<(float, float)>) currentAvailableSpaces = (null, null);

		int clusterIndex = 0;
		for (; clusterIndex < availableSpaceControl.Count; clusterIndex++) {
			(Transform, List<(float, float)>) availableSpace = availableSpaceControl[clusterIndex];

			// Find which cluster transform should be added to
			if (Mathf.Abs(availableSpace.Item1.position.x - transformToBeAdded.position.x) <= CLUSTER_HORIZONTAL_RADIUS) {
				break;
			}
		}
		// If nothing is found, the object belongs to a new cluster
		if (clusterIndex == availableSpaceControl.Count) {
			currentAvailableSpaces = DefineCurrentAvailableSpaces(
				new List<Transform>() { transformToBeAdded });
		}
		else {
			// Inserts into a cluster
			currentAvailableSpaces = DefineCurrentAvailableSpaces(
				new List<Transform>() { transformToBeAdded }, clusterIndex);
		}

		return (clusterIndex, currentAvailableSpaces);
	}

	private bool CheckIfEnoughAvailableSpace(int clusterIndex, (Transform, List<(float, float)>) currentAvailableSpaces) {
		// Check if it allows for ship maneuvering between previous and next position, based on cluster index
		bool allowsManeuvering = currentAvailableSpaces.Item1 != null;
		if (allowsManeuvering && clusterIndex > 0) {
			allowsManeuvering = allowsManeuvering && CheckIfEnoughOverlap(availableSpaceControl[clusterIndex - 1], currentAvailableSpaces);
		}
		if (allowsManeuvering && (clusterIndex < availableSpaceControl.Count - 1)) {
			allowsManeuvering = allowsManeuvering && CheckIfEnoughOverlap(currentAvailableSpaces, availableSpaceControl[clusterIndex + 1]);
		}
		return allowsManeuvering;
	}

	private (Transform, List<(float, float)>) DefineCurrentAvailableSpaces(List<Transform> transformsList, int clusterToAddIndex = -1) {
		Transform mainTransform = null;
		List<(float, float)> availableSpaces = new List<(float, float)>();

		// Load cluster if there is one
		if (clusterToAddIndex != -1) {
			availableSpaces = availableSpaceControl[clusterToAddIndex].Item2;
			mainTransform = availableSpaceControl[clusterToAddIndex].Item1;
		}
		else {
			availableSpaces.Add((GameController.GetCameraYMin(), GameController.GetCameraYMax()));
			mainTransform = transformsList[0];
		}

		//TODO make it work with multiple transforms
		foreach (Transform transform in transformsList) {
			float obstacleVerticalSize = GameObjectUtil.GetGameObjectVerticalSize(transform.gameObject);
			float obstacleMinReach = transform.position.y - obstacleVerticalSize / 2;
			float obstacleMaxReach = obstacleMinReach + obstacleVerticalSize;

			List<(float, float)> newAvailableSpaces = new List<(float, float)>(availableSpaces);

			// Alter available spaces with each iteration
			availableSpaces.Clear();
			foreach ((float, float) availableSpace in newAvailableSpaces) {

				bool unchanged = true;
				// If obstacle minimum reach is inside space, decrease space
				if (obstacleMinReach > availableSpace.Item1 && obstacleMinReach < availableSpace.Item2) {
					availableSpaces.Add((availableSpace.Item1, obstacleMinReach));
					//Debug.Log("Became " + (availableSpace.Item1, obstacleMinReach));
					unchanged = false;
				}

				// If obstacle minimum reach is inside space, decrease space
				if (obstacleMaxReach > availableSpace.Item1 && obstacleMaxReach < availableSpace.Item2) {
					availableSpaces.Add((obstacleMaxReach, availableSpace.Item2));
					//Debug.Log("Became " + (obstacleMaxReach, availableSpace.Item2));
					unchanged = false;
				}

				// If obstacle contains space, remove it
				if (obstacleMinReach <= availableSpace.Item1 && obstacleMaxReach >= availableSpace.Item2) {
					//Debug.Log("Removed ");
					unchanged = false;
				}

				// If it remains unchanged, readd
				if (unchanged) {
					availableSpaces.Add((availableSpace.Item1, availableSpace.Item2));
					//Debug.Log("Remained " + (availableSpace.Item1, availableSpace.Item2));
				}
			}
		}

		if (availableSpaces.Count > 0) {
			return (mainTransform, availableSpaces);
		}

		return (null, null);
	}

	private bool CheckIfEnoughOverlap((Transform, List<(float, float)>) previousSpaces, (Transform, List<(float, float)>) nextSpaces) {
		float playerShipSize = GameObjectUtil.GetGameObjectVerticalSize(PlayerController.controller.GetSpaceship());

		// For every vertical space available between the two lists, check if there's at least one with enough overlap
		foreach ((float, float) previousSpace in previousSpaces.Item2) {
			foreach ((float, float) nextSpace in nextSpaces.Item2) {
				float minimumCommonSpace = Mathf.Min(previousSpace.Item2, nextSpace.Item2) - Mathf.Max(previousSpace.Item1, nextSpace.Item1);
				if (minimumCommonSpace > playerShipSize * 2) {
					return true;
				}
			}
		}

		return false;
	}

	private void SpawnSimpleRandom(float curSpawnPosition) {
		// Roll random chances to define whether there will be 1 to 4 blocks
		float chance = Random.Range(0, 99.99f);

		if (chance <= chanceOf4Blocks) {
			// Create 4 block pattern
			CreateElementsPattern(curSpawnPosition, 4);
		}
		else if (chance <= chanceOf3Blocks) {
			// Create 3 block pattern
			CreateElementsPattern(curSpawnPosition, 3);
		}
		else if (chance <= chanceOf2Blocks) {
			// Create 2 block pattern
			CreateElementsPattern(curSpawnPosition, 2);
		}
		else {
			// Simply add one block
			CreateElementsPattern(curSpawnPosition, 1);
		}
	}

	private void SpawnBlocks(float curSpawnPosition) {
		// TODO Find better ways to spawn only blocks
		CreateElementsPattern(curSpawnPosition, 5);
	}

	// Check if the distance between transformPosition and every transform on transformListToTest is at least threshold
	private bool EnoughDistanceToTransformsList(Vector3 transformPosition, List<Transform> transformListToTest, float threshold) {
		foreach (Transform transformToTest in transformListToTest) {
			if (Vector3.Distance(transformPosition, transformToTest.position) < threshold) {
				return false;
			}
		}
		return true;
	}


	private void CreateElementsPattern(float positionX, int numElements) {
        GameObject foregroundPrefab = DefineNewForegroundElement();
		float blockVerticalSize = GameObjectUtil.GetGameObjectVerticalSize(foregroundPrefab);

		int elementsSpawned = 0;

		List<(float, float)> availableSpaces = new List<(float, float)>();
		float minPositionY = GameController.GetCameraYMin() + blockVerticalSize / 2;
		float maxPositionY = GameController.GetCameraYMax() - blockVerticalSize / 2;

		availableSpaces.Add((minPositionY, maxPositionY));

		while (elementsSpawned < numElements) {
			// Choose between available spaces
			if (availableSpaces.Count == 0) {
				// No available space
				return;
			}
			(float, float) availableSpace = availableSpaces[Random.Range(0, availableSpaces.Count)];

			float positionY = Random.Range(availableSpace.Item1, availableSpace.Item2);
			elementsSpawned++;

			(bool, GameObject) spawned = SpawnForegroundElement(foregroundPrefab, new Vector3(positionX, positionY, 0), GameObjectUtil.GenerateRandomRotation());
			if (spawned.Item1) {
				// Remove item from available spaces list
				availableSpaces.Remove(availableSpace);

				// Generate two new items for available spaces list
				// Item 1
				minPositionY = availableSpace.Item1;
				maxPositionY = positionY - blockVerticalSize - MIN_VERT_SPACE_BETWEEN_BLOCKS;

				// Check if a next element will be generated
				if (elementsSpawned < numElements) {
					// Define element
					foregroundPrefab = DefineNewForegroundElement();
					blockVerticalSize = GameObjectUtil.GetGameObjectVerticalSize(foregroundPrefab);

					// Check if the new spaces fit a block
					if (maxPositionY - minPositionY > blockVerticalSize) {
						availableSpaces.Add((minPositionY, maxPositionY));
					}

					// Item 2
					minPositionY = positionY + blockVerticalSize + MIN_VERT_SPACE_BETWEEN_BLOCKS;
					maxPositionY = availableSpace.Item2;

					// Check if the new spaces fit a block
					if (maxPositionY - minPositionY > blockVerticalSize) {
						availableSpaces.Add((minPositionY, maxPositionY));
					}
				}

				// Check if it is a moving object
				if (spawned.Item2.GetComponent<MovingObjectActivator>() != null) {
					// TODO Making so every moving object is a shaky one
					//if (GameController.RollChance(50)) {
						// Chance of moving object start delayed
						//if (GameController.RollChance(50)) {
							spawned.Item2.GetComponent<MovingObjectActivator>().ActivationDelay = 0.75f;
							// TODO Improve this, possibly with more prefabs
							spawned.Item2.AddComponent<ShakyObject>();
						//}
						spawned.Item2.GetComponent<MovingObjectActivator>().enabled = true;
					//}
				}
			}
		}
	}

	// Returns whether element was succesfully spawned
	private (bool, GameObject) SpawnForegroundElement(GameObject foregroundPrefab, Vector3 position, Quaternion rotation,
		bool randomizedX = true) {
		if (randomizedX) {
			// Add randomness to the horizontal axis
			float cameraLengthFraction = (GameController.GetCameraXMax() - GameController.GetCameraXMin()) / 4;
			position = new Vector3(position.x + Random.Range(0, cameraLengthFraction), position.y, position.z);
		}

        // Spawn element
        GameObject newForegroundElement = (GameObject)Instantiate(foregroundPrefab, position, new Quaternion(0, 0, 0, 1));
		newForegroundElement.transform.localRotation = rotation;

		//// Check if bound overlap
		//foreach (GameObject block in GameObject.FindGameObjectsWithTag("Block")) {
		//	if (block != newForegroundElement) {
		//		if (newForegroundElement.GetComponent<Collider2D>().bounds.Intersects(block.GetComponent<Collider2D>().bounds)) {
		//			Debug.Log("Element delete " + newForegroundElement.name);
  //                  Destroy(newForegroundElement);
		//			return (false, null);
		//		}
		//	}
		//} 
			
		return (true, newForegroundElement);
	}

	private GameObject DefineNewForegroundElement() {
        // Keep reference
        GameObject newForegroundElement = null;

		//TODO improve obstacle/energy choosing
		if (GameController.RollChance(obstacleSpawnChance)) {
			GameObject obstaclePrefab = ChooseObstaclePrefab();
			newForegroundElement = obstaclePrefab;
		}
		else {
			// TODO Find another way to define chance of energy prefab
			if (GameController.controller.GetCurrentDay() == 15) {
				return subtractBlockPrefab;
            }
			// Define each block
			switch (Random.Range(0, 2)) {
				case 0:
					newForegroundElement = addBlockPrefab;
					break;

				case 1:
					newForegroundElement = subtractBlockPrefab;
					break;
			}
		}

		return newForegroundElement;
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

			case StageMoment.OPERATION_BLOCK_GALORE:
				nextSpawnTimer = Random.Range(minSpawnInterval, (maxSpawnInterval + minSpawnInterval) /2);
				break;

		}
	}

	public void IncreaseNextSpawnTimer(float amountToIncrease) {
		nextSpawnTimer += amountToIncrease;
		// By raising the increase time, abandon current obstacle formation control
		if (StageController.controller.GetCurrentMomentState() == StageMoment.OBSTACLE_GALORE) {
			currentObstacleControl.Clear();
		}
	}

	private GameObject ChooseObstaclePrefab() {
		// TODO Choose from available obstacle types
		int completeChance = obstacleSpawnChancePool[obstacleSpawnChancePool.Count - 1].Item2;

		int randomChoice = Mathf.RoundToInt(1 + Random.Range(0, 1.0f) * (completeChance - 1));

		int spawnType = GetTypeFromPoolChance(randomChoice);
		switch (spawnType) {
			case DEBRIS_TYPE:
				return debrisPrefabList[Random.Range(0, debrisPrefabList.Count)];
			case METEOR_TYPE:
				return meteorPrefabList[Random.Range(0, meteorPrefabList.Count)];
			case STRAY_ENGINE_TYPE:
				return strayEnginePrefabList[Random.Range(0, strayEnginePrefabList.Count)];
		}
		// TODO remove
		Debug.LogError("Invalid obstacle type " + spawnType + "..." + System.String.Join(",",obstacleSpawnChancePool));
		return null;
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
	public void SetChanceOf2Blocks(float chanceOf2Blocks) {
		this.chanceOf2Blocks = chanceOf2Blocks;
    }

	public void SetChanceOf3Blocks(float chanceOf3Blocks) {
		this.chanceOf3Blocks = chanceOf3Blocks;
    }
	
	public void SetChanceOf4Blocks(float chanceOf4Blocks) {
		this.chanceOf4Blocks = chanceOf4Blocks;
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
