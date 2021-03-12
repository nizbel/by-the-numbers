using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForegroundController : MonoBehaviour
{
	// TODO Parameterize spawn periods for element generator

	public const float SPAWN_CAMERA_OFFSET = 3;

	// Element generator
	private ForegroundElementGenerator elementGenerator = null;

	// Event generator
	private ForegroundEventGenerator eventGenerator = null;
	private float lastEventSpawnTime = 0;
	private float nextEventSpawnCheck = 0;

	public static ForegroundController controller;

	// Additional timer for events
	// TODO Move event spawning logic to foreground event controller, just like the elements
	List<float> additionalSpawnTimer = new List<float>();

	void Awake() {
		if (controller == null) {
			controller = this;

			// Define event generator
			eventGenerator = GetComponent<ForegroundEventGenerator>();

			// Define element generator
			elementGenerator = GetComponent<ForegroundElementGenerator>();
		}
		else {
			Destroy(gameObject);
		}
	}

	// Start is called before the first frame update
	void Start() {
        DefineNextEventSpawnCheck();
    }

    public void DefineAvailableEventsForDay(DayData dayData) {
        if (GameController.controller.GetState() == GameController.GAMEPLAY_STORY) {
            eventGenerator.DefineAvailableEventsForDay(dayData);
        }
        // TODO For infinite, the choice of possible events is at every day choice
    }

    public void StartEventGenerator() {
		// Start event generation coroutine
		StartCoroutine(SpawnEvent());
	}

	IEnumerator SpawnEvent() {
		while (eventGenerator.eventsList.Count > 0) {
			// Wait
			yield return new WaitForSeconds(nextEventSpawnCheck);
			while (additionalSpawnTimer.Count > 0) {
				float waitTime = additionalSpawnTimer[0];
				additionalSpawnTimer.RemoveAt(0);
				yield return new WaitForSeconds(waitTime);
			}
			// Proceed with spawning
			if (ShouldSpawnEvent()) {
				eventGenerator.SpawnEvent(StageController.controller.TimeLeftBeforeNoSpawn());
			}
			DefineNextEventSpawnCheck();
		}
	}

	private bool ShouldSpawnEvent() {
		if (StageController.controller.GetCurrentMomentState() == MomentSpawnStateEnum.NoSpawn
			|| StageController.controller.GetCurrentMomentIsElementEncounter()) { 
			return false;
        }

		// Check if event should spawn
		float currentEventSpawnChance;

		// TODO Improve the verification of special spawning, getting all spawnable moments
		if (lastEventSpawnTime > 0) {
			float currentDuration = StageController.controller.GetPlayableMomentsDuration();
			currentEventSpawnChance = Mathf.Lerp(0, ForegroundEventGenerator.DEFAULT_MAX_EVENT_SPAWN_CHANCE,
				(Time.time - lastEventSpawnTime) / currentDuration);
		}
		else {
			currentEventSpawnChance = Mathf.Lerp(0, ForegroundEventGenerator.DEFAULT_MAX_EVENT_SPAWN_CHANCE,
				(Time.time - StageController.controller.GetCurrentMomentStartTime()) / StageController.controller.GetPlayableMomentsDuration());
		}

		// TODO Remove this test
		//bool should = GameController.RollChance(currentEventSpawnChance);
		//if (should) {
		//	Debug.Log("Current spawn chance: " + currentEventSpawnChance);
  //      }
		//return should;
		return GameController.RollChance(currentEventSpawnChance);
	}

	public void EventSpawned(ForegroundEvent foregroundEvent) {
		elementGenerator.IncreaseNextSpawnTimer(foregroundEvent.GetDelay() + foregroundEvent.GetCooldown());
		// Check if it should be applied to events
		if (foregroundEvent.GetApplyDelayToEvents()) {
			additionalSpawnTimer.Add(foregroundEvent.GetDelay());
        }
		if (foregroundEvent.GetApplyCooldownToEvents()) {
			additionalSpawnTimer.Add(foregroundEvent.GetCooldown());
		}

		lastEventSpawnTime = Time.time;
		StageController.controller.UseSpecialCharges(foregroundEvent.GetChargesCost());
	}

	void DefineNextEventSpawnCheck() {
		nextEventSpawnCheck = Random.Range(ForegroundEventGenerator.DEFAULT_MIN_SPAWN_INTERVAL, ForegroundEventGenerator.DEFAULT_MAX_SPAWN_INTERVAL);
	}

	public void SetEnergySpawnChances(int[] chances) {
		// TODO Find a better way to enable element generator outside of this method
		if (StageController.controller.GetCurrentMomentState() != MomentSpawnStateEnum.NoSpawn) {
			// Check if element generator should be active
			if (!elementGenerator.enabled) {
				elementGenerator.enabled = true;
			}

			if (chances.Length > 0) {
				elementGenerator.SetChanceOf4Energies(chances[0]);
				elementGenerator.SetChanceOf3Energies(chances[1]);
				elementGenerator.SetChanceOf2Energies(chances[2]);
			}
			else {
				elementGenerator.SetChanceOf4Energies(ForegroundElementGenerator.DEFAULT_CHANCE_OF_4_ENERGIES);
				elementGenerator.SetChanceOf3Energies(ForegroundElementGenerator.DEFAULT_CHANCE_OF_3_ENERGIES);
				elementGenerator.SetChanceOf2Energies(ForegroundElementGenerator.DEFAULT_CHANCE_OF_2_ENERGIES);
			}
		}
	}
 
	public void SetElementsSpawnChance(List<ElementSpawnChance> elementsSpawnChance) {
		if (StageController.controller.GetCurrentMomentState() != MomentSpawnStateEnum.NoSpawn) {
			elementGenerator.SetElementsSpawnChance(elementsSpawnChance);

			// Set elements that should be possible at the current moment
			eventGenerator.PrepareChancesPool();
		}
	}

	public void SetSpawnInterval(SpawnIntervalEnum spawnInterval) {
		elementGenerator.SetSpawnInterval(spawnInterval);
    }

	public void SetMovingElementSpawnChance(float movingElementSpawnChance) {
		elementGenerator.SetMovingElementSpawnChance(movingElementSpawnChance);
	}

	public void SetSpawnMagneticBarriers(bool shouldSpawn) {
		if (shouldSpawn) {
			elementGenerator.StartMagneticBarriersSpawn();
        } else {
			elementGenerator.StopMagneticBarriersSpawn();
		}
    }

	public void SetSpawnMagneticBarriersInterval(DifficultyEnum difficulty) {
		elementGenerator.SetMagneticBarrierSpawnInterval(difficulty);
    }
}
