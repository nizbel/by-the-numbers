using UnityEngine;
using System.Collections.Generic;

public class StoryStageController : StageController {

    // Stage moments
    List<StageMoment> startingMomentsList = new List<StageMoment>();
    List<StageMoment> gameplayMomentsList = new List<StageMoment>();
    List<StageMoment> endingMomentsList = new List<StageMoment>();

	private float playableMomentsDuration = 0;

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
		showScore = false;

		// Calculate playable moments duration for the stage
		CalculatePlayableMomentsDuration();
	}

	// Update is called once per frame
	void Update() {
		if (state != GAME_OVER_STATE) {
			// Control stage moments
			ControlMoments();
		} else {
			if (gameOverTimer > 0) {
				gameOverTimer -= Time.unscaledDeltaTime;
				TimeController.controller.SetTimeScale(Mathf.Lerp(0.1f, Time.timeScale, gameOverTimer / GAME_OVER_DURATION));
            } else {
				GameOver();
            }
        }
	}

	private void ControlMoments() {
		// Check if current moment is still valid
		if (currentMoment == null || Time.time > currentMoment.GetStartTime() + currentMoment.GetDurationInSeconds()) {
			// Check which list has the next moment
			if (startingMomentsList.Count > 0) {
				LoadCurrentMoment(startingMomentsList);
			}
			else if (gameplayMomentsList.Count > 0) {
				if (state == STARTING_STATE) {
					state = GAMEPLAY_STATE;
					ScreenFadeController.controller.StartFadeIn();
				}
				LoadCurrentMoment(gameplayMomentsList);
			}
			else if (endingMomentsList.Count > 0) {
				// Call fade out as soon as ending starts
				if (state == GAMEPLAY_STATE) {
					state = ENDING_STATE;
					ScreenFadeController.controller.StartFadeOut();
				}
				LoadCurrentMoment(endingMomentsList);
			}
			else {
				// Day over (Story mode)
				NarratorController.controller.GameOver();

				// Update game info
				GameController.controller.UpdateDayInfoSuccess(score);

				// Progress through days
				gameObject.AddComponent<CurrentDayController>();
			}
		}
		// Check if current moment still has speeches
		else if (currentMoment.speeches.Count > 0 && NarratorController.controller.GetState() == NarratorController.QUIET) {
			NarratorController.controller.StartMomentSpeech(currentMoment.speeches[0]);
			currentMoment.speeches.RemoveAt(0);
		}
	}

	// Set day info for story mode
	public void LoadCurrentDayData() {
		// Get current day
		int currentDay = GameController.controller.GetCurrentDay();

		// Load day data from JSON
		var jsonFileStageParts = Resources.Load<TextAsset>(PATH_JSON_MOMENTS + currentDay + "/data");
		DayData dayData = JsonUtility.FromJson<DayData>(jsonFileStageParts.text);

		// Check if a new element is seen on this day
		ElementsEnum newElement = CheckForNewElement(dayData.GetElementsInDay(), GameController.GetGameInfo().elementsSeen);
		if (newElement > 0) {
			// Load specific event for the new element
			LoadSpecificElementEvent(newElement);
        }

		LoadMoments(dayData);

		// Set constellation spawning chance
		//if (GameController.RollChance(dayData.constellationChance)) {
		if (GameController.RollChance(100)) {
			BackgroundStateController.controller.PrepareConstellationSpawn();
		}

		// Prepare charges
		currentSpecialCharges = Mathf.RoundToInt(currentDay * 1.3f + Random.Range(3.2f, 4.8f));

		if (dayData.startingValueRange != 0) {
			ValueRange.controller.SetMinValue(dayData.startingValueRange - ValueRange.INTERVAL);
			ValueRange.controller.SetMaxValue(dayData.startingValueRange + ValueRange.INTERVAL);
		}

		if (dayData.startingShipValue != 0) {
			PlayerController.controller.SetValue(dayData.startingShipValue);
			PlayerController.controller.UpdateEnergyBar();
		}
	}

	private void LoadMoments(DayData dayData) {
		startingMomentsList.AddRange(dayData.startingMoments);
		gameplayMomentsList.AddRange(dayData.gameplayMoments);
		endingMomentsList.AddRange(dayData.endingMoments);

		// Calculate each moments duration
		foreach (StageMoment moment in startingMomentsList) {
			moment.CalculateDurationInSeconds();
		}
		foreach (StageMoment moment in gameplayMomentsList) {
			moment.CalculateDurationInSeconds();
		}
		foreach (StageMoment moment in endingMomentsList) {
			moment.CalculateDurationInSeconds();
		}

		ControlMoments();
	}

	private void LoadCurrentMoment(List<StageMoment> momentList) {
		// Set current moment as first of the list
		currentMoment = momentList[0];

		// Remove it from list
		momentList.RemoveAt(0);

		// Set moment's start time
		currentMoment.SetStartTime(Time.time);

		// If moment has speech, pass it to Narrator Controller
		if (currentMoment.speeches.Count > 0) {
			NarratorController.controller.StartMomentSpeech(currentMoment.speeches[0]);
			currentMoment.speeches.RemoveAt(0);
		}

		// Send spawn chances to ForegroundController
		ForegroundController.controller.SetEnergySpawnChances(currentMoment.energySpawnChances);

		ForegroundController.controller.SetObstacleSpawnChances(currentMoment.obstacleSpawnChance, currentMoment.obstacleChancesByType);

		ForegroundController.controller.SetSpawnInterval(currentMoment.spawnInterval);

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

		// Calculate remaining playable duration for moments
		CalculatePlayableMomentsDuration();
	}

	public override void SkipCutscenes() {
		// Check if current moment is cutscene
		if (currentMoment.type == StageMoment.TYPE_CUTSCENE) {
			currentMoment.SetStartTime(Time.time - currentMoment.GetDurationInSeconds()-1);
			NarratorController.controller.StopSpeech();
			// Look for next cutscenes
			if (startingMomentsList.Count > 0) {
				while (startingMomentsList.Count > 0 && startingMomentsList[0].type == StageMoment.TYPE_CUTSCENE) {
					startingMomentsList.RemoveAt(0);
                }
				// If every element at starting list was removed, look in the next moments list
				while (startingMomentsList.Count == 0 && gameplayMomentsList[0].type == StageMoment.TYPE_CUTSCENE) {
					gameplayMomentsList.RemoveAt(0);
				}
            } else if (gameplayMomentsList.Count > 0) {
				while (gameplayMomentsList.Count > 0 && gameplayMomentsList[0].type == StageMoment.TYPE_CUTSCENE) {
					gameplayMomentsList.RemoveAt(0);
				}
				// If every element at gameplay list was removed, look in the next moments list
				while (gameplayMomentsList.Count == 0 && endingMomentsList[0].type == StageMoment.TYPE_CUTSCENE) {
					endingMomentsList.RemoveAt(0);
				}
			} else if (endingMomentsList.Count > 0) {
				while (endingMomentsList.Count > 0 && endingMomentsList[0].type == StageMoment.TYPE_CUTSCENE) {
					endingMomentsList.RemoveAt(0);
				}
			}
		}
	}

	private void CalculatePlayableMomentsDuration() {
		// Restart duration for 0 or current moment duration, if applicable
		if (currentMoment.type != StageMoment.TYPE_CUTSCENE && currentMoment.momentState != StageMoment.NO_SPAWN) {
			playableMomentsDuration = GetCurrentMomentDuration();
		} else {
			playableMomentsDuration = 0;
        }

		foreach (StageMoment moment in gameplayMomentsList){
			if (moment.type != StageMoment.TYPE_CUTSCENE && moment.momentState != StageMoment.NO_SPAWN) {
				playableMomentsDuration += moment.GetDurationInSeconds();
            }
        }
	}

	// Calculate time left of gameplay in which there can be spawns
	public override float TimeLeftBeforeNoSpawn() {
		if (currentMoment.momentState == StageMoment.NO_SPAWN) {
			return 0;
        } else {
			float timeLeft = currentMoment.GetDurationInSeconds() - (Time.time - currentMoment.GetStartTime());

			// Add in the next moment
			StageMoment nextMoment = GetNextMoment();
			if (nextMoment != null && nextMoment.momentState != StageMoment.NO_SPAWN) {
				timeLeft += nextMoment.GetDurationInSeconds();
            }

			return timeLeft;
        }
    }

	private StageMoment GetNextMoment() {
		if (startingMomentsList.Count > 0) {
			return startingMomentsList[0];
		}
		else if (gameplayMomentsList.Count > 0) {
			return gameplayMomentsList[0];
		}
		else if (endingMomentsList.Count > 0) {
			return endingMomentsList[0];
		}
		return null;
	}

	// Returns 0 if all the elements that can be seen on stage were already found before
	ElementsEnum CheckForNewElement(List<ElementsEnum> currentElements, bool[] elementsSeen) {
		foreach (ElementsEnum element in currentElements) {
			int elementValue = (int) element;
			if (!elementsSeen[elementValue - 1]) {
				return element;
            }
        }
		return 0;
	}

	void LoadSpecificElementEvent(ElementsEnum newElement) { 
		// TODO Prepare code to load event to know new element
	}

	/*
	 * Getters and Setters
	 */
	public override float GetPlayableMomentsDuration() {
		return playableMomentsDuration;
	}

}
