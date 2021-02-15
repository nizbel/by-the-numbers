using UnityEngine;
using System.IO;
using UnityEngine.Experimental.Rendering.Universal;
using System.Collections.Generic;

public class InfiniteStageController : StageController {

    // Stage moments
    List<StageMoment> gameplayMomentsList = new List<StageMoment>();

    // Use this for initialization
    void Start() {
        // Start object pooling
        objectPool.SetActive(true);

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
                TimeController.controller.SetTimeScale(Mathf.Lerp(0.2f, Time.timeScale, gameOverTimer / GAME_OVER_DURATION));
            }
            else {
                GameOver();
            }
        }
    }

    private void ControlMoments() {
        // Check if current moment is still valid
        if (Time.time > currentMoment.GetStartTime() + currentMoment.duration) {
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
        // Load day data
        DayData dayData = GetComponent<CurrentDayController>().GetDayData(currentDay);

        gameplayMomentsList.AddRange(dayData.gameplayMoments);

        LoadCurrentMoment(gameplayMomentsList);
    }

    private void LoadCurrentMoment(List<StageMoment> momentList) {
        // Set current moment as first of the list
        currentMoment = momentList[0];

        // Remove it from list
        momentList.RemoveAt(0);

        // Check if moment is playable
        if (currentMoment.type != MomentTypeEnum.Gameplay || (currentMoment.momentState == MomentSpawnStateEnum.NoSpawn && currentMoment.specialEvent == 0)) {
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

        //// If moment has speech, pass it to Narrator Controller
        //if (currentMoment.speeches.Count > 0) {
        //    NarratorController.controller.StartMomentSpeech(currentMoment.speeches[0]);
        //}

        // Send spawn chances to ForegroundController
        ForegroundController.controller.SetEnergySpawnChances(currentMoment.energySpawnChances);

        ForegroundController.controller.SetElementsSpawnChance(currentMoment.elementsSpawnChance);

        // If moment has magnetic barriers, keep track
        bool shouldSpawnMagneticBarriers = currentMoment.HasMagneticBarriers();
        if (shouldSpawnMagneticBarriers != magneticBarriersSpawning) {
            magneticBarriersSpawning = shouldSpawnMagneticBarriers;

            ForegroundController.controller.SetSpawnMagneticBarriers(magneticBarriersSpawning);
        }

        // If moment has special event, load the controller for it
        if (currentMoment.specialEventObject != null) {
            // Create special event controller object
            Instantiate(currentMoment.specialEventObject);
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
