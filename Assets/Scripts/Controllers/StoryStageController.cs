using UnityEngine;
using System.IO;
using UnityEngine.Experimental.Rendering.Universal;
using System.Collections.Generic;

public class StoryStageController : StageController {

    //// Stage events
    List<StageEvent> startingEventsList = new List<StageEvent>();
    List<StageEvent> gameplayEventsList = new List<StageEvent>();
    List<StageEvent> endingEventsList = new List<StageEvent>();

    // Use this for initialization
    void Start() {
		// Start narrator controller
		NarratorController.controller.StartGame();

		// Load data for the day
		LoadCurrentDayData();

		// Get player objects
		playerShipTransform = PlayerController.controller.transform;

        // Get score object
        scoreText = GameObject.FindGameObjectWithTag("Score").GetComponent<TextMesh>();

		// Keep track for range changer spawning
		lastRangeChangerSpawned = Time.timeSinceLevelLoad;
		DefineRangeChangerSpawn();
		nextRangeChangerPositive = DefineNextRangeChangerType();
	}

	// Update is called once per frame
	void Update() {
		// Game Over
		if ((PlayerController.controller.GetValue() < ValueRange.rangeController.GetMinValue()) ||
			(PlayerController.controller.GetValue() > ValueRange.rangeController.GetMaxValue())) {
			GameOver();
		}

		// Check if range changer can still spawn
		if (state == GAMEPLAY_STATE) {
			// Check if should warn about range changer
			if (!rangeChangerWarned && Time.timeSinceLevelLoad - lastRangeChangerSpawned > currentRangeChangerSpawnTimer - WARNING_PERIOD_BEFORE_RANGE_CHANGER) {
				WarnAboutRangeChanger();
			}

			// Check if range changer should be spawned
			else if (Time.timeSinceLevelLoad - lastRangeChangerSpawned > currentRangeChangerSpawnTimer) {
				SpawnRangeChanger();
			}
		}

		// Control stage events
		ControlEvents();
	}

	private void ControlEvents() {
		// Check if current event is still valid
		if (Time.time > currentEvent.GetStartTime() + currentEvent.GetDurationInSeconds()) {
			// Check which list has the next event
			if (startingEventsList.Count > 0) {
				LoadCurrentEvent(startingEventsList);
			}
			else if (gameplayEventsList.Count > 0) {
				if (state == STARTING_STATE) {
					state = GAMEPLAY_STATE;
					ScreenFadeController.controller.StartFadeIn();
				}
				LoadCurrentEvent(gameplayEventsList);
			}
			else if (endingEventsList.Count > 0) {
				// Call fade out as soon as ending starts
				if (state == GAMEPLAY_STATE) {
					state = ENDING_STATE;
					ScreenFadeController.controller.StartFadeOut();
				}
				LoadCurrentEvent(endingEventsList);
			}
			else {
				// Day over (Story mode)
				NarratorController.controller.GameOver();
				// TODO Add day calculator object
				if (GameController.controller.GetCurrentDay() == 1) {
					GameController.controller.SetCurrentDay(2);
					GameController.controller.ChangeState(GameController.GAMEPLAY_STORY);
				} else {
					GameController.controller.ChangeState(GameController.GAME_OVER_STORY);
				}
			}
		}
	}

	// Set day info for story mode
	public void LoadCurrentDayData() {
		// Get current day
		int currentDay = GameController.controller.GetCurrentDay();

		// Load data from JSON
		LoadEvents(currentDay);

		// Prepare charges
		currentSpecialCharges = currentDay * 4;
	}

	private void LoadEvents(int currentDay) {
		var jsonFileStageParts = Resources.Load<TextAsset>(PATH_JSON_EVENTS + currentDay + "/starting");
		startingEventsList.AddRange(JsonUtil.FromJson<StageEvent>(jsonFileStageParts.text));

		jsonFileStageParts = Resources.Load<TextAsset>(PATH_JSON_EVENTS + currentDay + "/gameplay");
		gameplayEventsList.AddRange(JsonUtil.FromJson<StageEvent>(jsonFileStageParts.text));

		jsonFileStageParts = Resources.Load<TextAsset>(PATH_JSON_EVENTS + currentDay + "/ending");
		endingEventsList.AddRange(JsonUtil.FromJson<StageEvent>(jsonFileStageParts.text));

		LoadCurrentEvent(startingEventsList);
	}

	private void LoadCurrentEvent(List<StageEvent> eventList) {
		// Set current event as first of the list
		currentEvent = eventList[0];

		// Remove it from list
		eventList.RemoveAt(0);

		// Set event's start time
		currentEvent.SetStartTime(Time.time);
		currentEvent.CalculateDurationInSeconds();

		// If event has speech, pass it to Narrator Controller
		if (currentEvent.speeches.Count > 0) {
			NarratorController.controller.StartEventSpeech(currentEvent.speeches[0]);
		}
	}
}
