using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockSpawner : MonoBehaviour {

	private const float MIN_SPAWN_INTERVAL = 0.3f;
	private const float MAX_SPAWN_INTERVAL = 0.95f;
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
	List<Vector3> nextSpawnPattern = new List<Vector3>();

	// Use this for initialization
	void Start () {
		player = StageController.controller.GetPlayerTransform();

		lastSpawn = Time.timeSinceLevelLoad;
		nextSpawnTimer = lastSpawn + Random.Range(MIN_SPAWN_INTERVAL, MAX_SPAWN_INTERVAL);
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.timeSinceLevelLoad > nextSpawnTimer) {

			// Define how many should be spawned
			DefineSpawnPattern();

			// Generate a block for each defined position in the pattern
			nextSpawnPattern.ForEach(GenerateRandomizedBlock);
			nextSpawnPattern.Clear();

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

			//TODO improve this
			if (GameController.RollChance(10)) {
				GameObject obstacle = (GameObject)Instantiate(obstaclePrefab, new Vector3(curSpawnPosition, Random.Range(-3.1f, 3.1f), 0), transform.rotation);
				obstacle.transform.parent = transform;
			}

			// Modify spawn timer randomly
			nextSpawnTimer = lastSpawn + Random.Range(MIN_SPAWN_INTERVAL, MAX_SPAWN_INTERVAL);
		}
	}

	private void DefineSpawnPattern() {
		float curSpawnPosition = SPAWN_CAMERA_OFFSET + GameController.GetCameraXMax();

		// Roll random chances to define whether there will be 1 to 4 blocks
		if (GameController.RollChance(CHANCE_OF_4_BLOCKS)) {
			// Create 4 block pattern
			CreateMultipleBlockPattern(curSpawnPosition, 4).ForEach(nextSpawnPattern.Add);
        } else if (GameController.RollChance(CHANCE_OF_3_BLOCKS)) {
			// Create 3 block pattern
			CreateMultipleBlockPattern(curSpawnPosition, 3).ForEach(nextSpawnPattern.Add);
		} else if (GameController.RollChance(CHANCE_OF_2_BLOCKS)) {
			// Create 2 block pattern
			CreateMultipleBlockPattern(curSpawnPosition, 2).ForEach(nextSpawnPattern.Add);
		} else {
			// Simply add one block
			nextSpawnPattern.Add(new Vector3(curSpawnPosition, 
				Random.Range(GameController.GetCameraYMin() + addBlockPrefab.GetComponent<SpriteRenderer>().sprite.bounds.extents.y, 
				GameController.GetCameraYMax() - addBlockPrefab.GetComponent<SpriteRenderer>().sprite.bounds.extents.y), 
				0));
		}
	}

    private List<Vector3> CreateMultipleBlockPattern(float positionX, int numBlocks) {
		float blockVerticalSize = addBlockPrefab.GetComponent<SpriteRenderer>().sprite.bounds.extents.y * 2;

		List<Vector3> blockList = new List<Vector3>();

		List<(float, float)> availableSpaces = new List<(float, float)>();
        float minPositionY = GameController.GetCameraYMin() + blockVerticalSize / 2;
        float maxPositionY = GameController.GetCameraYMax() - blockVerticalSize / 2;

		availableSpaces.Add((minPositionY, maxPositionY));

        while (blockList.Count < numBlocks) {
			// Choose between available spaces
			(float, float) availableSpace = availableSpaces[Random.Range(0, availableSpaces.Count)];

            float positionY = Random.Range(availableSpace.Item1, availableSpace.Item2);
			blockList.Add(new Vector3(positionX, positionY, 0));

			// Remove item from available spaces list
			availableSpaces.Remove(availableSpace);

			// Generate two new items for available spaces list
			// Item 1
			minPositionY = availableSpace.Item1;
			maxPositionY = positionY - blockVerticalSize - MIN_VERT_SPACE_BETWEEN_BLOCKS;
			
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

        return blockList;
    }

    private void GenerateRandomizedBlock(Vector3 position) {
		// Define each block
		switch (Random.Range(0, 2)) {
			case 0:
				GameObject newAddBlock = (GameObject)Instantiate(addBlockPrefab, position, transform.rotation);
				newAddBlock.transform.parent = transform;
				break;

			case 1:
				GameObject newSubtractBlock = (GameObject)Instantiate(subtractBlockPrefab, position, transform.rotation);
				newSubtractBlock.transform.parent = transform;
				break;
		}
	}
}
