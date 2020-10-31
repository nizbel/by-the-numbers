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
	protected const float GAME_OVER_DURATION = 1.2f;

	// Stage state constants
	public const int STARTING_STATE = 1;
	public const int GAMEPLAY_STATE = 2;
	public const int ENDING_STATE = 3;
	public const int GAME_OVER_STATE = 4;

	// Stage events constants
	protected const string PATH_JSON_EVENTS = "Json/Days/";

	protected int score = 0;

	protected int obstaclesPast = 0;

	protected int blocksCaught = 0;

	protected int rangeChangersPast = 0;

	protected bool gamePaused = false;

	protected float gameOverTimer = GAME_OVER_DURATION;

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
    public void DestroyShip() {
		//if (2 == 2) {
		//    return;
		//}

		// TODO play explosion sound

		PlayerController.controller.CrashAndBurn();

        state = GAME_OVER_STATE;

		ScreenFadeController.controller.StartFadeOut(ScreenFadeController.GAME_OVER_FADE_OUT_SPEED);
	}

	public void GameOver() {
		// Tells narrator controller to stop
		NarratorController.controller.GameOver();

		// Save game info
		StageInfo stageInfo = GameController.GetGameInfo().GetStageInfoByDay(GameController.controller.GetCurrentDay());
		stageInfo.played = true;
		GameController.controller.Save();

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
		Camera.main.GetComponent<CameraShake>().DefaultShake();
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

	public virtual void SkipCutscenes() {

    }

	// TODO Remove for production version
	public void SkipCurrentEvent() {
		currentEvent.SetStartTime(Time.time - currentEvent.GetDurationInSeconds());
    }

	public virtual int GetPlayableMomentsDuration() {
		return 0;
	}

	public virtual float TimeLeftBeforeNoSpawn() {
		return 0;
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

	public int GetCurrentEventType() {
		if (currentEvent != null) {
			return currentEvent.type;
		}
		return StageEvent.TYPE_CUTSCENE;
	}

	public bool GetGamePaused() {
		return gamePaused;
	}
}
