using UnityEngine;
using System.IO;
using UnityEngine.Experimental.Rendering.Universal;
using System.Collections.Generic;

public class StoryStageController : StageController {

    // Stage moments
    List<StageMoment> startingMomentsList = new List<StageMoment>();
    List<StageMoment> gameplayMomentsList = new List<StageMoment>();
    List<StageMoment> endingMomentsList = new List<StageMoment>();

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
			if ((PlayerController.controller.GetValue() < ValueRange.controller.GetMinValue()) ||
				(PlayerController.controller.GetValue() > ValueRange.controller.GetMaxValue())) {
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

			// Control stage moments
			ControlMoments();
		} else {
			if (gameOverTimer > 0) {
				gameOverTimer -= Time.unscaledDeltaTime;
				Time.timeScale = Mathf.Lerp(1, 0.1f, gameOverTimer / GAME_OVER_DURATION);
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
				StageInfo stageInfo = GameController.GetGameInfo().GetStageInfoByDay(GameController.controller.GetCurrentDay());
				stageInfo.played = true;
				stageInfo.UpdateHighScore(score);
				GameController.controller.Save();

				// Progress through days
				gameObject.AddComponent<CurrentDayController>();
			}
		}

		// Check if current moment still has speeches
		if (currentMoment.speeches.Count > 0 && NarratorController.controller.GetState() == NarratorController.QUIET) {
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

		LoadMoments(dayData);

		// Prepare charges
		currentSpecialCharges = Mathf.RoundToInt(currentDay * 1.3f + Random.Range(3.2f, 4.8f));

		if (dayData.startingShipValue != 0) {
			PlayerController.controller.SetValue(dayData.startingShipValue);
			PlayerController.controller.UpdateEnergyBar();
		}

		if (dayData.startingValueRange != 0) {
			ValueRange.controller.SetMinValue(dayData.startingValueRange - ValueRange.INTERVAL);
			ValueRange.controller.SetMaxValue(dayData.startingValueRange + ValueRange.INTERVAL);
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

		// If moment has range changers, keep track
		if (currentMoment.hasRangeChangers && !rangeChangersSpawning) {
			lastRangeChangerSpawned = Time.timeSinceLevelLoad;
			DefineRangeChangerSpawn();
			nextRangeChangerPositive = DefineNextRangeChangerType();
			rangeChangersSpawning = true;
		} else if (!currentMoment.hasRangeChangers && rangeChangersSpawning) {
			rangeChangersSpawning = false;
        }

		// If current moment is a cutscene, show skipping text
		skipCutsceneText.SetActive(currentMoment.type == StageMoment.TYPE_CUTSCENE && GameController.GetGameInfo().StagePlayed(GameController.controller.GetCurrentDay()));

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
			currentMoment.SetStartTime(Time.time - currentMoment.GetDurationInSeconds());
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
			float timeLeft = Time.time - (currentMoment.GetDurationInSeconds() + currentMoment.GetStartTime());

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

	/*
	 * Getters and Setters
	 */
	public override int GetPlayableMomentsDuration() {
		return playableMomentsDuration;
	}

}
