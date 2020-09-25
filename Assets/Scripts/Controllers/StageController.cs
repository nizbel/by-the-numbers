using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class StageController : MonoBehaviour {

	// Constants
	public const float GHOST_DATA_GATHER_INTERVAL = 0.1f;
	private const float RANGE_CHANGER_SPAWN_INTERVAL = 15;
	public const int SHIP_VALUE_LIMIT = 15;

	// Stage state constants
	public const int STARTING_STATE = 0;
	public const int COMMON_RANDOM_SPAWN_STATE = 1;
	public const int OBSTACLE_GALORE_STATE = 2;
	public const int OPERATION_BLOCK_GALORE_STATE = 3;

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

	// For range changer cration
	public GameObject rangeChangerPrefab;

	float lastRangeChangerSpawned;

	//TODO initial state has to be STARTING_STATE
	int state = COMMON_RANDOM_SPAWN_STATE;

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
	void Start () {
		// Get player objects
		playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
		playerShipScript = playerTransform.gameObject.GetComponent<PlayerShip>();
		playerShipTransform = playerShipScript.transform;

		// Get score object
		scoreText = GameObject.FindGameObjectWithTag("Score").GetComponent<TextMesh>();

		// Keep track for range changer spawning
		lastRangeChangerSpawned = Time.timeSinceLevelLoad;

		// Start NarratorController
		NarratorController.controller.StartGame();
	}
	
	// Update is called once per frame
	void Update () {
		// Game Over
		if ((playerShipScript.GetValue() < ValueRange.rangeController.GetMinValue()) ||
		    (playerShipScript.GetValue() > ValueRange.rangeController.GetMaxValue())) {
            GameOver();
        }

		// Check if range changer should be spawned
		if (Time.timeSinceLevelLoad - lastRangeChangerSpawned > RANGE_CHANGER_SPAWN_INTERVAL) {
			GameObject newRangeChanger = (GameObject) Instantiate(rangeChangerPrefab, new Vector3(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x + 2, 0, 0),
			                                                      transform.rotation);
			lastRangeChangerSpawned = Time.timeSinceLevelLoad;
		}

		// Check narrator
		if ((playerShipScript.GetValue() == ValueRange.rangeController.GetMinValue()) ||
			(playerShipScript.GetValue() == ValueRange.rangeController.GetMaxValue())) {
			NarratorController.controller.WarnRange();
		}

		// Update score text
		scoreText.text = score.ToString();
	}

	// Method for game over
	public void GameOver() {
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
        GameController.controller.ChangeState(GameController.GAME_OVER);
    }

	// Method when player hits a block
	public void BlockCaught() {
		blocksCaught++;
		score++;
	}

	// Player passed though range changer
	public void PastThroughRangeChanger() {
		rangeChangersPast++;
		score++;

		// TODO Check if event is not happening soon to change state
		if (GameController.RollChance(50)) {
			if (state == COMMON_RANDOM_SPAWN_STATE) {
				state = OBSTACLE_GALORE_STATE;
			}
			else {
				state = COMMON_RANDOM_SPAWN_STATE;
			}
		}
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

	public float GetPlayerShipSpeed()
    {
		return playerShipScript.GetSpeed();
    }

	public int GetState() {
		return state;
    }

	public bool GetGamePaused() {
		return gamePaused;
    }
}
