using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForegroundController : MonoBehaviour
{
	public const float SPAWN_CAMERA_OFFSET = 3;

	// Element generator
	private ForegroundElementGenerator elementGenerator = null;

	// Event generator
	private ForegroundEventGenerator eventGenerator = null;
	private float lastEventSpawnTime = 0;
	private bool applyEventsCooldown = true;
	private float nextEventSpawnCheck = 0;

	public static ForegroundController controller;

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
	void Start()
    {
		DefineNextEventSpawnCheck();

		if (GameController.controller.GetState() == GameController.GAMEPLAY_STORY) {
			// TODO At the start of every day, define the possible spawns for each generator
		}
	}


	// Update is called once per frame
	void Update()
    {
		if (StageController.controller.GetCurrentMomentState() != StageMoment.NO_SPAWN && StageController.controller.GetCurrentSpecialCharges() > 0) {
			if (nextEventSpawnCheck <= 0) {
				if (ShouldSpawnEvent()) {
					eventGenerator.SpawnEvent(StageController.controller.TimeLeftBeforeNoSpawn());
				}
				DefineNextEventSpawnCheck();
			}
			else {
				nextEventSpawnCheck -= Time.deltaTime;
			}
		}
	}

	private bool ShouldSpawnEvent() {
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

		return GameController.RollChance(currentEventSpawnChance);
	}

	public void EventSpawned(ForegroundEvent foregroundEvent) {
		if (applyEventsCooldown) {
			elementGenerator.IncreaseNextSpawnTimer(foregroundEvent.GetCooldown());
        }
		lastEventSpawnTime = Time.time;
		StageController.controller.UseSpecialCharges(foregroundEvent.GetChargesCost());
	}

	void DefineNextEventSpawnCheck() {
		nextEventSpawnCheck = Random.Range(ForegroundEventGenerator.DEFAULT_MIN_SPAWN_INTERVAL, ForegroundEventGenerator.DEFAULT_MAX_SPAWN_INTERVAL);
	}

	public void SetEnergySpawnChances(int[] chances) {
		if (StageController.controller.GetCurrentMomentState() != StageMoment.NO_SPAWN) {
			// Check if element generator should be active
			if (!elementGenerator.enabled) {
				elementGenerator.enabled = true;
			}

			if (chances != null) {
				elementGenerator.SetChanceOf4Blocks(chances[0]);
				elementGenerator.SetChanceOf3Blocks(chances[1]);
				elementGenerator.SetChanceOf2Blocks(chances[2]);
			}
			else {
				elementGenerator.SetChanceOf4Blocks(ForegroundElementGenerator.DEFAULT_CHANCE_OF_4_BLOCKS);
				elementGenerator.SetChanceOf3Blocks(ForegroundElementGenerator.DEFAULT_CHANCE_OF_3_BLOCKS);
				elementGenerator.SetChanceOf2Blocks(ForegroundElementGenerator.DEFAULT_CHANCE_OF_2_BLOCKS);
			}
		}
	}

 
	public void SetObstacleSpawnChances(float chance, int[] chancesByType) {
		if (StageController.controller.GetCurrentMomentState() != StageMoment.NO_SPAWN) {
			if (chance == -1) {
				elementGenerator.SetObstacleSpawnChance(ForegroundElementGenerator.DEFAULT_OBSTACLE_SPAWN_CHANCE);
				SetObstacleSpawnChancesByType(chancesByType);
			}
			else {
				elementGenerator.SetObstacleSpawnChance(chance);
				if (chance > 0) {
					SetObstacleSpawnChancesByType(chancesByType);
				}
			}
		}
	}

	void SetObstacleSpawnChancesByType(int[] chancesByType) {
		if (chancesByType != null) {
			elementGenerator.SetDebrisSpawnChance(chancesByType[0]);
			elementGenerator.SetMeteorSpawnChance(chancesByType[1]);
			elementGenerator.SetStrayEngineSpawnChance(chancesByType[2]);
		} else {
			elementGenerator.SetDebrisSpawnChance(ForegroundElementGenerator.DEFAULT_DEBRIS_SPAWN_CHANCE);
			elementGenerator.SetMeteorSpawnChance(ForegroundElementGenerator.DEFAULT_METEOR_SPAWN_CHANCE);
			elementGenerator.SetStrayEngineSpawnChance(ForegroundElementGenerator.DEFAULT_STRAY_ENGINE_SPAWN_CHANCE);
		}
	}
}
