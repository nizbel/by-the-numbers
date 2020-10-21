using UnityEngine;
using System.IO;
using UnityEngine.Experimental.Rendering.Universal;
using System.Collections.Generic;

public abstract class StageController : MonoBehaviour {

	// Constants
	public const float GHOST_DATA_GATHER_INTERVAL = 0.1f;
	protected const float MIN_RANGE_CHANGER_SPAWN_INTERVAL = 10;
	protected const float MAX_RANGE_CHANGER_SPAWN_INTERVAL = 15;
	public const float WARNING_PERIOD_BEFORE_RANGE_CHANGER = 5.5f;
	public const int SHIP_VALUE_LIMIT = 15;

	// Stage state constants
	public const int STARTING_STATE = 1;
	public const int GAMEPLAY_STATE = 2;
	public const int ENDING_STATE = 3;

	// Stage events constants
	protected const string PATH_JSON_EVENTS = "Json/Days/";

	protected int score = 0;

	protected int obstaclesPast = 0;

	protected int blocksCaught = 0;

	protected int rangeChangersPast = 0;

	protected bool gamePaused = false;

	// Player data
	protected Transform playerShipTransform;

	// Score text object during stage
	protected TextMesh scoreText;
	protected bool showScore;

	// For range changer creation
	public GameObject rangeChangerPrefab;
	public GameObject rangeChangeWarningPrefab;

	// Range changer variables
	protected float lastRangeChangerSpawned;
	protected float currentRangeChangerSpawnTimer;
	protected bool rangeChangerWarned = false;
	protected bool nextRangeChangerPositive;
	// Keeps track of whether range changers are spawning
	protected bool rangeChangersSpawning = false;

	// Current stage state
	// TODO Remove serializeField
	[SerializeField]
	protected int state = STARTING_STATE;

	// Current day special charges (used for special spawns)
	protected int currentSpecialCharges = 0;

	// Current event
	protected StageEvent currentEvent = null;

	// Controls the foreground layers that moves objects in the foreground
	protected List<ForegroundLayer> foregroundLayers = new List<ForegroundLayer>();

	public static StageController controller;

    void Awake() {
        if (controller == null) {
            controller = this;
		}
        else {
            Destroy(gameObject);
        }
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
		if (showScore) {
			scoreText.text = score.ToString();
		}
    }

	// Defines whether current event has range changers
	protected bool ShouldSpawnRangeChangers() {
		return currentEvent.hasRangeChangers;
    }

	// Define current range changer timer to appear
	protected void DefineRangeChangerSpawn() {
		currentRangeChangerSpawnTimer = Random.Range(MIN_RANGE_CHANGER_SPAWN_INTERVAL, MAX_RANGE_CHANGER_SPAWN_INTERVAL);
	}

	protected void SpawnRangeChanger() {
        GameObject newRangeChanger = (GameObject)Instantiate(rangeChangerPrefab, new Vector3(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x + 2, 0, 0),
                                                                      transform.rotation);
		// Set whether it is positive
		newRangeChanger.GetComponent<RangeChanger>().SetPositive(nextRangeChangerPositive);

		lastRangeChangerSpawned = Time.timeSinceLevelLoad;
		DefineRangeChangerSpawn();
		nextRangeChangerPositive = DefineNextRangeChangerType();
		rangeChangerWarned = false;
	}

	protected bool DefineNextRangeChangerType() {
		return GameController.RollChance(50);
	}

	// Show warning regarding range changer
	protected void WarnAboutRangeChanger() {
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
		Camera.main.GetComponent<CameraShake>().enabled = true;
	}

	// Warn about incoming special event danger
	public void PanelWarnDanger() {
		AudioSource panelWarning = playerShipTransform.Find("Panel").GetComponent<AudioSource>();
		panelWarning.Play();
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

	public void UseSpecialCharges(int chargesAmount) {
		currentSpecialCharges -= chargesAmount;
		Debug.Log("Charges left: " + currentSpecialCharges);
	}

	public void AddForegroundLayer(ForegroundLayer layer) {
		foregroundLayers.Add(layer);
		// Remove old layer
		if (foregroundLayers.Count > 1) {
			foregroundLayers.RemoveAt(0);
		}
    }

	public ForegroundLayer GetCurrentForegroundLayer() {
		return foregroundLayers[0];
    }

	// TODO Remove for production version
	public void SkipCurrentEvent() {
		currentEvent.SetStartTime(-3600);
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

	public Transform GetPlayerShipTransform() {
		return playerShipTransform;
	}


    public int GetState() {
        return state;
    }

	public int GetCurrentSpecialCharges() {
		return currentSpecialCharges;
    }

	public int GetCurrentEventDuration() {
		if (currentEvent != null) {
			return currentEvent.GetDurationInSeconds();
		}
		return 0;
	}

	public float GetCurrentEventStartTime() {
		if (currentEvent != null) {
			return currentEvent.GetStartTime();
		}
		return 0;
	}

	public int GetCurrentEventState() {
		if (currentEvent != null) {
			return currentEvent.eventState;
		}
		return StageEvent.NO_SPAWN;
	}

	public bool GetGamePaused() {
		return gamePaused;
	}
}
