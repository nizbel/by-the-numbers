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
	}

	// Update is called once per frame
	void Update() {
        if (state != GAME_OVER_STATE) {
            // Control stage moments
            ControlMoments();
        } else {
            if (gameOverTimer > 0) {
                gameOverTimer -= Time.unscaledDeltaTime;
                Time.timeScale = Mathf.Lerp(1, 0.1f, gameOverTimer / GAME_OVER_DURATION);
            }
            else {
                GameOver();
            }
        }
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
        var jsonFileStageParts = Resources.Load<TextAsset>(PATH_JSON_MOMENTS + currentDay + "/data");
        DayData dayData = JsonUtility.FromJson<DayData>(jsonFileStageParts.text);

        gameplayMomentsList.AddRange(dayData.gameplayMoments);

        LoadCurrentMoment(gameplayMomentsList);
    }

    private void LoadCurrentMoment(List<StageMoment> momentList) {
        // Set current moment as first of the list
        currentMoment = momentList[0];

        // Remove it from list
        momentList.RemoveAt(0);

        // Check if moment is playable
        if (currentMoment.type != StageMoment.TYPE_GAMEPLAY || (currentMoment.momentState == StageMoment.NO_SPAWN && currentMoment.specialEvent == 0)) {
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

        // Send spawn chances to ForegroundController
        ForegroundController.controller.SetEnergySpawnChances(currentMoment.energySpawnChances);

        ForegroundController.controller.SetObstacleSpawnChances(currentMoment.obstacleSpawnChance, currentMoment.elementsSpawnChance);

        // If moment has magnetic barriers, keep track
        if (currentMoment.hasMagneticBarriers != magneticBarriersSpawning) {
            magneticBarriersSpawning = currentMoment.hasMagneticBarriers;

            ForegroundController.controller.SetSpawnMagneticBarriers(magneticBarriersSpawning);
        }

        // If moment has special event, load the controller for it
        if (currentMoment.specialEvent != 0) {
            // Create special event controller object
            // TODO fix fixed string
            Instantiate(Resources.Load("Prefabs/Special Events/Special Event Controller Day " + GameController.controller.GetCurrentDay()));
        }
    }

    private int ChooseDayAtRandom() {
        // TODO Find a way to get the pool of days
        List<int> availableDays = CurrentDayController.GetDaysAvailable();
        int currentDayIndex = Random.Range(0, availableDays.Count);

        GameController.controller.SetCurrentDay(availableDays[currentDayIndex]);

        return availableDays[currentDayIndex];
    }
}
