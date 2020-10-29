using UnityEngine;
using System.IO;
using UnityEngine.Experimental.Rendering.Universal;
using System.Collections.Generic;

public class StoryStageController : StageController {

    // Stage events
    List<StageEvent> startingEventsList = new List<StageEvent>();
    List<StageEvent> gameplayEventsList = new List<StageEvent>();
    List<StageEvent> endingEventsList = new List<StageEvent>();

	// Show text for cutscene skipping
	private GameObject skipCutsceneText = null;

	private int playableMomentsDuration = 0;

	// Use this for initialization
	void Start() {
		// Start narrator controller
		NarratorController.controller.StartGame();

		// Load skip cutscene text
		skipCutsceneText = GameObject.Find("Skip Cutscene Text").gameObject;
		// Hide it on start
		skipCutsceneText.SetActive(false);

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
			// Game Over
			if ((PlayerController.controller.GetValue() < ValueRange.rangeController.GetMinValue()) ||
				(PlayerController.controller.GetValue() > ValueRange.rangeController.GetMaxValue())) {
				DestroyShip();
			}

			// Check if range changer can still spawn
			if (state == GAMEPLAY_STATE && rangeChangersSpawning) {
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
		} else {
			if (gameOverTimer > 0) {
				gameOverTimer -= Time.unscaledDeltaTime;
				Time.timeScale = Mathf.Lerp(1, 0.1f, gameOverTimer / GAME_OVER_DURATION);
            } else {
				GameOver();
            }
        }
	}

	private void ControlEvents() {
		// Check if current event is still valid
		if (currentEvent == null || Time.time > currentEvent.GetStartTime() + currentEvent.GetDurationInSeconds()) {
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

				// Update game info
				StageInfo stageInfo = GameController.GetGameInfo().GetStageInfoByDay(GameController.controller.GetCurrentDay());
				stageInfo.played = true;
				stageInfo.UpdateHighScore(score);
				GameController.controller.Save();

				// Progress through days
				gameObject.AddComponent<CurrentDayController>();
			}
		}

		// Check if current event still has speeches
		if (currentEvent.speeches.Count > 0 && NarratorController.controller.GetState() == NarratorController.QUIET) {
			NarratorController.controller.StartEventSpeech(currentEvent.speeches[0]);
			currentEvent.speeches.RemoveAt(0);
		}
	}

	// Set day info for story mode
	public void LoadCurrentDayData() {
		// Get current day
		int currentDay = GameController.controller.GetCurrentDay();

		// Load data from JSON
		LoadEvents(currentDay);

		// Prepare charges
		currentSpecialCharges = Mathf.RoundToInt(currentDay * 1.3f + Random.Range(3.2f, 4.8f));
	}

	private void LoadEvents(int currentDay) {
		var jsonFileStageParts = Resources.Load<TextAsset>(PATH_JSON_EVENTS + currentDay + "/starting");
		startingEventsList.AddRange(JsonUtil.FromJson<StageEvent>(jsonFileStageParts.text));

		jsonFileStageParts = Resources.Load<TextAsset>(PATH_JSON_EVENTS + currentDay + "/gameplay");
		gameplayEventsList.AddRange(JsonUtil.FromJson<StageEvent>(jsonFileStageParts.text));

		jsonFileStageParts = Resources.Load<TextAsset>(PATH_JSON_EVENTS + currentDay + "/ending");
		endingEventsList.AddRange(JsonUtil.FromJson<StageEvent>(jsonFileStageParts.text));

		// Calculate each moments duration
		foreach (StageEvent moment in startingEventsList) {
			moment.CalculateDurationInSeconds();
		}
		foreach (StageEvent moment in gameplayEventsList) {
			moment.CalculateDurationInSeconds();
		}
		foreach (StageEvent moment in endingEventsList) {
			moment.CalculateDurationInSeconds();
		}

		ControlEvents();
	}

	private void LoadCurrentEvent(List<StageEvent> eventList) {
		// Set current event as first of the list
		currentEvent = eventList[0];

		// Remove it from list
		eventList.RemoveAt(0);

		// Set event's start time
		currentEvent.SetStartTime(Time.time);

		// If event has speech, pass it to Narrator Controller
		if (currentEvent.speeches.Count > 0) {
			NarratorController.controller.StartEventSpeech(currentEvent.speeches[0]);
			currentEvent.speeches.RemoveAt(0);
		}

		// If event has energy spawn chances, send it to ForegroundController
		if (currentEvent.energySpawnChances != null) {
			ForegroundController.controller.SetEnergySpawnChances(currentEvent.energySpawnChances);
        } else {
			ForegroundController.controller.SetDefaultEnergySpawnChances();
		}

		if (currentEvent.obstacleSpawnChances != 0) { 
			ForegroundController.controller.SetObstacleSpawnChances(currentEvent.obstacleSpawnChances);
		} else {
			ForegroundController.controller.SetDefaultObstacleSpawnChances();
		}

		// If event has range changers, keep track
		if (currentEvent.hasRangeChangers && !rangeChangersSpawning) {
			lastRangeChangerSpawned = Time.timeSinceLevelLoad;
			DefineRangeChangerSpawn();
			nextRangeChangerPositive = DefineNextRangeChangerType();
			rangeChangersSpawning = true;
		} else if (!currentEvent.hasRangeChangers && rangeChangersSpawning) {
			rangeChangersSpawning = false;
        }

		// If current event is a cutscene, show skipping text
		skipCutsceneText.SetActive(currentEvent.type == StageEvent.TYPE_CUTSCENE && GameController.GetGameInfo().StagePlayed(GameController.controller.GetCurrentDay()));

		// If event has special event, load the controller for it
		if (currentEvent.specialEvent != 0) {
			// Create special event controller object
			// TODO fix fixed string
			Instantiate(Resources.Load("Prefabs/Special Events/Special Event Controller Day " + GameController.controller.GetCurrentDay()));
		}

		// Calculate remaining playable duration for events
		CalculatePlayableMomentsDuration();
	}

	public override void SkipCutscenes() {
		// Check if current event is cutscene
		if (currentEvent.type == StageEvent.TYPE_CUTSCENE) {
			currentEvent.SetStartTime(Time.time - currentEvent.GetDurationInSeconds());
			NarratorController.controller.StopSpeech();
			// Look for next cutscenes
			if (startingEventsList.Count > 0) {
				while (startingEventsList.Count > 0 && startingEventsList[0].type == StageEvent.TYPE_CUTSCENE) {
					startingEventsList.RemoveAt(0);
                }
				// If every element at starting list was removed, look in the next events list
				while (startingEventsList.Count == 0 && gameplayEventsList[0].type == StageEvent.TYPE_CUTSCENE) {
					gameplayEventsList.RemoveAt(0);
				}
            } else if (gameplayEventsList.Count > 0) {
				while (gameplayEventsList.Count > 0 && gameplayEventsList[0].type == StageEvent.TYPE_CUTSCENE) {
					gameplayEventsList.RemoveAt(0);
				}
				// If every element at gameplay list was removed, look in the next events list
				while (gameplayEventsList.Count == 0 && endingEventsList[0].type == StageEvent.TYPE_CUTSCENE) {
					endingEventsList.RemoveAt(0);
				}
			} else if (endingEventsList.Count > 0) {
				while (endingEventsList.Count > 0 && endingEventsList[0].type == StageEvent.TYPE_CUTSCENE) {
					endingEventsList.RemoveAt(0);
				}
			}
		}
	}

	private void CalculatePlayableMomentsDuration() {
		// Restart duration for 0 or current event duration, if applicable
		if (currentEvent.type != StageEvent.TYPE_CUTSCENE && currentEvent.eventState != StageEvent.NO_SPAWN) {
			playableMomentsDuration = GetCurrentEventDuration();
		} else {
			playableMomentsDuration = 0;
        }

		foreach (StageEvent moment in gameplayEventsList){
			if (moment.type != StageEvent.TYPE_CUTSCENE && moment.eventState != StageEvent.NO_SPAWN) {
				playableMomentsDuration += moment.GetDurationInSeconds();
            }
        }
	}

	/*
	 * Getters and Setters
	 */
	public override int GetPlayableMomentsDuration() {
		return playableMomentsDuration;
	}
}
