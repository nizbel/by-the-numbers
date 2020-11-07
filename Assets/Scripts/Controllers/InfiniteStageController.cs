using UnityEngine;
using System.IO;
using UnityEngine.Experimental.Rendering.Universal;
using System.Collections.Generic;

public class InfiniteStageController : StageController {

    // Stage moments
    List<StageMoment> gameplayMomentsList = new List<StageMoment>();

    // Use this for initialization
    void Start() {
		// Set starting state as gameplay
		state = GAMEPLAY_STATE;

        ScreenFadeController.controller.StartFadeIn();

        // Start narrator controller
        NarratorController.controller.StartGame();

        // Load data for random day
        LoadMoments(ChooseDayAtRandom());

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
		if ((PlayerController.controller.GetValue() < ValueRange.controller.GetMinValue()) ||
			(PlayerController.controller.GetValue() > ValueRange.controller.GetMaxValue())) {
			DestroyShip();
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

        // Control stage moments
        ControlMoments();
    }

    private void ControlMoments() {
        // Check if current moment is still valid
        if (Time.time > currentMoment.GetStartTime() + currentMoment.GetDurationInSeconds()) {
            LoadCurrentMoment(gameplayMomentsList);

            // If moments list is empty, reload list
            if (gameplayMomentsList.Count == 0) {
                LoadMoments(ChooseDayAtRandom());
            }

            // TODO improve logic
            // Replenish special charges
            currentSpecialCharges += 3;
            Debug.Log("replenish charges");
        }
    }

    private void LoadMoments(int currentDay) {
        var jsonFileStageParts = Resources.Load<TextAsset>(PATH_JSON_MOMENTS + currentDay + "/gameplay");
        gameplayMomentsList.AddRange(JsonUtil.FromJson<StageMoment>(jsonFileStageParts.text));

        LoadCurrentMoment(gameplayMomentsList);
    }

    private void LoadCurrentMoment(List<StageMoment> momentList) {
        // Set current moment as first of the list
        currentMoment = momentList[0];

        // Remove it from list
        momentList.RemoveAt(0);

        // Check if moment is playable
        if (currentMoment.type != StageMoment.TYPE_GAMEPLAY || currentMoment.momentState == StageMoment.NO_SPAWN) {
            if (gameplayMomentsList.Count > 0) {
                LoadCurrentMoment(gameplayMomentsList);
                return;
            }
            else {
                LoadMoments(ChooseDayAtRandom());
                return;
            }
        }

        // Set moment's start time
        currentMoment.SetStartTime(Time.time);
        currentMoment.CalculateDurationInSeconds();

        //// If moment has speech, pass it to Narrator Controller
        //if (currentMoment.speeches.Count > 0) {
        //    NarratorController.controller.StartMomentSpeech(currentMoment.speeches[0]);
        //}

        // If moment has range changers, keep track
        if (currentMoment.hasRangeChangers && !rangeChangersSpawning) {
            lastRangeChangerSpawned = Time.timeSinceLevelLoad;
            DefineRangeChangerSpawn();
            nextRangeChangerPositive = DefineNextRangeChangerType();
            rangeChangersSpawning = true;
        }
        else if (!currentMoment.hasRangeChangers && rangeChangersSpawning) {
            rangeChangersSpawning = false;
        }
    }

    private int ChooseDayAtRandom() {
        // TODO Find a way to get the pool of days
        int currentDay = Random.Range(1, 3);

        return currentDay;
    }
}
