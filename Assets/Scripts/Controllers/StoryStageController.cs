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
		// TODO Remove once going for production
		CurrentDayController testScript = gameObject.GetComponent<CurrentDayController>();
		testScript.enabled = true;
		testScript.enabled = false;

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
				TimeController.controller.SetTimeScale(Mathf.Lerp(0.2f, Time.timeScale, gameOverTimer / GAME_OVER_DURATION));
            } else {
				GameOver();
            }
        }
	}

	private void ControlMoments() {
		// Check if current moment is still valid
		if (currentMoment == null || Time.time > currentMoment.GetStartTime() + currentMoment.duration) {
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
				gameObject.GetComponent<CurrentDayController>().enabled = true;
			}
		}
		// Check if current moment still has speeches
		else if (currentMoment.momentSpeeches.Count > 0 && NarratorController.controller.GetState() == NarratorController.QUIET) {
			NarratorController.controller.StartMomentSpeech(currentMoment.momentSpeeches[0]);
			currentMoment.momentSpeeches.RemoveAt(0);
		}
	}

	// Set day info for story mode
	public void LoadCurrentDayData() {
		// Get current day
		int currentDay = GameController.controller.GetCurrentDay();

		// Load day data
		DayData dayData = GetComponent<CurrentDayController>().GetDayData(currentDay);

        // Check if a new element is seen on this day
        ElementsEnum newElement = CheckForNewElement(dayData.elementsInDay, GameController.GetGameInfo().elementsSeen);
		if (newElement > 0) {
			// Load specific event for the new element
			LoadSpecificElementEvent(newElement);
        }

		LoadMoments(dayData);

		// TODO Do the same for infinite mode
		// Enable elements generation in distant foreground
		BackgroundStateController.controller.EnableGeneratorsWithDayDataAvailable();

		// Set constellation spawning chance
		if (GameController.RollChance(dayData.constellationChance)) {
			BackgroundStateController.controller.PrepareConstellationSpawn();
		}

		// Prepare charges
		currentSpecialCharges = Mathf.RoundToInt(currentDay * 1.3f + Random.Range(3.2f, 4.8f));

		// Set values for ship and energy bar
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
		if (currentMoment.momentSpeeches.Count > 0) {
			NarratorController.controller.StartMomentSpeech(currentMoment.momentSpeeches[0]);
			currentMoment.momentSpeeches.RemoveAt(0);
		}

		// Send spawn chances to ForegroundController
		ForegroundController.controller.SetEnergySpawnChances(currentMoment.energySpawnChances);

		ForegroundController.controller.SetElementsSpawnChance(currentMoment.elementsSpawnChance);

		ForegroundController.controller.SetSpawnInterval(currentMoment.spawnInterval);

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
		} else if (currentMoment is ElementEncounterStageMoment) {
			// Load controller for element special event
			ElementSpecialEventController specialEvent = GameObject.Instantiate((currentMoment as ElementEncounterStageMoment).elementEventPrefab).GetComponent<ElementSpecialEventController>();
			specialEvent.SetStageMoment((ElementEncounterStageMoment) currentMoment);
		}

		// TODO Do the same for infinite mode
		// Send current moment distant foreground spawn data to background state controller
		BackgroundStateController.controller.UpdateDistantForegroundGenerator(currentMoment.distantForegroundSpawn, currentMoment.elementsSpawnChance);

		// Calculate remaining playable duration for moments
		CalculatePlayableMomentsDuration();
	}

	public override void SkipCutscenes() {
		// Check if current moment is cutscene
		if (currentMoment.type == MomentTypeEnum.Cutscene) {
			currentMoment.SetStartTime(Time.time - currentMoment.duration-1);
			NarratorController.controller.StopSpeech();
			// Look for next cutscenes
			if (startingMomentsList.Count > 0) {
				while (startingMomentsList.Count > 0 && startingMomentsList[0].type == MomentTypeEnum.Cutscene) {
					startingMomentsList.RemoveAt(0);
                }
				// If every element at starting list was removed, look in the next moments list
				while (startingMomentsList.Count == 0 && gameplayMomentsList[0].type == MomentTypeEnum.Cutscene) {
					gameplayMomentsList.RemoveAt(0);
				}
            } else if (gameplayMomentsList.Count > 0) {
				while (gameplayMomentsList.Count > 0 && gameplayMomentsList[0].type == MomentTypeEnum.Cutscene) {
					gameplayMomentsList.RemoveAt(0);
				}
				// If every element at gameplay list was removed, look in the next moments list
				while (gameplayMomentsList.Count == 0 && endingMomentsList[0].type == MomentTypeEnum.Cutscene) {
					endingMomentsList.RemoveAt(0);
				}
			} else if (endingMomentsList.Count > 0) {
				while (endingMomentsList.Count > 0 && endingMomentsList[0].type == MomentTypeEnum.Cutscene) {
					endingMomentsList.RemoveAt(0);
				}
			}
		}
	}

	private void CalculatePlayableMomentsDuration() {
		// Restart duration for 0 or current moment duration, if applicable
		if (currentMoment.type != MomentTypeEnum.Cutscene && currentMoment.momentState != MomentSpawnStateEnum.NoSpawn) {
			playableMomentsDuration = GetCurrentMomentDuration();
		} else {
			playableMomentsDuration = 0;
        }

		foreach (StageMoment moment in gameplayMomentsList){
			if (moment.type != MomentTypeEnum.Cutscene && moment.momentState != MomentSpawnStateEnum.NoSpawn) {
				playableMomentsDuration += moment.duration;
            }
        }
	}

	// Calculate time left of gameplay in which there can be spawns
	public override float TimeLeftBeforeNoSpawn() {
		if (currentMoment.momentState == MomentSpawnStateEnum.NoSpawn) {
			return 0;
        } else {
			float timeLeft = currentMoment.duration - (Time.time - currentMoment.GetStartTime());

			// Add in the next moment
			StageMoment nextMoment = GetNextMoment();
			if (nextMoment != null && nextMoment.momentState != MomentSpawnStateEnum.NoSpawn) {
				timeLeft += nextMoment.duration;
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
	ElementsEnum CheckForNewElement(List<ElementsEnum> currentElements, Dictionary<ElementsEnum, bool> elementsSeen) {
		foreach (ElementsEnum elementSeen in elementsSeen.Keys) {
			if (!elementsSeen[elementSeen]) {
				if (currentElements.Contains(elementSeen)) {
					return elementSeen;
				}
			}
		}
		return 0;
	}

	[SerializeField]
	ElementEncounterStageMoment[] elementMoments;

	void LoadSpecificElementEvent(ElementsEnum newElement) {
		// Load event to know new element
		foreach (ElementEncounterStageMoment elementMoment in elementMoments) {
			if (elementMoment.element == newElement) {
				gameplayMomentsList.Add(elementMoment);
				break;
			}
        }
	}

	/*
	 * Getters and Setters
	 */
	public override float GetPlayableMomentsDuration() {
		return playableMomentsDuration;
	}
}
