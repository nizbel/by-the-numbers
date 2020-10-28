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
		}
		else {
			Destroy(gameObject);
		}
	}

	// Start is called before the first frame update
	void Start()
    {
		// Define event generator
		eventGenerator = GetComponent<ForegroundEventGenerator>();

		DefineNextEventSpawnCheck();

		// Define element generator
		elementGenerator = GetComponent<ForegroundElementGenerator>();

		if (GameController.controller.GetState() == GameController.GAMEPLAY_STORY) {
			// TODO At the start of every day, define the possible spawns for each generator
		}
	}


	// Update is called once per frame
	void Update()
    {
		if (StageController.controller.GetCurrentEventState() != StageEvent.NO_SPAWN) {
			if (StageController.controller.GetCurrentSpecialCharges() > 0 && nextEventSpawnCheck <= 0) {
				if (ShouldSpawnEvent()) {
					eventGenerator.SpawnEvent();
				}
				DefineNextEventSpawnCheck();
			}
			else {
				nextEventSpawnCheck -= Time.deltaTime;
			}

			// Check if element generator should be active
			if (!elementGenerator.enabled) {
				elementGenerator.enabled = true;
			}
		}
	}

	private bool ShouldSpawnEvent() {
		// Check if event should spawn
		float currentEventSpawnChance;
			
		// TODO Improve the verification of special spawning, getting all spawnable moments
		if (lastEventSpawnTime > 0) {
			float currentDuration = StageController.controller.GetCurrentEventDuration() - (lastEventSpawnTime - StageController.controller.GetCurrentEventStartTime());
			currentEventSpawnChance = Mathf.Lerp(0, ForegroundEventGenerator.DEFAULT_MAX_EVENT_SPAWN_CHANCE,
				(Time.time - lastEventSpawnTime) / currentDuration);
		}
		else {
			currentEventSpawnChance = Mathf.Lerp(0, ForegroundEventGenerator.DEFAULT_MAX_EVENT_SPAWN_CHANCE,
				(Time.time - StageController.controller.GetCurrentEventStartTime()) / StageController.controller.GetCurrentEventDuration());
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
}
