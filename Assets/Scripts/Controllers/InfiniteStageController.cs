﻿using UnityEngine;
using System.IO;
using UnityEngine.Experimental.Rendering.Universal;
using System.Collections.Generic;

public class InfiniteStageController : StageController {

    // Stage events
    List<StageEvent> gameplayEventsList = new List<StageEvent>();

    // Use this for initialization
    void Start() {
		// Set starting state as gameplay
		state = GAMEPLAY_STATE;

        ScreenFadeController.controller.StartFadeIn();

        // Start narrator controller
        NarratorController.controller.StartGame();

        // Load data for random day
        LoadEvents(ChooseDayAtRandom());

        // Get player objects
		playerShipTransform = PlayerController.controller.transform;

        // Get score object
        scoreText = GameObject.FindGameObjectWithTag("Score").GetComponent<TextMesh>();
        showScore = true;

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
        if (rangeChangersSpawning) {
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
            LoadCurrentEvent(gameplayEventsList);

            // If events list is empty, reload list
            if (gameplayEventsList.Count == 0) {
                LoadEvents(ChooseDayAtRandom());
            }

            // TODO improve logic
            // Replenish special charges
            currentSpecialCharges += 3;
            Debug.Log("replenish charges");
        }
    }

    private void LoadEvents(int currentDay) {
        var jsonFileStageParts = Resources.Load<TextAsset>(PATH_JSON_EVENTS + currentDay + "/gameplay");
        gameplayEventsList.AddRange(JsonUtil.FromJson<StageEvent>(jsonFileStageParts.text));

        LoadCurrentEvent(gameplayEventsList);
    }

    private void LoadCurrentEvent(List<StageEvent> eventList) {
        // Set current event as first of the list
        currentEvent = eventList[0];

        // Remove it from list
        eventList.RemoveAt(0);

        // Check if event is playable
        if (currentEvent.type != StageEvent.TYPE_GAMEPLAY || currentEvent.eventState == StageEvent.NO_SPAWN) {
            if (gameplayEventsList.Count > 0) {
                LoadCurrentEvent(gameplayEventsList);
                return;
            }
            else {
                LoadEvents(ChooseDayAtRandom());
                return;
            }
        }

        // Set event's start time
        currentEvent.SetStartTime(Time.time);
        currentEvent.CalculateDurationInSeconds();

        //// If event has speech, pass it to Narrator Controller
        //if (currentEvent.speeches.Count > 0) {
        //    NarratorController.controller.StartEventSpeech(currentEvent.speeches[0]);
        //}

        // If event has range changers, keep track
        if (currentEvent.hasRangeChangers && !rangeChangersSpawning) {
            lastRangeChangerSpawned = Time.timeSinceLevelLoad;
            DefineRangeChangerSpawn();
            nextRangeChangerPositive = DefineNextRangeChangerType();
            rangeChangersSpawning = true;
        }
        else if (!currentEvent.hasRangeChangers && rangeChangersSpawning) {
            rangeChangersSpawning = false;
        }
    }

    private int ChooseDayAtRandom() {
        // TODO Find a way to get the pool of days
        int currentDay = Random.Range(1, 3);

        return currentDay;
    }
}
