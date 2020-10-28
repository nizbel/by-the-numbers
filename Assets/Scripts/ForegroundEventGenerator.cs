using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForegroundEventGenerator : MonoBehaviour
{
	/* 
	 * Constants
	 */
	public const float DEFAULT_MIN_SPAWN_INTERVAL = 0.4f;
	public const float DEFAULT_MAX_SPAWN_INTERVAL = 0.6f;

	private const int ENERGY_FORMATION_TYPE = 1;
	private const int OBSTACLE_GENERATOR_TYPE = 2;
	private const int TURBINE_GROUP_TYPE = 3;
	
	public const float DEFAULT_MAX_EVENT_SPAWN_CHANCE = 50;
	public const int DEFAULT_ENERGY_FORMATION_SPAWN_CHANCE = 65;
	public const int DEFAULT_OBSTACLE_GENERATOR_SPAWN_CHANCE = 30;
	public const int DEFAULT_TURBINE_GROUP_SPAWN_CHANCE = 5;

	private int energyFormationChance = DEFAULT_ENERGY_FORMATION_SPAWN_CHANCE;
	private int obstacleGeneratorChance = DEFAULT_OBSTACLE_GENERATOR_SPAWN_CHANCE;
	private int turbineGroupSpawnChance = DEFAULT_TURBINE_GROUP_SPAWN_CHANCE;

	// Keeps (type, chance) list
	private List<(int, int)> spawnChancePool = new List<(int,int)>();
	/*
	 * Energy formation prefabs
	 */
	public List<GameObject> energyFormationList;

	/*
	 * Obstacle generator prefabs
	 */
	public List<GameObject> obstacleGeneratorPrefabList;

	// Start is called before the first frame update
	void Start()
    {
		PrepareChancesPool();
	}

    // Update is called once per frame
    void Update()
    {
        
    }

    // Spawn
    public void SpawnEvent() {
		// TODO check if events have representants
		int completeChance = spawnChancePool[spawnChancePool.Count-1].Item2;

		int randomChoice = Mathf.RoundToInt(1 + Random.Range(0, 1.0f) * (completeChance - 1));

		int type = GetTypeFromPoolChance(randomChoice);

		Debug.Log(string.Join(",", spawnChancePool));
		Debug.Log(type + "..." + randomChoice);

		bool eventSpawned = false;

		// TODO Improve choosing formation
		// Check if spawned will be a formation or obstacle generator
		if (type == ENERGY_FORMATION_TYPE) {
			// Define current spawning position
			float curSpawnPosition = ForegroundController.SPAWN_CAMERA_OFFSET + GameController.GetCameraXMax();

			GameObject energyFormation = energyFormationList[Random.Range(0, energyFormationList.Count)];
			//GameObject energyFormation = energyFormationList[2];

			float formationScreenOffset = energyFormation.GetComponent<Formation>().GetScreenOffset();
			(bool, GameObject) spawnedFormation = SpawnForegroundElement(energyFormation,
				new Vector3(curSpawnPosition + formationScreenOffset, Random.Range(-1, 1), 0),
				GameObjectUtil.GenerateRandomRotation());

			// Check if spawned to count on stage's special spawning charges
			if (spawnedFormation.Item1) {
				ForegroundController.controller.EventSpawned(spawnedFormation.Item2.GetComponent<ForegroundEvent>());
				//Debug.Log(currentSpecialSpawnChance);
				eventSpawned = true;
			}
		}
		else if (type == OBSTACLE_GENERATOR_TYPE) {
			// Spawn obstacle generator
			GameObject obstacleGenerator = obstacleGeneratorPrefabList[Random.Range(0, obstacleGeneratorPrefabList.Count)];

			// Define a radial position from the middle horizontal right
			float angle = Random.Range(-0.25f, 0.25f) * Mathf.PI;
			float x = GameController.GetCameraXMax() + Mathf.Cos(angle) * GameController.GetCameraYMax();
			float y = Mathf.Sin(angle) * GameController.GetCameraYMax();
			Vector3 spawnPosition = new Vector3(x, y, 0);

			(bool, GameObject) spawnedGeneration = SpawnForegroundElement(obstacleGenerator,
				spawnPosition, new Quaternion(0, 0, 0, 1), false);

			// Check if spawned to count on stage's special spawning charges
			if (spawnedGeneration.Item1) {
				// Add duration to generator
				TimedDurationObject durationScript = spawnedGeneration.Item2.AddComponent<TimedDurationObject>();
				durationScript.Duration = 5;
				durationScript.WaitTime = 1.2f;
				// Make meteor generator activate after wait time
				spawnedGeneration.Item2.GetComponent<MeteorGenerator>().enabled = false;
				durationScript.AddOnWaitListener(spawnedGeneration.Item2.GetComponent<MeteorGenerator>().Enable);

				// Play warning on panel
				StageController.controller.PanelWarnDanger();

				ForegroundController.controller.EventSpawned(spawnedGeneration.Item2.GetComponent<ForegroundEvent>());

				//Debug.Log(currentSpecialSpawnChance);
				eventSpawned = true;
			}
		}

		// If an event has really spawned, recalculate pool chances
		if (eventSpawned) {
			// Remove elements that have a cost above what's available
			RemoveUnavailableEvents();

			// Prepare chances
			PrepareChancesPool();
		}
	}

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

		// Check if bound overlap
		if (newForegroundElement.GetComponent<Formation>() != null) {
			// Get children of formation
			foreach (Transform child in newForegroundElement.transform) {
				foreach (GameObject block in GameObject.FindGameObjectsWithTag("Block")) {
					if (block.transform.parent != newForegroundElement.transform) {
						if (child.GetComponent<Collider2D>().bounds.Intersects(block.GetComponent<Collider2D>().bounds)) {
							Destroy(newForegroundElement);
							return (false, null);
						}
					}
				}
			}
		}

		return (true, newForegroundElement);
	}

	void PrepareChancesPool() {
		spawnChancePool.Clear();
		// Energy formation
		if (energyFormationList.Count > 0) {
			AddChanceToPool(ENERGY_FORMATION_TYPE, energyFormationChance);
		}

		// Obstacle generator
		if (obstacleGeneratorPrefabList.Count > 0) {
			AddChanceToPool(OBSTACLE_GENERATOR_TYPE, obstacleGeneratorChance);
		}
	}

	void AddChanceToPool(int spawnType, int chance) {
		if (spawnChancePool.Count > 0) {
			spawnChancePool.Add((spawnType, spawnChancePool[spawnChancePool.Count - 1].Item2 + chance));
		}
		else {
			spawnChancePool.Add((spawnType, chance));
		}
	}

	// Returns the type chosen based on the chance
	int GetTypeFromPoolChance(int chance) {
		foreach ((int, int) spawnChance in spawnChancePool) {
			if (spawnChance.Item2 > chance) {
				return spawnChance.Item1;
            }
        }
		return 0;
    }

	void RemoveUnavailableEvents() {
		RemoveEventsWithImpossibleCost(energyFormationList);
		RemoveEventsWithImpossibleCost(obstacleGeneratorPrefabList);
	}

	void RemoveEventsWithImpossibleCost(List<GameObject> eventsList) {
		for (int i = eventsList.Count - 1; i >= 0; i--) {
			ForegroundEvent currentEvent = eventsList[i].GetComponent<ForegroundEvent>();

			if (currentEvent.GetChargesCost() > StageController.controller.GetCurrentSpecialCharges()) {
				eventsList.RemoveAt(i);
			}
		}
	}
}
