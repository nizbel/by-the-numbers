using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForegroundEventGenerator : MonoBehaviour {
	/* 
	 * Constants
	 */
	public const float DEFAULT_MIN_SPAWN_INTERVAL = 0.4f;
	public const float DEFAULT_MAX_SPAWN_INTERVAL = 1f;

	public const float DEFAULT_MAX_EVENT_SPAWN_CHANCE = 50;

	private const int SAME_DIFFICULTY_EVENT_SPAWN_CHANCE = 6;
	private const int HIGHER_DIFFICULTY_EVENT_SPAWN_CHANCE = 2;
	private const int LOWER_DIFFICULTY_EVENT_SPAWN_CHANCE = 1;

	// Keeps (index, chance) list
	private List<(int, int)> spawnChancePool = new List<(int, int)>();
	/*
	 * Energy formation prefabs
	 */
	public List<GameObject> energyFormationList;

	public List<EventData> eventsList;

	// Spawn
	public void SpawnEvent(float timeAvailableForSpawn) {
		// Choose event at random
        int completeChance = spawnChancePool[spawnChancePool.Count - 1].Item2;

        int randomChoice = Random.Range(1, completeChance+1);

        int chosenIndex = GetIndexFromPoolChance(randomChoice);
        EventData chosenEvent = eventsList[chosenIndex];

		// TODO Find a better way to insert time remaining verification
        if (timeAvailableForSpawn >= chosenEvent.duration) {
			GameObject newEvent = GameObject.Instantiate(chosenEvent.foregroundEvent);
			chosenEvent.FillEventWithData(newEvent);
		}
		// Remove elements that have a cost above what's available
		bool eventRemoved = RemoveUnavailableEvents(chosenEvent, timeAvailableForSpawn);

		// Recalculate pool chances
		if (eventRemoved) {
			PrepareChancesPool();
		}
	}

	// Spawn
	public void SpawnEventOld(float timeAvailableForSpawn) {
		// TODO Check if cooldown and duration fit for current moment, if next moment is a NO_SPAWN
		// Check if events have representants
		//int completeChance = spawnChancePool[spawnChancePool.Count - 1].Item2;

		//int randomChoice = Mathf.RoundToInt(1 + Random.Range(0, 1.0f) * (completeChance - 1));

		//int type = GetTypeFromPoolChance(randomChoice);

		bool eventSpawned = false;

		// TODO Find a better way to insert time remaining verification
		// Check if spawned event will be a formation or obstacle generator
			// TODO Decide where mine event should be put
			if (GameController.RollChance(20)) {
				float testSpawnPosition = ForegroundController.SPAWN_CAMERA_OFFSET + GameController.GetCameraXMax();

				GameObject test = energyFormationList[3];

				float testScreenOffset = 1.5f;
				GameObject spawnedTest = SpawnForegroundElement(test,
					new Vector3(testSpawnPosition + testScreenOffset, Random.Range(-1, 1), 0), Quaternion.Euler(0, 0, 0));

				// Check if spawned to count on stage's special spawning charges
				if (spawnedTest) {
					ForegroundController.controller.EventSpawned(spawnedTest.GetComponent<ForegroundEvent>());
					//Debug.Log(currentSpecialSpawnChance);
					eventSpawned = true;
				}
			}

		// If an event has really spawned, recalculate pool chances
		if (eventSpawned) {
			// Remove elements that have a cost above what's available
			//RemoveUnavailableEvents();

			// Prepare chances
			PrepareChancesPool();
		}
	}

	private GameObject SpawnForegroundElement(GameObject foregroundPrefab, Vector3 position, Quaternion rotation,
		bool randomizedX = true) {
		if (randomizedX) {
			// Add randomness to the horizontal axis
			float cameraLengthFraction = (GameController.GetCameraXMax() - GameController.GetCameraXMin()) / 4;
			position = new Vector3(position.x + Random.Range(0, cameraLengthFraction), position.y, position.z);
		}

		// Spawn element
		GameObject newForegroundElement = (GameObject)Instantiate(foregroundPrefab, position, new Quaternion(0, 0, 0, 1));
		newForegroundElement.transform.localRotation = rotation;

		return newForegroundElement;
	}

	void PrepareChancesPool() {
		spawnChancePool.Clear();

		for (int i = 0; i < eventsList.Count; i++) {
			EventData currentEvent = eventsList[i];
            AddChanceToPool(i, CalculateChanceByDifficulty(currentEvent.difficulty));
            //spawnChancePool.Add((i, CalculateChanceByDifficulty(currentEvent.difficulty)));
		}
	}

	void AddChanceToPool(int index, int chance) {
		if (spawnChancePool.Count > 0) {
			spawnChancePool.Add((index, spawnChancePool[spawnChancePool.Count - 1].Item2 + chance));
		}
		else {
			spawnChancePool.Add((index, chance));
		}
	}

	int CalculateChanceByDifficulty(DifficultyEnum difficulty) {
		// Get current day difficulty
		DifficultyEnum currentDifficulty = StageController.controller.GetDayData().difficulty;

		int difficultyDifference = currentDifficulty - difficulty;

		if (difficultyDifference == 0) {
			return SAME_DIFFICULTY_EVENT_SPAWN_CHANCE;
		} else if (difficultyDifference > 0) {
			return LOWER_DIFFICULTY_EVENT_SPAWN_CHANCE;
		} else {
			return HIGHER_DIFFICULTY_EVENT_SPAWN_CHANCE;
		}
    }

	// Returns the type chosen based on the chance
	int GetIndexFromPoolChance(int chance) {
		foreach ((int, int) spawnChance in spawnChancePool) {
			if (spawnChance.Item2 >= chance) {
				return spawnChance.Item1;
			}
		}

		Debug.LogError("INVALID EVENT");
		return 0;
	}

	//void RemoveUnavailableEvents() {
	//	RemoveEventsWithImpossibleCost(energyFormationList);
	//	RemoveEventsWithImpossibleCost(obstacleGeneratorPrefabList);
	//}

	bool RemoveUnavailableEvents(EventData spawnedEvent, float timeAvailableForSpawn) {
		//RemoveEventsWithImpossibleCost(spawnedEvent.chargesCost);
		// Gather new value for special event charges
		int newCurrentSpecialChanges = StageController.controller.GetCurrentSpecialCharges() - spawnedEvent.chargesCost;

		int initialEventListCount = eventsList.Count;

		// Iterate through events for removal 
		for (int i = eventsList.Count - 1; i >= 0; i--) {
			EventData currentEvent = eventsList[i];

			// Remove events with impossible cost
			if (currentEvent.chargesCost > newCurrentSpecialChanges) {
				eventsList.RemoveAt(i);
			} 
			// Remove events with impossible duration
			else if (currentEvent.duration > timeAvailableForSpawn) {
				eventsList.RemoveAt(i);
			}
		}

		return eventsList.Count != initialEventListCount;
	}

	public void DefineAvailableEventsForDay(DayData dayData) {
		// If difficulty is too far from day's, remove event
		for (int i = eventsList.Count - 1; i >= 0; i--) {
			// Difficulty too far from day difficulty 
			if (Mathf.Abs(eventsList[i].difficulty - dayData.difficulty) > 1) {
				eventsList.RemoveAt(i);
			}

			// Event starts appearing on a later day
			else if (eventsList[i].firstAppearingDay > dayData.day) {
				eventsList.RemoveAt(i);
            }
		}

		// Check for elements available in day
		List<ElementsEnum> elementsInDay = dayData.elementsInDay;
		for (int i = eventsList.Count-1; i >= 0; i--) {
			EventData currentEvent = eventsList[i];
			bool shouldRemove = false;
			foreach (ElementsEnum element in currentEvent.obligatoryElements) {
				if (!elementsInDay.Contains(element)) {
					shouldRemove = true;
					break;
                } 
            }

			// If already set for removal
			if (shouldRemove) {
				eventsList.RemoveAt(i);
			} else if (currentEvent.optionalElements.Count > 0) {
				// Has to remove unless at least one optional element is available
				shouldRemove = true;
				foreach (ElementsEnum element in currentEvent.optionalElements) {
					if (elementsInDay.Contains(element)) {
						shouldRemove = false;
						break;
					}
				}

				if (shouldRemove) {
					eventsList.RemoveAt(i);
				}
			}
		}
		// Prepare pool of chances after setting available events
		PrepareChancesPool();
	}

}
