using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockSpawner : MonoBehaviour {

	private const float DEFAULT_MIN_SPAWN_INTERVAL = 0.3f;
	private const float DEFAULT_MAX_SPAWN_INTERVAL = 0.95f;
	private const float SPAWN_CAMERA_OFFSET = 3;

	private const float CHANCE_OF_4_BLOCKS = 5f;
	private const float CHANCE_OF_3_BLOCKS = 15f;
	private const float CHANCE_OF_2_BLOCKS = 25f;

	private const float MIN_VERT_SPACE_BETWEEN_BLOCKS = 0.1f;

	/*
	 * Block prefabs
	 */
	public GameObject addBlockPrefab;
	public GameObject subtractBlockPrefab;
	public GameObject multiplyBlockPrefab;
	public GameObject divideBlockPrefab;

	/*
	 * Power Up prefabs
	 */
	public GameObject neutralizerPrefab;
	public GameObject growthPrefab;

	/*
	 * Obstacle prefabs
	 */
	public GameObject obstaclePrefab;

	Transform player;

	// Spawn control
	float lastSpawn;
	float nextSpawnTimer;

	// Use this for initialization
	void Start () {
		player = StageController.controller.GetPlayerTransform();

		lastSpawn = Time.timeSinceLevelLoad;
		nextSpawnTimer = lastSpawn + Random.Range(DEFAULT_MIN_SPAWN_INTERVAL, DEFAULT_MAX_SPAWN_INTERVAL);
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.timeSinceLevelLoad > nextSpawnTimer) {

			// Define how many should be spawned
			SpawnForegroundElements();

			// Keep spawn time
			lastSpawn = Time.timeSinceLevelLoad;

            //TODO get a better way of spawning power ups
            float curSpawnPosition = SPAWN_CAMERA_OFFSET + GameController.GetCameraXMax();
            switch (Random.Range(0, 30)) {
			case 0:
				GameObject neutralizer = (GameObject) Instantiate(neutralizerPrefab, new Vector3(curSpawnPosition, Random.Range(-3.1f, 3.1f), 0), transform.rotation);
				neutralizer.transform.parent = transform; 
				break;

			case 1:
				GameObject growth = (GameObject) Instantiate(growthPrefab, new Vector3(curSpawnPosition, Random.Range(-3.1f, 3.1f), 0), transform.rotation);
				growth.transform.parent = transform; 
				break;
			}

			// Modify spawn timer randomly
			nextSpawnTimer = lastSpawn + Random.Range(DEFAULT_MIN_SPAWN_INTERVAL, DEFAULT_MAX_SPAWN_INTERVAL);
		}
	}

	private void SpawnForegroundElements() {
		float curSpawnPosition = SPAWN_CAMERA_OFFSET + GameController.GetCameraXMax();

		int currentState = 0;

		switch(currentState) {
			case 0:
				SpawnSimpleRandom(curSpawnPosition);
				break;

            case 1:
                SpawnObstacles();
                break;
        }

	}

	private void SpawnObstacles(float curSpawnPosition) {
		
	}

	private void SpawnSimpleRandom(float curSpawnPosition) {
		// Roll random chances to define whether there will be 1 to 4 blocks
		if (GameController.RollChance(CHANCE_OF_4_BLOCKS)) {
			// Create 4 block pattern
			CreateElementsPattern(curSpawnPosition, 4);
		}
		else if (GameController.RollChance(CHANCE_OF_3_BLOCKS)) {
			// Create 3 block pattern
			CreateElementsPattern(curSpawnPosition, 3);
		}
		else if (GameController.RollChance(CHANCE_OF_2_BLOCKS)) {
			// Create 2 block pattern
			CreateElementsPattern(curSpawnPosition, 2);
		}
		else {
			// Simply add one block
			CreateElementsPattern(curSpawnPosition, 1);
		}
	}

	private float GetGameObjectVerticalSize(GameObject gameObj) {
		return gameObj.GetComponent<SpriteRenderer>().sprite.bounds.extents.y * 2 * gameObj.transform.localScale.y;
	}

    private void CreateElementsPattern(float positionX, int numElements) {
		GameObject foregroundPrefab = DefineNewForegroundElement();
		float blockVerticalSize = GetGameObjectVerticalSize(foregroundPrefab);

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

			bool spawned = SpawnForegroundElement(foregroundPrefab, new Vector3(positionX, positionY, 0), transform.rotation);
			if (spawned) {
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
					blockVerticalSize = GetGameObjectVerticalSize(foregroundPrefab);

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
			}
		}
    }

	// Returns whether element was succesfully spawned
    private bool SpawnForegroundElement(GameObject foregroundPrefab, Vector3 position, Quaternion rotation, bool randomizedX = true) {
		if (randomizedX) {
			// Add randomness to the horizontal axis
			float cameraLengthFraction = (GameController.GetCameraXMax() - GameController.GetCameraXMin()) / 4;
			position = new Vector3(position.x + Random.Range(0, cameraLengthFraction), position.y, position.z);
		}

		// Spawn element
		GameObject newForegroundElement = (GameObject)Instantiate(foregroundPrefab, position, rotation);
		newForegroundElement.transform.parent = transform;

		// Check if bound overlap
		foreach (GameObject block in GameObject.FindGameObjectsWithTag("Block")) {
			if (block != newForegroundElement) {
				if (newForegroundElement.GetComponent<Collider2D>().bounds.Intersects(block.GetComponent<Collider2D>().bounds)) {
					Destroy(newForegroundElement);
					return false;
				}
			}
		}
		return true;
	}

	private GameObject DefineNewForegroundElement() {
		// Keep reference
		GameObject newForegroundElement = null;

		//TODO improve this
		if (GameController.RollChance(20)) {
			newForegroundElement = obstaclePrefab;
		}
		else {
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
}
