using UnityEngine;
using System.IO;
using UnityEngine.Experimental.Rendering.Universal;
using System.Collections.Generic;

public class StageController : MonoBehaviour {

	// Constants
	public const float GHOST_DATA_GATHER_INTERVAL = 0.1f;
	private const float MIN_RANGE_CHANGER_SPAWN_INTERVAL = 12;
	private const float MAX_RANGE_CHANGER_SPAWN_INTERVAL = 25;
	public const float WARNING_PERIOD_BEFORE_RANGE_CHANGER = 5.5f;
	public const int SHIP_VALUE_LIMIT = 15;

	// Stage state constants
	public const int STARTING_STATE = 1;
	public const int GAMEPLAY_STATE = 2;
	public const int ENDING_STATE = 3;

	// Stage events constants
	private const string PATH_JSON_EVENTS = "Json/Days/";

	int score = 0;

	int obstaclesPast = 0;

	int blocksCaught = 0;

	int rangeChangersPast = 0;

	bool gamePaused = false;

	// Player data
	Transform playerTransform;
	PlayerShip playerShipScript;
	Transform playerShipTransform;

	// Score text object during stage
	TextMesh scoreText;

	// For range changer creation
	public GameObject rangeChangerPrefab;
	public GameObject rangeChangeWarningPrefab;

	// Range changer variables
	float lastRangeChangerSpawned;
	float currentRangeChangerSpawnTimer;
	bool rangeChangerWarned = false;
	bool nextRangeChangerPositive;

	// Stage events
	List<StageEvent> startingEventsList = new List<StageEvent>();
	List<StageEvent> gameplayEventsList = new List<StageEvent>();
	List<StageEvent> endingEventsList = new List<StageEvent>();

	// Current stage state
	// TODO Remove serializeField
	[SerializeField]
	int state = STARTING_STATE;

	// Current event
	StageEvent currentEvent = null;

	public static StageController controller;

	void Awake() {
		if (controller == null) {
			controller = this;
		}
		else {
			Destroy(gameObject);
		}
	}

	// Use this for initialization
	void Start() {
		// Start narrator controller
		NarratorController.controller.StartGame();

		// Load data for the day
		LoadCurrentDayData();

		// Get player objects
		playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
		playerShipScript = playerTransform.gameObject.GetComponent<PlayerShip>();
		playerShipTransform = playerShipScript.transform;

		// Get score object
		scoreText = GameObject.FindGameObjectWithTag("Score").GetComponent<TextMesh>();

		// Keep track for range changer spawning
		lastRangeChangerSpawned = Time.timeSinceLevelLoad;
		DefineRangeChangerSpawn();
		nextRangeChangerPositive = DefineNextRangeChangerType();

		//// Start NarratorController
		//NarratorController.controller.StartGame();
	}

	// Update is called once per frame
	void Update() {
		// Game Over
		if ((playerShipScript.GetValue() < ValueRange.rangeController.GetMinValue()) ||
			(playerShipScript.GetValue() > ValueRange.rangeController.GetMaxValue())) {
			GameOver();
		}

		// Check if range changer can still spawn
		if (state != STARTING_STATE && state != ENDING_STATE) {
			// Check if should warn about range changer
			if (!rangeChangerWarned && Time.timeSinceLevelLoad - lastRangeChangerSpawned > currentRangeChangerSpawnTimer - WARNING_PERIOD_BEFORE_RANGE_CHANGER) {
				WarnAboutRangeChanger();
			}

			// Check if range changer should be spawned
			else if (Time.timeSinceLevelLoad - lastRangeChangerSpawned > currentRangeChangerSpawnTimer) {
				GameObject newRangeChanger = (GameObject)Instantiate(rangeChangerPrefab, new Vector3(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x + 2, 0, 0),
																	  transform.rotation);
				// Set whether it is positive
				newRangeChanger.GetComponent<RangeChanger>().SetPositive(nextRangeChangerPositive);

				lastRangeChangerSpawned = Time.timeSinceLevelLoad;
				DefineRangeChangerSpawn();
				nextRangeChangerPositive = DefineNextRangeChangerType();
				rangeChangerWarned = false;
			}
		}

		// Control stage events
		ControlEvents();
	}

	// Method for game over
	public void GameOver() {
		//if (2 == 2) {
		//    return;
		//}
		// Writes the final position data
		//		player.GetComponent<GhostBlockDataGenerator>().writeToFile();
		//		player.GetComponent<GhostBlockDataGenerator>().endFile();

		// Get ghost's data reader component
		//		GameObject.Find("Ghost Block").GetComponent<GhostBlockDataReader>().closeReader();

		//		File.Delete("pdata.txt");
		//		File.Copy("pdataw.txt", "pdata.txt");

		// Tells narrator controller to stop
		NarratorController.controller.GameOver();

		// Calls game controller for state change
		if (GameController.controller.GetState() == GameController.GAMEPLAY_STORY) {
			GameController.controller.ChangeState(GameController.GAME_OVER_STORY);
		}
		else {
			GameController.controller.ChangeState(GameController.GAME_OVER_INFINITE);
		}
	}

	// Method when player hits a block
	public void BlockCaught() {
		blocksCaught++;
		AddScore(1);
	}

	// Add amount to score
	public void AddScore(int amount) {
		score += amount;

		// Update score text
		scoreText.text = score.ToString();
	}

	// Define current range changer timer to appear
	private void DefineRangeChangerSpawn() {
		currentRangeChangerSpawnTimer = Random.Range(MIN_RANGE_CHANGER_SPAWN_INTERVAL, MAX_RANGE_CHANGER_SPAWN_INTERVAL);
	}

	private bool DefineNextRangeChangerType() {
		return GameController.RollChance(50);
	}

	// Show warning regarding range changer
	private void WarnAboutRangeChanger() {
		// TODO Roll chance to test if narrator will also warn

		GameObject rangeChangerWarning = GameObject.Instantiate(rangeChangeWarningPrefab);
		if (nextRangeChangerPositive) {
			rangeChangerWarning.GetComponent<Light2D>().color = new Color(0.05f, 0.05f, 0.92f);
		}
		else {
			rangeChangerWarning.GetComponent<Light2D>().color = new Color(0.92f, 0.05f, 0.05f);
		}
		rangeChangerWarned = true;
	}

	// Player passed though range changer
	public void PastThroughRangeChanger() {
		rangeChangersPast++;
		AddScore(1);

		//if (state == COMMON_RANDOM_SPAWN_STATE) {
		//	// TODO Check if event is not happening soon to change state
		//	if (GameController.RollChance(50)) {
		//		state = OBSTACLE_GALORE_STATE;
		//	} else {
		//		state = OPERATION_BLOCK_GALORE_STATE;
		//          }
		//}
		//else {
		//	state = COMMON_RANDOM_SPAWN_STATE;
		//}
	}

	// Pause game
	public void PauseGame() {
		Time.timeScale = 0;
		AudioListener.pause = true;
		gamePaused = true;
		InputController inputController = FindObjectOfType<InputController>();
		inputController.enabled = false;
	}

	// Resume game
	public void ResumeGame() {
		Time.timeScale = 1;
		AudioListener.pause = false;
		gamePaused = false;
		InputController inputController = FindObjectOfType<InputController>();
		inputController.enabled = true;
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
                }
				LoadCurrentEvent(gameplayEventsList);
			}
			else if (endingEventsList.Count > 0) {
				// Call fade out as soon as ending starts
				if (state == GAMEPLAY_STATE) {
					state = ENDING_STATE;
					ScreenFadeController.controller.RestartFadeOut();
				}
				LoadCurrentEvent(endingEventsList);
			}
			else {
				// Day over (Story mode)
				NarratorController.controller.GameOver();
				GameController.controller.SetCurrentDay(2);
				GameController.controller.ChangeState(GameController.GAMEPLAY_STORY);
			}
		}
	}

	// Set day info for story mode
	public void LoadCurrentDayData() {
		// Get current day
		int currentDay = GameController.controller.GetCurrentDay();

		// Load data from JSON
		LoadEvents(currentDay);
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

		// Change current stage state
		state = currentEvent.eventCode;

		// If event has speech, pass it to Narrator Controller
		if (currentEvent.speeches.Count > 0) {
			NarratorController.controller.StartEventSpeech(currentEvent.speeches[0]);
		}
	}

	public bool CheckIfDayOver() {
		// TODO check if action sequences are over
		return endingEventsList.Count == 0;
	}

	/*
	 * Getters and setters
	 */

	public int GetScore() {
		return score;
	}

	public int GetBlocksCaught() {
		return blocksCaught;
	}

	public void SetBlocksCaught(int blocksCaught) {
		this.blocksCaught = blocksCaught;
	}

	public int GetRangeChangersPast() {
		return rangeChangersPast;
	}

	public void SetRangeChangersPast(int rangeChangersPast) {
		this.rangeChangersPast = rangeChangersPast;
	}

	public Transform GetPlayerTransform() {
		return playerTransform;
	}

	public Transform GetPlayerShipTransform() {
		return playerShipTransform;
	}

	public float GetPlayerShipSpeed() {
		return playerShipScript.GetSpeed();
	}

	public int GetState() {
		return state;
	}

	public void SetState(int state) {
		this.state = state;
	}

	public int GetCurrentEvent() {
		return currentEvent.eventCode;
	}

	public bool GetGamePaused() {
		return gamePaused;
	}
}
